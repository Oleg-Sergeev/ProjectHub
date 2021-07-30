using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using Infrastructure.Data.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        private const string ContainerCss = "container";
        private const string PaginationCss = "pagination";
        private const string PageItemCss = "page-item";
        private const string PageLinkCss = "page-link";
        private const string ActiveCss = "active";
        private const string DisabledCss = "disabled";

        private const int PageCount = 7;


        private readonly IUrlHelperFactory _urlHelperFactory;


        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagedViewModel PagedModel { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();


        public PaginationTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PagedModel == null) return;

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            output.TagName = "div";
            output.AddClass(ContainerCss, HtmlEncoder.Default);

            var ul = new TagBuilder("ul");
            ul.AddCssClass(PaginationCss);


            var remainder = (PageCount + 1) % 2;

            var startPage = Math.Max(PagedModel.CurrentPage - PageCount / 2 + remainder, 1);
            var finishPage = Math.Min(PagedModel.CurrentPage + PageCount / 2, PagedModel.TotalPages);

            var action = ViewContext.RouteData.Values["action"].ToString();

            var firstPage = BuildPageLink(1, "«");
            if (PagedModel.CurrentPage == 1) firstPage.AddCssClass(DisabledCss);
            ul.InnerHtml.AppendHtml(firstPage);

            for (var i = startPage; i <= finishPage; i++)
            {
                var page = BuildPageLink(i);
                if (PagedModel.CurrentPage == i) page.AddCssClass(ActiveCss);

                ul.InnerHtml.AppendHtml(page);
            }

            var lastPage = BuildPageLink(PagedModel.TotalPages, "»");
            if (PagedModel.CurrentPage == PagedModel.TotalPages) lastPage.AddCssClass(DisabledCss);
            ul.InnerHtml.AppendHtml(lastPage);


            output.Content.SetHtmlContent(ul);
            output.TagMode = TagMode.StartTagAndEndTag;



            TagBuilder BuildPageLink(int page, string text = null)
            {
                var li = new TagBuilder("li");
                li.AddCssClass(PageItemCss);

                PageUrlValues["page"] = page;

                var a = new TagBuilder("a");
                a.AddCssClass(PageLinkCss);
                a.Attributes["href"] = urlHelper.Action(action, PageUrlValues);
                a.InnerHtml.Append(text ?? page.ToString());

                li.InnerHtml.AppendHtml(a);

                return li;
            }
        }
    }
}
