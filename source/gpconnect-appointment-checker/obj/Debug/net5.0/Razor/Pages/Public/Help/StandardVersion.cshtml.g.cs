#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Help/StandardVersion.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2294f28a69e84c583d25170fe0301c290f4a261a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Public.Help.Pages_Public_Help_StandardVersion), @"mvc.1.0.view", @"/Pages/Public/Help/StandardVersion.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2294f28a69e84c583d25170fe0301c290f4a261a", @"/Pages/Public/Help/StandardVersion.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Public_Help_StandardVersion : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<h3>Standard Version</h3>
<h3>Search at</h3>
<p><strong>Provider ODS code:</strong> enter the ODS code of the service whose appointment configuration you want to check.</p>
<h3>Search on behalf of</h3>
<div class=""nhsuk-do-dont-list"">
    <h3 class=""nhsuk-do-dont-list__label"">If you do NOT have permission to search by organisation type</h3>
    <ul class=""nhsuk-list"">
        <li><strong>Consumer ODS code:</strong> enter the ODS code of a service which will be looking for free slots at the Provider.</li>
    </ul>
</div>
<div class=""nhsuk-do-dont-list"">
    <h3 class=""nhsuk-do-dont-list__label"">If you have permission to search by organisation type</h3>
    <ul class=""nhsuk-list"">
        <li><strong>Consumer ODS code:</strong> enter the ODS code of a service which will be looking for free slots at the Provider. This may be left blank if a Consumer organisation type (see below) is selected.</li>
        <li><strong>Consumer organisation type:</strong> select an organisation type from the drop-down list, to ret");
            WriteLiteral(@"urn slots which have been restricted to this type of consumer, or which have not been restricted by type of consumer.  Leave the field blank to return slots which have not been restricted by the provider to any type or consumer. In this case, a consumer ODS code must be entered.</li>
        <li>Either a consumer ODS code must be entered or an organisation type must be selected, or both.</li>
    </ul>
</div>
<p><strong>Date range:</strong> select a week (Monday – Sunday) from the drop-down list, that you want to run the search for.</p>
<p>Depending on the system in use at the Provider service, the results of the search may include free slots which are in the past. In this case, these slots will be visible by clicking on the 'Past Appointment Slots' expander.</p>
<p>Otherwise, only free slots in the future (after the current date and current time) will be included. For example, if you run the checker on Friday, only slots for Friday (after the current time), Saturday and Sunday will be included.</p>
<p>Press ");
            WriteLiteral("the \'Search for free slots\' button to run the search.</p>\n<p>Click on the green \'Export results\' button to export to an Excel spreadsheet.</p>");
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
