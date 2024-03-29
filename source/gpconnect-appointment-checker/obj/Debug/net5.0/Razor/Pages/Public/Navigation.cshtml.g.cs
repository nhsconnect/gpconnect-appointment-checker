#pragma checksum "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ffec0a6fcf7fa5f4cbe7e8379af087d8c33047e9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(gpconnect_appointment_checker.Pages.Public.Pages_Public_Navigation), @"mvc.1.0.view", @"/Pages/Public/Navigation.cshtml")]
namespace gpconnect_appointment_checker.Pages.Public
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
#line 1 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
using gpconnect_appointment_checker.Helpers.Constants;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
using gpconnect_appointment_checker.Helpers.Enumerations;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
using gpconnect_appointment_checker.Helpers;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ffec0a6fcf7fa5f4cbe7e8379af087d8c33047e9", @"/Pages/Public/Navigation.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"32be8927f9e91edec434094dfa4f694b44875205", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Public_Navigation : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 4 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
  
    var userIsAdmin = User.GetClaimValue("IsAdmin").StringToBoolean(false);
    var userAccountStatus = User.GetClaimValue<UserAccountStatus>("UserAccountStatus");

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 9 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
 if (User.Identity.IsAuthenticated)
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <nav class=""nhsuk-header__navigation"" id=""header-navigation"" role=""navigation"" aria-label=""Primary navigation"" aria-labelledby=""label-navigation"">
        <div class=""nhsuk-width-container"">
            <p class=""nhsuk-header__navigation-title"">
                <span id=""label-navigation"">Menu</span>
                <button class=""nhsuk-header__navigation-close"" id=""close-menu"">
                    <svg class=""nhsuk-icon nhsuk-icon__close"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" aria-hidden=""true"" focusable=""false"" width=""27"" height=""27"">
                        <path d=""M13.41 12l5.3-5.29a1 1 0 1 0-1.42-1.42L12 10.59l-5.29-5.3a1 1 0 0 0-1.42 1.42l5.3 5.29-5.3 5.29a1 1 0 0 0 0 1.42 1 1 0 0 0 1.42 0l5.29-5.3 5.29 5.3a1 1 0 0 0 1.42 0 1 1 0 0 0 0-1.42z""></path>
                    </svg>
                    <span class=""nhsuk-u-visually-hidden"">Close menu</span>
                </button>
            </p>
            <ul class=""nhsuk-header__navigation-list"">

");
#nullable restore
#line 24 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                 if (userAccountStatus == UserAccountStatus.Authorised)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <li class=\"nhsuk-header__navigation-item\">\n                        <a class=\"nhsuk-header__navigation-link\" href=\"/Search\">\n                            <text>");
#nullable restore
#line 28 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                             Write(SearchConstants.SEARCHTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</text>
                            <svg class=""nhsuk-icon nhsuk-icon__chevron-right"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" aria-hidden=""true"" width=""34"" height=""34"">
                                <path d=""M15.5 12a1 1 0 0 1-.29.71l-5 5a1 1 0 0 1-1.42-1.42l4.3-4.29-4.3-4.29a1 1 0 0 1 1.42-1.42l5 5a1 1 0 0 1 .29.71z""></path>
                            </svg>
                        </a>
                    </li>
");
#nullable restore
#line 34 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                     if (userIsAdmin)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <li class=\"nhsuk-header__navigation-item\">\n                            <a class=\"nhsuk-header__navigation-link\" href=\"/Admin\">\n                                <text>");
#nullable restore
#line 38 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                                 Write(SearchConstants.ADMINTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</text>
                                <svg class=""nhsuk-icon nhsuk-icon__chevron-right"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" aria-hidden=""true"" width=""34"" height=""34"">
                                    <path d=""M15.5 12a1 1 0 0 1-.29.71l-5 5a1 1 0 0 1-1.42-1.42l4.3-4.29-4.3-4.29a1 1 0 0 1 1.42-1.42l5 5a1 1 0 0 1 .29.71z""></path>
                                </svg>
                            </a>
                        </li>
                        <li class=""nhsuk-header__navigation-item"">
                            <a class=""nhsuk-header__navigation-link"" href=""/Reports"">
                                <text>");
#nullable restore
#line 46 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                                 Write(SearchConstants.REPORTSTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</text>
                                <svg class=""nhsuk-icon nhsuk-icon__chevron-right"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" aria-hidden=""true"" width=""34"" height=""34"">
                                    <path d=""M15.5 12a1 1 0 0 1-.29.71l-5 5a1 1 0 0 1-1.42-1.42l4.3-4.29-4.3-4.29a1 1 0 0 1 1.42-1.42l5 5a1 1 0 0 1 .29.71z""></path>
                                </svg>
                            </a>
                        </li>
");
#nullable restore
#line 52 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <li class=\"nhsuk-header__navigation-item\">\n                        <a class=\"nhsuk-header__navigation-link\" href=\"/Help\">\n                            <text>");
#nullable restore
#line 55 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                             Write(SearchConstants.HELPTEXT);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</text>
                            <svg class=""nhsuk-icon nhsuk-icon__chevron-right"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24"" aria-hidden=""true"" width=""34"" height=""34"">
                                <path d=""M15.5 12a1 1 0 0 1-.29.71l-5 5a1 1 0 0 1-1.42-1.42l4.3-4.29-4.3-4.29a1 1 0 0 1 1.42-1.42l5 5a1 1 0 0 1 .29.71z""></path>
                            </svg>
                        </a>
                    </li>
");
#nullable restore
#line 61 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </ul>\n        </div>\n    </nav>\n");
#nullable restore
#line 65 "/Users/martin/Documents/GitHub/gpconnect-appointment-checker/source/gpconnect-appointment-checker/Pages/Public/Navigation.cshtml"
}

#line default
#line hidden
#nullable disable
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
