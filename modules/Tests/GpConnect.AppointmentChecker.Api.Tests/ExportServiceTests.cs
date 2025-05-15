using System.Data;
using gpconnect_appointment_checker.api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using NSubstitute;
using Xunit;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class ExportServiceTests
{
    [Fact]
    public async Task GenerateExport_Should_CallReportingService_CreateReport_With_SlotSummaryType()
    {
        // Arrange 

        var simpleSlot = new SlotSimple
        {
            CurrentSlotEntrySimple = [],
            PastSlotEntrySimple = [],
            Issue = [],
            SearchExportId = 1011
        };

        var mockQueryExecutionService = Substitute.For<IGpConnectQueryExecutionService>();
        mockQueryExecutionService.ExecuteFreeSlotSearchResultFromDatabase(Arg.Any<int>())
            .Returns(simpleSlot);


        var mockReportingService = Substitute.For<IReportingService>();
        mockReportingService.CreateReport(Arg.Any<DataTable>(), Arg.Any<string>(), reportType: ReportType.SlotSummary)
            .Returns(new MemoryStream());

        var exportRequest = new ExportRequest
        {
            ExportRequestId = 101010101,
            UserId = 101,
            ReportName = "Report Test"
        };

        var sut = new ExportService(mockQueryExecutionService, mockReportingService);

        // Act
        await sut.ExportSearchResultFromDatabase(exportRequest);

        // Assert
        mockReportingService.Received().CreateReport(
            Arg.Any<DataTable>(),
            Arg.Any<string>(),
            reportType: ReportType.SlotSummary
        );
    }

    [Fact]
    public async Task GenerateExport_ShouldNot_CallReportingService_CreateReport_With_CapabilityReportType()
    {
        // Arrange 

        var simpleSlot = new SlotSimple
        {
            CurrentSlotEntrySimple = [],
            PastSlotEntrySimple = [],
            Issue = [],
            SearchExportId = 1011
        };

        var mockQueryExecutionService = Substitute.For<IGpConnectQueryExecutionService>();
        mockQueryExecutionService.ExecuteFreeSlotSearchResultFromDatabase(Arg.Any<int>())
            .Returns(simpleSlot);


        var mockReportingService = Substitute.For<IReportingService>();
        mockReportingService.CreateReport(Arg.Any<DataTable>(), Arg.Any<string>(), reportType: ReportType.SlotSummary)
            .Returns(new MemoryStream());

        var exportRequest = new ExportRequest
        {
            ExportRequestId = 101010101,
            UserId = 101,
            ReportName = "Report Test"
        };

        var sut = new ExportService(mockQueryExecutionService, mockReportingService);

        // Act
        await sut.ExportSearchResultFromDatabase(exportRequest);

        // Assert
        mockReportingService.DidNotReceive().CreateReport(
            Arg.Any<DataTable>(),
            Arg.Any<string>(),
            reportType: ReportType.Capability
        );
    }
}