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
using NSubstitute; // Using NSubstitute instead of Moq

namespace EndUserTests;

public class SearchModelTests
{
    private readonly HttpContext _mockHttpContext;
    private readonly HttpRequest _mockHttpRequest;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly IExportService _mockExportService;
    private readonly IApplicationService _mockApplicationService;
    private readonly IConfigurationService _mockConfigurationService;
    private readonly ISearchService _mockSearchService;
    private readonly ILogger<SearchModel> _mockLogger;
    private readonly SearchModel _searchModel;

    public SearchModelTests()
    {
        var routeData = new RouteData();
        routeData.Values.Add("controller", "Search");

        _mockHttpRequest = Substitute.For<HttpRequest>();
        _mockHttpRequest.Scheme.Returns("https"); 
        _mockHttpRequest.Host.Returns(new HostString("search")); 
        _mockHttpRequest.PathBase.Returns(new PathString("")); 

        _mockHttpContext = Substitute.For<HttpContext>();
        _mockHttpContext.Request.Returns(_mockHttpRequest);

        var modelState = new ModelStateDictionary();
        var actionContext =
            new ActionContext(_mockHttpContext, routeData, new PageActionDescriptor(), modelState);

        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var tempData = new TempDataDictionary(_mockHttpContext, Substitute.For<ITempDataProvider>());
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData,
            HttpContext = _mockHttpContext
        };

        // Substitute other services
        _mockExportService = Substitute.For<IExportService>();
        _mockApplicationService = Substitute.For<IApplicationService>();
        _mockConfigurationService = Substitute.For<IConfigurationService>();
        _mockSearchService = Substitute.For<ISearchService>();
        _mockLogger = Substitute.For<ILogger<SearchModel>>();
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _searchModel = new SearchModel(
            Substitute.For<IOptions<GeneralConfig>>(),
            _mockHttpContextAccessor,
            _mockLogger,
            _mockExportService,
            _mockApplicationService,
            _mockConfigurationService,
            _mockSearchService)
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