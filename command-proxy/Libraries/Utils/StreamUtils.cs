using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Spectero.Cproxy.Libraries.Utils
{
    public static class StreamUtils
    {
        public static async Task<string> ReadStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}