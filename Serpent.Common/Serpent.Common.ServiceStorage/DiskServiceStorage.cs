namespace Serpent.Common.ServiceStorage
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Serpent.Common.Async;
    using Serpent.Common.Stream;

    public class JsonDiskServiceStorageService<T> : IServiceStorageService<T>
    {
        private readonly string filename;

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly JsonSerializer serializer = new JsonSerializer();

        public JsonDiskServiceStorageService(DiskServiceStorageServiceOptions<T> options)
        {
            this.filename = options.Filename;
        }

        public async Task<T> GetStorageAsync()
        {
            Stream memoryStream;

            using (await this.semaphoreSlim.LockAsync())
            {
                memoryStream = await this.GetMemoryStreamFromFile();
            }

            return this.DeserializeStream(memoryStream);
        }

        public async Task UpdateStorageAsync(T storage)
        {
            Stream memoryStream = new MemoryStream(16 * 1024);

            using (await this.semaphoreSlim.LockAsync())
            {
                this.serializer.Serialize(new StreamWriter(memoryStream), storage);
                memoryStream.Position = 0;

                using (var fileStream = File.Create(this.filename))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }
        }

        public async Task UpdateStorageAsync(Func<T, bool> updateFunc)
        {
            using (await this.semaphoreSlim.LockAsync())
            {
                var memoryStream = await this.GetMemoryStreamFromFile();
                var storage = this.DeserializeStream(memoryStream);

                if (updateFunc(storage))
                {
                    memoryStream.SetLength(0);

                    this.serializer.Serialize(new StreamWriter(memoryStream), storage);
                    memoryStream.Position = 0;

                    using (var fileStream = File.Create(this.filename))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                    }
                }
            }
        }

        private T DeserializeStream(Stream memoryStream)
        {
            return this.serializer.Deserialize<T>(new JsonTextReader(new StreamReader(memoryStream)));
        }

        private async Task<Stream> GetMemoryStreamFromFile()
        {
            using (var fileStream = File.OpenRead(this.filename))
            {
                return await fileStream.GetMemoryStreamCopyAsync();
            }
        }
    }
}