namespace Serpent.Chain.Samples.FirstSample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class FirstSample
    {
        private readonly Func<IEnumerable<string>, Task> readFilesFunc;

        public FirstSample()
        {
            this.readFilesFunc = Create.SimpleFunc<IEnumerable<string>>(
                c => c
                    .SelectMany(m => m)
                    .Retry(r => r.MaximumNumberOfAttempts(3).RetryDelay(TimeSpan.FromSeconds(10)))
                    .Concurrent(10)
                    .Handler(ReadFileAndCheckForTest));
        }

        public Task ReadFilesAndCheckForTest(IEnumerable<string> filenames)
        {
            return this.readFilesFunc(filenames);
        }

        private static async Task ReadFileAndCheckForTest(string filename)
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
        }
    }
}
