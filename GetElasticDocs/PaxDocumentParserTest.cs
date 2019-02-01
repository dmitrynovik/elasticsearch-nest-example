using System.IO;
using FluentAssertions;
using Xunit;

namespace ElasticScanner
{
    public class PaxDocumentParserTest
    {
        [Fact]
        public void Must_Return_True_On_Names_Mixup()
        {
            var parser = new PaxDocumentParser();
            using (var stream = File.OpenText("mixed-names-message.txt"))
            {
                var text = stream.ReadToEnd();
                parser.FindPassengerNameMismatch(text).Item2.Should().Be("MGEE");
            }
        }
    }
}
