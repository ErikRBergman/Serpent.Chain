namespace Serpent.Common.ServiceStorage.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonDiskServiceStorageServiceTests
    {
        [TestMethod]
        public async Task GetOrCreateStorageAsyncTests()
        {
            var filename = Path.GetTempFileName();

            try
            {
                var serviceStorage = new JsonDiskServiceStorageService<Test>(
                    new JsonDiskServiceStorageServiceOptions<Test>()
                        {
                            Filename = filename
                        });

                var storage = await serviceStorage.GetOrCreateStorageAsync(
                                  () => new Test
                                            {
                                                TimeStamp = new DateTime(2001, 04, 01)
                                            });

                Assert.IsNotNull(storage);

                storage = await serviceStorage.GetOrCreateStorageAsync(
                              () => new Test());

                Assert.IsNotNull(storage);

                Assert.AreEqual(new DateTime(2001, 04, 01), storage.TimeStamp.Value);
            }
            finally
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
        }

        private class Test
        {
            public DateTime? TimeStamp { get; set; }
        }
    }
}