﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
    using QuranX.Persistence.Models;
    
    #line default
    #line hidden
    using QuranX.Web;
    
    #line 1 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
    using QuranX.Web.Views.HadithIndex;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/HadithIndex/HadithIndexHeader.cshtml")]
    public partial class _Views_HadithIndex_HadithIndexHeader_cshtml : System.Web.Mvc.WebViewPage<HadithIndexHeaderViewModel>
    {
        public _Views_HadithIndex_HadithIndexHeader_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<h1>Hadith collection</h1>\r\n<h2>\r\n\t<a");

WriteAttribute("href", Tuple.Create(" href=\"", 145), Tuple.Create("\"", 221)
, Tuple.Create(Tuple.Create("", 152), Tuple.Create("/Hadith/", 152), true)
            
            #line 7 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
, Tuple.Create(Tuple.Create("", 160), Tuple.Create<System.Object, System.Int32>(Model.Collection.Code
            
            #line default
            #line hidden
, 160), false)
, Tuple.Create(Tuple.Create("", 182), Tuple.Create("/", 182), true)
            
            #line 7 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
, Tuple.Create(Tuple.Create("", 183), Tuple.Create<System.Object, System.Int32>(Model.Collection.PrimaryReferenceCode
            
            #line default
            #line hidden
, 183), false)
);

WriteLiteral(">\r\n");

WriteLiteral("\t\t");

            
            #line 8 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
   Write(Model.Collection.Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n\t</a>\r\n");

            
            #line 10 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
	
            
            #line default
            #line hidden
            
            #line 10 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
      
		string urlSoFar = $"/Hadith/{Model.Collection.Code}/{Model.Collection.PrimaryReferenceCode}/";
		foreach (string urlPart in Model.ReferencePartNamesAndValues)
		{
			urlSoFar += urlPart + "/";

            
            #line default
            #line hidden
WriteLiteral("\t\t\t<span> / </span>");

WriteLiteral("<a");

WriteAttribute("href", Tuple.Create(" href=\"", 483), Tuple.Create("\"", 499)
            
            #line 15 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
, Tuple.Create(Tuple.Create("", 490), Tuple.Create<System.Object, System.Int32>(urlSoFar
            
            #line default
            #line hidden
, 490), false)
);

WriteLiteral(">");

            
            #line 15 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
                                           Write(urlPart.Replace("-", " "));

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n");

            
            #line 16 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
		}
	
            
            #line default
            #line hidden
WriteLiteral("\r\n</h2>\r\n<div");

WriteLiteral(" class=\"hadith-reference-list btn-group\"");

WriteLiteral(" role=\"group\"");

WriteLiteral(" aria-label=\"Indexes\"");

WriteLiteral(">\r\n");

            
            #line 20 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
	
            
            #line default
            #line hidden
            
            #line 20 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
     foreach (HadithReferenceDefinition referenceDefinition in Model.Collection.ReferenceDefinitions.OrderBy(x => x.Name))
	{
		string cssClass = string.Compare(referenceDefinition.Code, Model.SelectedReferenceCode, true) == 0 ? "selected" : "";

            
            #line default
            #line hidden
WriteLiteral("\t\t<a");

WriteAttribute("href", Tuple.Create(" href=\"", 880), Tuple.Create("\"", 943)
, Tuple.Create(Tuple.Create("", 887), Tuple.Create("/hadith/", 887), true)
            
            #line 23 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
, Tuple.Create(Tuple.Create("", 895), Tuple.Create<System.Object, System.Int32>(Model.Collection.Code
            
            #line default
            #line hidden
, 895), false)
, Tuple.Create(Tuple.Create("", 917), Tuple.Create("/", 917), true)
            
            #line 23 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
, Tuple.Create(Tuple.Create("", 918), Tuple.Create<System.Object, System.Int32>(referenceDefinition.Code
            
            #line default
            #line hidden
, 918), false)
);

WriteAttribute("class", Tuple.Create(" class=\"", 944), Tuple.Create("\"", 1005)
, Tuple.Create(Tuple.Create("", 952), Tuple.Create("btn", 952), true)
, Tuple.Create(Tuple.Create(" ", 955), Tuple.Create("btn-primary", 956), true)
, Tuple.Create(Tuple.Create(" ", 967), Tuple.Create("hadith-reference-list__item", 968), true)
            
            #line 23 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
                                              , Tuple.Create(Tuple.Create(" ", 995), Tuple.Create<System.Object, System.Int32>(cssClass
            
            #line default
            #line hidden
, 996), false)
);

WriteLiteral(">\r\n");

WriteLiteral("\t\t\t");

            
            #line 24 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
       Write(referenceDefinition.Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n\t\t</a>\r\n");

            
            #line 26 "..\..\Views\HadithIndex\HadithIndexHeader.cshtml"
	}

            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n\r\n");

        }
    }
}
#pragma warning restore 1591