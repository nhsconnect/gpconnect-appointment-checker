#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Help/What.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "294b5674a8019aa901136305bafed4bb311ef60f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Public.Help.Pages_Public_Help_What), @"mvc.1.0.view", @"/Pages/Public/Help/What.cshtml")]
namespace gpconnect_appointment_checker.Pages.Public.Help
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"294b5674a8019aa901136305bafed4bb311ef60f", @"/Pages/Public/Help/What.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Public_Help_What : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<h2>What does the Appointment Checker do?</h2>
<p>The Appointment Checker shows you appointments which have been set up in a specified healthcare setting (Provider) and are available to be booked by another service (Consumer) using GP Connect. Once a search has been made, it will display any slots which have been set up correctly. Those that have been set up incorrectly will not be displayed and further investigation will be required to understand why they are configured incorrectly.</p>
<p>The 'Provider' organisation can be a GP Practice or any other organisation which has a GP Foundation system installed and is enabled for GP Connect.</p>
<p>The 'Consumer' organisation can be any organisation with an ODS code. As appointment slots may be made available by the provider to all organisations of a particular type, it is also possible to search by organisation type, if you have been granted permission to do so by the System Administrator. In this case the option to 'Select a consumer organisation type' will be s");
            WriteLiteral("hown on the search screen.</p>\n");
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
