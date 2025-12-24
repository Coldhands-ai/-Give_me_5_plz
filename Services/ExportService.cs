using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PayrollSystem.Models;

namespace PayrollSystem.Services
{
    public static class ExportService
    {
        public static void ExportPayslipToPdf(Payroll payroll, List<PayrollDeduction> deductions, string filePath)
        {
            var doc = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            string fontPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts), "arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var titleFont = new Font(baseFont, 18, Font.BOLD);
            var headerFont = new Font(baseFont, 12, Font.BOLD);
            var normalFont = new Font(baseFont, 11, Font.NORMAL);
            var smallFont = new Font(baseFont, 10, Font.NORMAL);

            var title = new Paragraph("РАСЧЁТНЫЙ ЛИСТОК", titleFont)
            {
                Alignment = Element.ALIGN_CENTER
            };
            doc.Add(title);
            doc.Add(new Paragraph("\n"));

            var infoTable = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            infoTable.SetWidths(new float[] { 1, 2 });

            AddCell(infoTable, "Сотрудник:", headerFont);
            AddCell(infoTable, payroll.EmployeeName, normalFont);
            AddCell(infoTable, "Период:", headerFont);
            AddCell(infoTable, payroll.Period, normalFont);
            AddCell(infoTable, "Отработано часов:", headerFont);
            AddCell(infoTable, payroll.WorkedHours.ToString("N1"), normalFont);

            doc.Add(infoTable);
            doc.Add(new Paragraph("\n"));

            var accrualHeader = new Paragraph("НАЧИСЛЕНИЯ", headerFont);
            doc.Add(accrualHeader);

            var accrualTable = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            accrualTable.SetWidths(new float[] { 3, 1 });

            AddCell(accrualTable, "За отработанное время:", normalFont);
            AddCell(accrualTable, payroll.BaseSalary.ToString("N2") + " руб.", normalFont, Element.ALIGN_RIGHT);
            AddCell(accrualTable, "Премия:", normalFont);
            AddCell(accrualTable, payroll.Bonus.ToString("N2") + " руб.", normalFont, Element.ALIGN_RIGHT);

            decimal totalAccrual = payroll.BaseSalary + payroll.Bonus;
            var totalAccrualLabel = new PdfPCell(new Phrase("ИТОГО начислено:", headerFont))
            {
                Border = Rectangle.TOP_BORDER,
                PaddingTop = 5
            };
            accrualTable.AddCell(totalAccrualLabel);

            var totalAccrualValue = new PdfPCell(new Phrase(totalAccrual.ToString("N2") + " руб.", headerFont))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 5
            };
            accrualTable.AddCell(totalAccrualValue);

            doc.Add(accrualTable);
            doc.Add(new Paragraph("\n"));

            var deductionHeader = new Paragraph("УДЕРЖАНИЯ", headerFont);
            doc.Add(deductionHeader);

            var deductionTable = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            deductionTable.SetWidths(new float[] { 3, 1 });

            decimal totalDeductions = 0;
            foreach (var d in deductions)
            {
                AddCell(deductionTable, d.DeductionName + ":", normalFont);
                AddCell(deductionTable, d.Amount.ToString("N2") + " руб.", normalFont, Element.ALIGN_RIGHT);
                totalDeductions += d.Amount;
            }

            var totalDeductionLabel = new PdfPCell(new Phrase("ИТОГО удержано:", headerFont))
            {
                Border = Rectangle.TOP_BORDER,
                PaddingTop = 5
            };
            deductionTable.AddCell(totalDeductionLabel);

            var totalDeductionValue = new PdfPCell(new Phrase(totalDeductions.ToString("N2") + " руб.", headerFont))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 5
            };
            deductionTable.AddCell(totalDeductionValue);

            doc.Add(deductionTable);
            doc.Add(new Paragraph("\n"));

            var totalTable = new PdfPTable(2)
            {
                WidthPercentage = 100
            };
            totalTable.SetWidths(new float[] { 3, 1 });

            var totalFont = new Font(baseFont, 14, Font.BOLD);

            var toPayLabel = new PdfPCell(new Phrase("К ВЫПЛАТЕ:", totalFont))
            {
                Border = Rectangle.BOX,
                Padding = 10,
                BackgroundColor = new BaseColor(230, 230, 230)
            };
            totalTable.AddCell(toPayLabel);

            var toPayValue = new PdfPCell(new Phrase(payroll.NetSalary.ToString("N2") + " руб.", totalFont))
            {
                Border = Rectangle.BOX,
                Padding = 10,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                BackgroundColor = new BaseColor(230, 230, 230)
            };
            totalTable.AddCell(toPayValue);

            doc.Add(totalTable);
            doc.Add(new Paragraph("\n\n"));

            doc.Add(new Paragraph("Бухгалтер: ___________________ / ___________________", smallFont));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("Дата: ___________________", smallFont));

            doc.Close();
        }

        private static void AddCell(PdfPTable table, string text, Font font, int alignment = Element.ALIGN_LEFT)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = alignment,
                Padding = 3
            };
            table.AddCell(cell);
        }

        public static void ExportPayrollToExcel(List<Payroll> payrolls, string period, string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Ведомость");

                ws.Cells["A1"].Value = "ЗАРПЛАТНАЯ ВЕДОМОСТЬ";
                ws.Cells["A1:H1"].Merge = true;
                ws.Cells["A1"].Style.Font.Size = 16;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells["A2"].Value = "Период: " + period;
                ws.Cells["A2:H2"].Merge = true;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int row = 4;
                ws.Cells[row, 1].Value = "№";
                ws.Cells[row, 2].Value = "Сотрудник";
                ws.Cells[row, 3].Value = "Часы";
                ws.Cells[row, 4].Value = "Начислено";
                ws.Cells[row, 5].Value = "Премия";
                ws.Cells[row, 6].Value = "Штраф";
                ws.Cells[row, 7].Value = "Удержания";
                ws.Cells[row, 8].Value = "К выплате";

                using (var range = ws.Cells[row, 1, row, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int num = 1;
                decimal totalNet = 0;
                foreach (var p in payrolls)
                {
                    row++;
                    ws.Cells[row, 1].Value = num++;
                    ws.Cells[row, 2].Value = p.EmployeeName;
                    ws.Cells[row, 3].Value = p.WorkedHours;
                    ws.Cells[row, 4].Value = p.BaseSalary;
                    ws.Cells[row, 5].Value = p.Bonus;
                    ws.Cells[row, 6].Value = p.Penalty;
                    ws.Cells[row, 7].Value = p.TotalDeductions;
                    ws.Cells[row, 8].Value = p.NetSalary;

                    ws.Cells[row, 3].Style.Numberformat.Format = "#,##0.0";
                    ws.Cells[row, 4, row, 8].Style.Numberformat.Format = "#,##0.00";

                    totalNet += p.NetSalary;
                }

                row++;
                ws.Cells[row, 1, row, 7].Merge = true;
                ws.Cells[row, 1].Value = "ИТОГО:";
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[row, 8].Value = totalNet;
                ws.Cells[row, 8].Style.Font.Bold = true;
                ws.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";

                ws.Cells[4, 1, row, 8].AutoFitColumns();
                ws.Column(2).Width = 30;

                using (var range = ws.Cells[4, 1, row, 8])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}
