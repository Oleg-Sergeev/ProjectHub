using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IAsyncFileTemplateParser
    {
        Task<string> ParseFileAsync(string filePath, Dictionary<string, string> templateTags);
    }
}
