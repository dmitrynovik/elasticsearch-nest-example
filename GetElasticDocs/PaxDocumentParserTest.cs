using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace FindSinglePassengerNameMixup
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
                var @event = new logevent { Timestamp = DateTime.Now.ToString(), CorrelationId = Guid.Empty.ToString(), Message = text };
                parser.FindPassengerNameMismatch(@event).Item2.Should().Be("MGEE");
            }
        }
    }
}
