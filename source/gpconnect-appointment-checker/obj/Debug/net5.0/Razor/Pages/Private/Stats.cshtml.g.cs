#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/Stats.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e611c92845b549d3e8743570359dae0eb6365c69"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Private.Pages_Private_Stats), @"mvc.1.0.view", @"/Pages/Private/Stats.cshtml")]
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
#line 1 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/Stats.cshtml"
using gpconnect_appointment_checker.Helpers.Constants;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/Stats.cshtml"
using gpconnect_appointment_checker.Helpers;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e611c92845b549d3e8743570359dae0eb6365c69", @"/Pages/Private/Stats.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Private_Stats : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n<div class=\"nhsuk-grid-row\">\n    <div class=\"nhsuk-grid-column-full\">\n        <div class=\"nhsuk-inset-text\">\n            <span class=\"nhsuk-u-visually-hidden\">Information: </span>\n            <p>");
#nullable restore
#line 8 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Private/Stats.cshtml"
          Write(Html.Raw(string.Format(SearchConstants.SEARCHSTATSTEXT, Model.SearchDuration.ToString("#.##s"), DateTime.Now.TimeZoneConverter("Europe/London", "d MMM yyyy HH:mm:ss"))));

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n        </div>\n    </div>\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591