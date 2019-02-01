using System;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace ElasticScanner
{
    public class PaxDocumentParser
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public (string, string) FindPassengerNameMismatch(string content)
        {
            try
            {
                if (!content.StartsWith("Output for passenger search for Givenname"))
                    return ("", "");

                var surname = new Regex("Surname\\: [A-Z\\s]+\\,").Match(content);
                if (!surname.Success)
                    return ("", "");

                var firstSurname = surname.Value?
                    .Replace("Surname: ", "")?
                    .TrimEnd(',')
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                    .Last()
                    .ToUpperInvariant();

                var onePassengerRecord = new Regex(@"Basic search Retrieved one passenger record for [A-Z\s]+").Match(content);
                if (!onePassengerRecord.Success)
                    return ("", "");

                var secondSurname = onePassengerRecord.Value
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                    .Last()
                    ?.ToUpperInvariant();

                if (secondSurname == null)
                    return ("", "");

                if (secondSurname != firstSurname)
                    return (firstSurname, secondSurname);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            return ("", "");
        }
    }
}