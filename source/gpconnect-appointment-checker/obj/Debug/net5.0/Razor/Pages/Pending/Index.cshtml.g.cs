#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d4dd27857bfe85181d11e2f3917cee0a11cfe572"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Pending.Pages_Pending_Index), @"mvc.1.0.razor-page", @"/Pages/Pending/Index.cshtml")]
namespace gpconnect_appointment_checker.Pages.Pending
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
#line 2 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
using gpconnect_appointment_checker.Helpers.Constants;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d4dd27857bfe85181d11e2f3917cee0a11cfe572", @"/Pages/Pending/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Pending_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n<div class=\"nhsuk-grid-row\">\n    <div class=\"nhsuk-grid-column-full\">\n        <div class=\"nhsuk-warning-callout\">\n            <h3 class=\"nhsuk-warning-callout__label\">");
#nullable restore
#line 8 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
                                                Write(SearchConstants.REQUESTSUBMITTEDLABEL);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\n            <p>\n");
#nullable restore
#line 10 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
                 if (TempData["EmailSent"] as bool? == false)
                {
                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
               Write(Html.Raw(string.Format(SearchConstants.REQUESTSUBMITTEDERRORTEXT, TempData["EmailAddressManual"])));

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
                                                                                                                       
                }
                else
                {
                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
               Write(SearchConstants.REQUESTSUBMITTEDTEXT);

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Pending/Index.cshtml"
                                                         
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </p>\n        </div>\n    </div>\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Pages_Pending_Index> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_Pending_Index> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_Pending_Index>)PageContext?.ViewData;
        public Pages_Pending_Index Model => ViewData.Model;
    }
}
#pragma warning restore 1591