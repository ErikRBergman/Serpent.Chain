namespace Serpent.Common.Stream
{
    using System.IO;
    using System.Threading.Tasks;

    public class StreamHelpers
    {
        public static async Task<Stream> LoadFileIntoMemoryStream(string filename)
        {
            using (var fileStream = File.OpenRead(filename))
            {
                return await fileStream.GetMemoryStreamCopyAsync();
            }
        }
    }
}