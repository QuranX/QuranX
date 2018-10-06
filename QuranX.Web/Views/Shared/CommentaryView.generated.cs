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
    
    #line 2 "..\..\Views\Shared\CommentaryView.cshtml"
    using QuranX.Shared.Models;
    
    #line default
    #line hidden
    using QuranX.Web;
    
    #line 1 "..\..\Views\Shared\CommentaryView.cshtml"
    using QuranX.Web.Models;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/CommentaryView.cshtml")]
    public partial class _Views_Shared_CommentaryView_cshtml : System.Web.Mvc.WebViewPage<CommentatorAndCommentary>
    {
        public _Views_Shared_CommentaryView_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\Shared\CommentaryView.cshtml"
  
	var verse = new VerseRangeReference(Model.Commentary.ChapterNumber, Model.Commentary.FirstVerseNumber, Model.Commentary.LastVerseNumber);
	string verseCaption = verse.IsMultipleVerses ? "verses" : "verse";

            
            #line default
            #line hidden
WriteLiteral("\r\n<div>\r\n\t<dl");

WriteLiteral(" class=\"boxed\"");

WriteLiteral(">\r\n\t\t<dt>\r\n\t\t\t<a");

WriteAttribute("href", Tuple.Create(" href=\"", 347), Tuple.Create("\"", 361)
, Tuple.Create(Tuple.Create("", 354), Tuple.Create("/", 354), true)
            
            #line 12 "..\..\Views\Shared\CommentaryView.cshtml"
, Tuple.Create(Tuple.Create("", 355), Tuple.Create<System.Object, System.Int32>(verse
            
            #line default
            #line hidden
, 355), false)
);

WriteLiteral("><span");

WriteLiteral(" class=\"verse-reference\"");

WriteLiteral(">");

            
            #line 12 "..\..\Views\Shared\CommentaryView.cshtml"
                                                       Write(verse);

            
            #line default
            #line hidden
WriteLiteral("</span></a>\r\n");

WriteLiteral("\t\t\t");

            
            #line 13 "..\..\Views\Shared\CommentaryView.cshtml"
       Write(Model.Commentator.Code);

            
            #line default
            #line hidden
WriteLiteral(" - ");

            
            #line 13 "..\..\Views\Shared\CommentaryView.cshtml"
                                 Write(Model.Commentator.Description);

            
            #line default
            #line hidden
WriteLiteral("\r\n\t\t</dt>\r\n");

            
            #line 15 "..\..\Views\Shared\CommentaryView.cshtml"
		
            
            #line default
            #line hidden
            
            #line 15 "..\..\Views\Shared\CommentaryView.cshtml"
         foreach (string text in Model.Commentary.Text)
		{

            
            #line default
            #line hidden
WriteLiteral("\t\t\t<dd>");

            
            #line 17 "..\..\Views\Shared\CommentaryView.cshtml"
           Write(text);

            
            #line default
            #line hidden
WriteLiteral("</dd>\r\n");

            
            #line 18 "..\..\Views\Shared\CommentaryView.cshtml"
		}

            
            #line default
            #line hidden
WriteLiteral("\t</dl>\r\n</div>\r\n\r\n");

        }
    }
}
#pragma warning restore 1591