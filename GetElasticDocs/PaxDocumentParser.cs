using System;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace ElasticScanner
{
    public class PaxDocumentParser
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private (string, string, string, string) Empty => ("", "", "", "");

        public (string, string, string, string) FindPassengerNameMismatch(logevent @event)
        {
            var content = @event.Message;
            try
            {
                if (!content.StartsWith("Output for passenger search for Givenname"))
                    return Empty;

                var surname = new Regex("Surname\\: [A-Z\\s]+\\,").Match(content);
                if (!surname.Success)
                    return Empty;

                var firstSurname = surname.Value?
                    .Replace("Surname: ", "")?
                    .TrimEnd(',')
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                    .Last()
                    .ToUpperInvariant();

                var onePassengerRecord = new Regex(@"Basic search Retrieved one passenger record for [A-Z\s]+").Match(content);
                if (!onePassengerRecord.Success)
                    return Empty;

                var secondSurname = onePassengerRecord.Value
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                    .Last()
                    ?.ToUpperInvariant();

                if (secondSurname == null)
                    return Empty;

                if (secondSurname != firstSurname)
                    return (firstSurname, secondSurname, @event.Timestamp, @event.CorrelationId);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return Empty;
        }
    }
}