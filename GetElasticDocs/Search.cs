using System;
using System.Diagnostics;
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
        }

        public void SearchAndAnalyze(string url, string index, string phrase, Action<ISearchResponse<logevent>> documentAction, int size = 10000)
        {
            using (var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(index))
            {
                var client = new ElasticClient(settings);
                _logger.Info("Searching {0} -> {1}", url, index);
                var watch = Stopwatch.StartNew();

                var response = client.Search<logevent>(s => s
                    .From(0)
                    .Size(size)
                    .Query(q => q
                        .Bool(b => b
                            .Must(m => m.MatchPhrase(x => x.Field(f => f.Message).Query("Output for passenger search for Givenname")))
                            .Must(m => m.MatchPhrase(x => x.Field(f => f.Message).Query("one passenger record")))
                        )
                    )
                );

                watch.Stop();
                _logger.Info($"{index} got {response.Hits.Count} hits, time: {watch.Elapsed}");
                documentAction?.Invoke(response);
            }
        }
    }
}