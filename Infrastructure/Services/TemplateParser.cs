using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class FileTemplateParser : IAsyncFileTemplateParser
    {
        public async Task<string> ParseFileAsync(string filePath, Dictionary<string, string> templateTags)
        {
            string parsedString;

            using (StreamReader str = new(filePath))
            {
                parsedString = await str.ReadToEndAsync();
            }

            foreach (var tag in templateTags) parsedString = parsedString.Replace($"[{tag.Key}]", tag.Value);

            return parsedString;
        }
    }
}
