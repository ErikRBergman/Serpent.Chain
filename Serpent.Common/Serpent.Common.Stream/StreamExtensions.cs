namespace Serpent.Common.Stream
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static async Task<Stream> GetMemoryStreamCopyAsync(this Stream stream)
        {
            var memoryStream = new MemoryStream((int)stream.Length);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        public static async Task<Stream> GetMemoryStreamCopyAsync(this Stream stream, CancellationToken cancellationToken)
        {
            var memoryStream = new MemoryStream((int)stream.Length);
            await stream.CopyToAsync(memoryStream, 81920, cancellationToken);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}