namespace Serpent.Chain.Samples.FirstSample
{
    using System.IO;
    using System.Threading.Tasks;

    using Xunit;

    public class FirstSampleTests
    {
        [Fact]
        public async Task FirstSampleTest()
        {
            var firstSample = new FirstSample();

            var files = Directory.GetFiles(@"c:\temp", "*.json", SearchOption.AllDirectories);

            await firstSample.ReadFilesAndCheckForTest(files);
        }
    }
}
