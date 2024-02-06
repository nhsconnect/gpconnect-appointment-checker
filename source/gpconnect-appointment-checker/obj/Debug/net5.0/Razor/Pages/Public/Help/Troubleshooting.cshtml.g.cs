#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Help/Troubleshooting.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9e342b2d55a2c94237eef10fb44e7726231b37f4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Public.Help.Pages_Public_Help_Troubleshooting), @"mvc.1.0.view", @"/Pages/Public/Help/Troubleshooting.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9e342b2d55a2c94237eef10fb44e7726231b37f4", @"/Pages/Public/Help/Troubleshooting.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Public_Help_Troubleshooting : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<h2>Troubleshooting Error Messages</h2>
<div class=""nhsuk-expander-group"">
    <details class=""nhsuk-details nhsuk-expander"">
        <summary class=""nhsuk-details__summary"">
            <span class=""nhsuk-details__summary-text"">PARTYKEY_INTERACTION_CHECK_FAILED_MESSAGERECEIVER</span>
        </summary>
        <div class=""nhsuk-details__text""><p><b>Reason: </b>The practice specified by the Provider ODS code you have entered has not been set up correctly in Spine Directory Service (SDS)</p></div>
        <div class=""nhsuk-details__text""><p><b>Solution: </b>Contact the GP Connect team at ");
#nullable restore
#line 8 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Help/Troubleshooting.cshtml"
                                                                                       Write(Html.Raw(Model.GetAccessEmailAddressLink));

#line default
#line hidden
#nullable disable
            WriteLiteral(@", including the error message details in your email</p></div>
    </details>
    <details class=""nhsuk-details nhsuk-expander"">
        <summary class=""nhsuk-details__summary"">
            <span class=""nhsuk-details__summary-text"">Access denied (ACCESS_DENIED)<br />Recipient organisation is not enabled for interaction ID</span>
        </summary>
        <div class=""nhsuk-details__text""><p><b>Reason: </b>GP Connect (or specifically GP Connect Appointments) is disabled at the practice</p></div>
        <div class=""nhsuk-details__text""><p><b>Solution: </b>The practice can re-enable this within the GP system</p></div>
    </details>
    <details class=""nhsuk-details nhsuk-expander"">
        <summary class=""nhsuk-details__summary"">
            <span class=""nhsuk-details__summary-text"">Access has been denied to process this request (ACCESS DENIED)<br />Service type disabled</span>
        </summary>
        <div class=""nhsuk-details__text""><p><b>Reason: </b>GP Connect Appointment Booking has been disabled at the p");
            WriteLiteral(@"ractice</p></div>
        <div class=""nhsuk-details__text""><p><b>Solution: </b>The practice can re-enable this within the GP system</p></div>
    </details>
    <details class=""nhsuk-details nhsuk-expander"">
        <summary class=""nhsuk-details__summary"">
            <span class=""nhsuk-details__summary-text"">Access has been denied to process this request (ACCESS DENIED)<br />GP Connect not enabled</span>
        </summary>
        <div class=""nhsuk-details__text""><p><b>Reason: </b>GP Connect has been disabled at the practice</p></div>
        <div class=""nhsuk-details__text""><p><b>Solution: </b>The practice can re-enable this within the GP system</p></div>
    </details>
</div>");
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