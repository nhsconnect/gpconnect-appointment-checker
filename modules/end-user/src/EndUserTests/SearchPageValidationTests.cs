using FluentAssertions;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Pages;
using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace EndUserTests;

public class SearchModelTests
{
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<HttpRequest> _mockHttpRequest;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IExportService> _mockExportService;
    private readonly Mock<IApplicationService> _mockApplicationService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<ISearchService> _mockSearchService;
    private readonly Mock<ILogger<SearchModel>> _mockLogger;
    private readonly SearchModel _searchModel;

    public SearchModelTests()
    {
        var routeData = new RouteData();
        routeData.Values.Add("controller", "Search");

        _mockHttpRequest = new Mock<HttpRequest>();
        _mockHttpRequest.Setup(req => req.Scheme).Returns("https"); // Scheme like https
        _mockHttpRequest.Setup(req => req.Host).Returns(new HostString("search")); // Set the host to 'search'
        _mockHttpRequest.Setup(req => req.PathBase).Returns(new PathString("")); // Base path, can be empty

        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(x => x.Request).Returns(_mockHttpRequest.Object);


        var modelState = new ModelStateDictionary();
        var actionContext =
            new ActionContext(_mockHttpContext.Object, routeData, new PageActionDescriptor(), modelState);

        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var tempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData,
            HttpContext = _mockHttpContext.Object
        };

        // Mock other services
        _mockExportService = new Mock<IExportService>();
        _mockApplicationService = new Mock<IApplicationService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockSearchService = new Mock<ISearchService>();
        _mockLogger = new Mock<ILogger<SearchModel>>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _searchModel = new SearchModel(
            Mock.Of<IOptions<GeneralConfig>>(),
            _mockHttpContextAccessor.Object,
            _mockLogger.Object,
            _mockExportService.Object,
            _mockApplicationService.Object,
            _mockConfigurationService.Object,
            _mockSearchService.Object)
        {
            PageContext = pageContext,
            TempData = tempData,
            Url = new UrlHelper(actionContext)
        };
    }

    [Fact]
    public void ModelState_ShouldBeValid_When_1Provider_1Consumer()
    {
        // Arrange
        _searchModel.ProviderOdsCode = "22";
        _searchModel.ConsumerOdsCode = "23";

        // Act
        var result = _searchModel.ValidSearchCombination;

        // Assert
        result.Should().BeTrue();
        _searchModel.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ModelState_ShouldBeValid_When_2Provider_1Consumer()
    {
        // Arrange
        _searchModel.ProviderOdsCode = "22 29";
        _searchModel.ConsumerOdsCode = "23";

        // Act
        var result = _searchModel.ValidSearchCombination;

        // Assert
        result.Should().BeTrue();
        _searchModel.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ModelState_ShouldBeValid_When_1Provider_2Consumer()
    {
        // Arrange
        _searchModel.ProviderOdsCode = "22";
        _searchModel.ConsumerOdsCode = "23 25";

        // Act
        var result = _searchModel.ValidSearchCombination;

        // Assert
        result.Should().BeTrue();
        _searchModel.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ModelState_ShouldBeInValid_When_2Provider_2Consumer()
    {
        // Arrange
        _searchModel.ProviderOdsCode = "22 23";
        _searchModel.ConsumerOdsCode = "22 23";

        // Act
        _ = _searchModel.OnPostSearchAsync();

        // Assert
        _searchModel.ValidSearchCombination.Should().BeFalse();
        _searchModel.ModelState.IsValid.Should().BeFalse();
        _searchModel.ModelState["ProviderOdsCode"]
            ?.Errors.Should()
            .Contain(error => error.ErrorMessage == SearchConstants.IssueWithOdsCodesInputText);
    }
}