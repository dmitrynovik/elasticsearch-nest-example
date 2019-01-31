using System;
using Nest;
using NLog;

namespace ElasticScanner
{
    public class Search
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Search(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            _logger.Info("Connecting to {0}", url);

            for (int i = 1; i <= 31; i++)
            {
                SearchIndex(url, $"logstash-2019.01.{i:D2}");
                break;
            }
        }

        private void SearchIndex(string url, string index)
        {
            using (var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(index))
            {
                var client = new ElasticClient(settings);

                _logger.Info("Searching {0} -> {1}", url, index);

                var response = client.Search<logevent>(s => s
                    .From(0)
                    //.Size(10)
                    .Query(q => q
                        .Match(m => m
                            .Field(f => f.Message)
                            .Query("Output for passenger search for Givenname")
                        )
                    )
                );

                _logger.Info($"Hits: {response.Hits.Count}");
            }
        }
    }
}