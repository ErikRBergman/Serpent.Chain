namespace Serpent.Common.ServiceStorage
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Serpent.Common.Async;
    using Serpent.Common.Stream;

    public class JsonDiskServiceStorageService<T> : IServiceStorageService<T>
    {
        private readonly string filename;

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly JsonSerializer serializer = new JsonSerializer()
                                                         {
                                                             Formatting = Formatting.Indented
                                                         };

        public JsonDiskServiceStorageService(JsonDiskServiceStorageServiceOptions<T> options)
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

        public async Task<T> GetOrCreateStorageAsync(Func<T> func)
        {
            using (await this.semaphoreSlim.LockAsync())
            {
                if (File.Exists(this.filename))
                {
                    var stream = await this.GetMemoryStreamFromFile();
                    var resultStorage = this.DeserializeStream(stream);

                    if (resultStorage != null)
                    {
                        return resultStorage;
                    }
                }

                var storage = func();

                await this.WriteStorageToFile(storage);

                return storage;
            }
        }

        private async Task WriteStorageToFile(T storage)
        {
            var memoryStream = new MemoryStream(16 * 1024);
            this.SerializeToStream(memoryStream, storage);
            memoryStream.Position = 0;

            using (var fileStream = File.Create(this.filename))
            {
                await memoryStream.CopyToAsync(fileStream);
            }
        }

        private void SerializeToStream(Stream memoryStream, T storage)
        {
            using (var writer = new StreamWriter(memoryStream, Encoding.Default, 4096, true))
            {
                this.serializer.Serialize(new JsonTextWriter(writer), storage);
            }
        }

        public async Task UpdateStorageAsync(T storage)
        {
            using (await this.semaphoreSlim.LockAsync())
            {
                await this.WriteStorageToFile(storage);
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
                    await this.WriteStorageToFile(storage);
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