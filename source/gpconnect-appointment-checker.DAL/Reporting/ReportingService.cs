using System;
using DocumentFormat.OpenXml.Packaging;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Reporting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;

namespace gpconnect_appointment_checker.DAL.Reporting
{
    public class ReportingService : IReportingService
    {
        private readonly ILogger<ReportingService> _logger;
        private readonly IDataService _dataService;

        public ReportingService(ILogger<ReportingService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public DataTable GetReport(string functionName)
        {
            var result = _dataService.ExecuteFunctionAndGetDataTable($"reporting.{functionName}");
            return result;
        }

        public void ExportReport(string functionName)
        {
            var result = _dataService.ExecuteFunctionAndGetDataTable($"reporting.{functionName}");
            CreateReport(result);
        }

        private void CreateReport(DataTable result, string sheetName = "")
        {
            var memoryStream = new MemoryStream();
            var document = SpreadsheetDocument.Create(memoryStream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            var worksheetPart = document.AddNewPart<WorksheetPart>();

            var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

            //for (var i = 0; i < result.Rows.Count; i++)
            //{
            //    var row = new Row();
            //    for (var j = 0; j < result.Columns.Count; j++)
            //    {
            //        row.Add
            //        sheetData.AppendChild(row(new Cell()))
            //    }
            //}

            worksheetPart.Worksheet.Save();
            document.WorkbookPart.Workbook.Sheets.AppendChild(new Sheet
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = (uint)document.WorkbookPart.Workbook.Sheets.Count() + 1,
                Name = sheetName
            });

            document.WorkbookPart.Workbook.Save();
            document.Close();
        }

        public List<Report> GetReports()
        {
            var functionName = "reporting.get_reports";
            var result = _dataService.ExecuteFunction<Report>(functionName).ToList();
            return result;
        }
    }
}
