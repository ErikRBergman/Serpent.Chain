//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Serpent.Chain.Samples.FirstSample
//{
//    using System.IO;
//    using System.Threading.Tasks;

//    public class FirstSample
//    {
//        private readonly Func<string, Task> readFilesChain;

//        public FirstSample()
//        {
//            this.readFilesChain = Create.SimpleFunc<string>(
//                c => c
//                    .Retry(r => r.MaximumNumberOfAttempts(3).RetryDelay(TimeSpan.FromSeconds(10)))
//                    .Concurrent(10)
//                    .Handler(ReadFileAndCheckForTest));
//        }

//        public Task

//        private static async Task ReadFileAndCheckForTest(string filename)
//        {
//            Console.WriteLine("Reading " + filename);

//            using (var fileStream = File.OpenRead(filename))
//            {
//                using (var streamReader = new StreamReader(fileStream))
//                {
//                    var contents = await streamReader.ReadToEndAsync();

//                    if (contents.IndexOf("test", StringComparison.OrdinalIgnoreCase) != -1)
//                    {
//                        Console.WriteLine(filename + " contains the phrase 'test'");
//                    }
//                }
//            }
//        }
//    }
//}
