#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b937c83e7c71a8b985b1c985c1900052a9a753e1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Private.Pages_Private_SearchDetailResults), @"mvc.1.0.view", @"/Pages/Private/SearchDetailResults.cshtml")]
namespace gpconnect_appointment_checker.Pages.Private
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/_ViewImports.cshtml"
using gpconnect_appointment_checker;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
using gpconnect_appointment_checker.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
using gpconnect_appointment_checker.Helpers.Constants;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b937c83e7c71a8b985b1c985c1900052a9a753e1", @"/Pages/Private/SearchDetailResults.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Private_SearchDetailResults : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<SearchDetailModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "StatsCount", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("nhsuk-button"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page", "SearchDetail", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "ExportSearchResults", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("submit"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "SearchResultsTableHeader", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n<div class=\"nhsuk-grid-row\">\n    <div class=\"nhsuk-grid-column-full\">\n        <div class=\"nhsuk-care-card\">\n            <div class=\"nhsuk-care-card__heading-container\">\n                <h3 class=\"nhsuk-care-card__heading\"><span role=\"text\">");
#nullable restore
#line 9 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                  Write(SearchConstants.SEARCHRESULTSSEARCHATTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></h3>\n                <span class=\"nhsuk-care-card__arrow\" aria-hidden=\"true\"></span>\n            </div>\n            <div class=\"nhsuk-care-card__content\">\n                <p>");
#nullable restore
#line 13 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
              Write(Model.SearchAtResultsText);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n                <p><em>");
#nullable restore
#line 14 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                  Write(SearchConstants.SEARCHRESULTSPUBLISHERLABEL);

#line default
#line hidden
#nullable disable
            WriteLiteral("</em>&nbsp;");
#nullable restore
#line 14 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                         Write(Model.ProviderPublisher);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n            </div>\n        </div>\n        <div class=\"nhsuk-care-card\">\n            <div class=\"nhsuk-care-card__heading-container\">\n                <h3 class=\"nhsuk-care-card__heading\"><span role=\"text\">");
#nullable restore
#line 19 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                  Write(SearchConstants.SEARCHRESULTSSEARCHONBEHALFOFTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></h3>\n                <span class=\"nhsuk-care-card__arrow\" aria-hidden=\"true\"></span>\n            </div>\n            <div class=\"nhsuk-care-card__content\"><p>");
#nullable restore
#line 22 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                Write(Html.Raw(Model.SearchOnBehalfOfResultsText));

#line default
#line hidden
#nullable disable
            WriteLiteral("</p></div>\n        </div>\n\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "b937c83e7c71a8b985b1c985c1900052a9a753e18634", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n\n");
#nullable restore
#line 27 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
         if (Model.SearchResultsTotalCount > 0)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"nhsuk-grid-row\">\n                <div class=\"nhsuk-grid-column-full\">\n\n                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b937c83e7c71a8b985b1c985c1900052a9a753e110159", async() => {
#nullable restore
#line 32 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                                                                                                          Write(SearchConstants.EXPORTSEARCHRESULTSBUTTONTEXT);

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.Page = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-searchexportid", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 32 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                                                              WriteLiteral(Model.SearchExportId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["searchexportid"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-searchexportid", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["searchexportid"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                </div>\n            </div>\n");
#nullable restore
#line 36 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
             if (Model.SearchResultsPastCount > 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <details class=\"nhsuk-details nhsuk-expander\">\n                    <summary class=\"nhsuk-details__summary\">\n                        <span class=\"nhsuk-details__summary-text\">");
#nullable restore
#line 40 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                             Write(SearchConstants.SEARCHRESULTSPASTSLOTSTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\n                    </summary>\n                    <div class=\"nhsuk-details__text\">\n");
#nullable restore
#line 43 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                         foreach (var locationGrouping in Model.SearchResultsPast)
                        {
                            var firstLocation = locationGrouping.FirstOrDefault();

                            

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                             if (firstLocation != null)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <h4>\n                                    ");
#nullable restore
#line 50 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                               Write(firstLocation.LocationName);

#line default
#line hidden
#nullable disable
            WriteLiteral("<br />\n");
#nullable restore
#line 51 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                     if (!firstLocation.NoAddressProvided)
                                    {
                                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 53 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                   Write(firstLocation.LocationAddressLinesAsString);

#line default
#line hidden
#nullable disable
#nullable restore
#line 53 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                   
                                    }
                                    else
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <em>");
#nullable restore
#line 57 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                       Write(SearchConstants.SEARCHRESULTSNOADDRESSPROVIDEDTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</em>\n");
#nullable restore
#line 58 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </h4>\n");
#nullable restore
#line 60 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <div class=\"nhsuk-table__panel-with-heading-tab\">\n                                <table role=\"table\" class=\"nhsuk-table-responsive\">\n                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "b937c83e7c71a8b985b1c985c1900052a9a753e117759", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                                    <tbody class=\"nhsuk-table__body\">\n");
#nullable restore
#line 66 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                         foreach (var t in locationGrouping)
                                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                            <tr role=\"row\" class=\"nhsuk-table__row\">\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 69 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.AppointmentDate.DateFormatter("ddd dd MMM yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 70 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.SessionName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 71 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.StartTime.DateFormatter("t"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 72 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.Duration.UnitsFormatter("mins"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 73 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.SlotType);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 74 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                                     Write(t.DeliveryChannel);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                                <td role=\"cell\" class=\"nhsuk-table__cell\">\n                                                    ");
#nullable restore
#line 76 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                               Write(t.PractitionerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                                                    <div>");
#nullable restore
#line 77 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                    Write(t.PractitionerRole);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n                                                    <div>");
#nullable restore
#line 78 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                    Write(t.PractitionerGender.FirstCharToUpper());

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n                                                </td>\n                                            </tr>\n");
#nullable restore
#line 81 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    </tbody>\n                                </table>\n                            </div>\n");
#nullable restore
#line 85 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </div>\n                </details>\n");
#nullable restore
#line 88 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 90 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
             if (Model.SearchResultsCurrentCount > 0)
            {
                foreach (var locationGrouping in Model.SearchResults)
                {
                    var firstLocation = locationGrouping.FirstOrDefault();
                    if (firstLocation != null)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <h4>\n                            ");
#nullable restore
#line 98 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                       Write(firstLocation.LocationName);

#line default
#line hidden
#nullable disable
            WriteLiteral("<br />\n");
#nullable restore
#line 99 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                             if (!firstLocation.NoAddressProvided)
                            {
                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 101 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                           Write(firstLocation.LocationAddressLinesAsString);

#line default
#line hidden
#nullable disable
#nullable restore
#line 101 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                           
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <em>");
#nullable restore
#line 105 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                               Write(SearchConstants.SEARCHRESULTSNOADDRESSPROVIDEDTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</em>\n");
#nullable restore
#line 106 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </h4>\n");
#nullable restore
#line 108 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"nhsuk-table__panel-with-heading-tab\">\n                        <table role=\"table\" class=\"nhsuk-table-responsive\">\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "b937c83e7c71a8b985b1c985c1900052a9a753e127603", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                            <tbody class=\"nhsuk-table__body\">\n");
#nullable restore
#line 113 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                 foreach (var t in locationGrouping)
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <tr role=\"row\" class=\"nhsuk-table__row\">\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 116 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.AppointmentDate.DateFormatter("ddd dd MMM yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 117 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.SessionName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 118 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.StartTime.DateFormatter("t"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 119 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.Duration.UnitsFormatter("mins"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 120 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.SlotType);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">");
#nullable restore
#line 121 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                                                             Write(t.DeliveryChannel);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                                        <td role=\"cell\" class=\"nhsuk-table__cell\">\n                                            ");
#nullable restore
#line 123 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                       Write(t.PractitionerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                                            <div>");
#nullable restore
#line 124 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                            Write(t.PractitionerRole);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n                                            <div>");
#nullable restore
#line 125 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                            Write(t.PractitionerGender.FirstCharToUpper());

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n                                        </td>\n                                    </tr>\n");
#nullable restore
#line 128 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </tbody>\n                        </table>\n                    </div>\n");
#nullable restore
#line 132 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 133 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
             
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <p>");
#nullable restore
#line 137 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
          Write(SearchConstants.SEARCHRESULTSNOAVAILABLEAPPOINTMENTSLOTSTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n");
#nullable restore
#line 138 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/SearchDetailResults.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SearchDetailModel> Html { get; private set; }
    }
}
#pragma warning restore 1591