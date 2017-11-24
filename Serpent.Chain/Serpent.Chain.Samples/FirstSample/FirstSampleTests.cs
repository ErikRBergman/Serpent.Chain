namespace Serpent.Chain.Samples.FirstSample
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Xunit;

    public class FirstSampleTests
    {
        [Fact]
        public async Task FirstSampleTessffast()
        {
            Func<string, Task> readFilesChain;
            readFilesChain = Create.SimpleFunc<string>(
                c => c
                    .Retry(r => r.MaximumNumberOfAttempts(3).RetryDelay(TimeSpan.FromSeconds(10)))
                    .Concurrent(10)
                    .Handler(
                        async filename =>
                            {
                                Console.WriteLine("Reading " + filename);

                                using (var fileStream = File.OpenRead(filename))
                                {
                                    using (var streamReader = new StreamReader(fileStream))
                                    {
                                        var contents = await streamReader.ReadToEndAsync();

                                        if (contents.IndexOf("test", StringComparison.OrdinalIgnoreCase) != -1)
                                        {
                                            Console.WriteLine(filename + " contains the phrase 'test'");
                                        }
                                    }
                                }
                            }));

            // Get all directories
            var files = Directory.GetDirectories(@"c:\temp", "*.json", SearchOption.AllDirectories);



        }
    }
}
