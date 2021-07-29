using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.TagHelpers
{
    [HtmlTargetElement("p", Attributes = AutoLinkAttributeName)]
    public class AutoLinkTagHelper : TagHelper
    {
        private const string AutoLinkAttributeName = "asp-auto-link";

        [HtmlAttributeName(AutoLinkAttributeName)]
        public string Pattern { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            const string CR = "&#xD;";
            char[] chars = { ' ', ',' };

            var content = (await output.GetChildContentAsync()).GetContent();

            var index = 0;
            while (true)
            {
                var linkStartIndex = content.IndexOf(Pattern, index);
                if (linkStartIndex < 0) break;

                var linkEndIndex = content.IndexOf(CR, linkStartIndex);
                if (linkEndIndex < 0) linkEndIndex = content.IndexOfAny(chars, linkStartIndex);
                if (linkEndIndex < 0) linkEndIndex = content.Length;

                var linkLength = linkEndIndex - linkStartIndex;
                var link = content.Substring(linkStartIndex, linkLength).TrimEnd('.', ';');

                var replaceString = $"<a href='{link}'>{link}</a>";

                content = content.Replace(link, replaceString);

                index = content.IndexOf("</a>", linkEndIndex);
            }

            output.Content.SetHtmlContent(content);
        }
    }
}
