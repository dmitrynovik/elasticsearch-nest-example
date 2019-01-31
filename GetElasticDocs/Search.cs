using System;
using System.Collections.Generic;
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

            for (int i = 27; i <= 27; i++)
            {
                try
                {
                    SearchAndAnalyze(url, $"logstash-2019.01.{i:D2}");
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }

        private void SearchAndAnalyze(string url, string index)
        {
            using (var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(index))
            {
                var client = new ElasticClient(settings);

                _logger.Info("Searching {0} -> {1}", url, index);

                //var mustClauses = new List<QueryContainer>
                //{
                //    new FuzzyQuery
                //    {
                //        Field = new Field("Message"), Value = "Output for passenger search for Givenname*"
                //    }
                //};

                //var searchRequest = new SearchRequest<logevent>(index)
                //{
                //    Size = 3000,
                //    From = 0,
                //    Query = new BoolQuery { Must = mustClauses }
                //};

                //var response = client.Search<logevent>(searchRequest);
                //.Query("one passenger record")

                //var response = client.Search<logevent>(s => s
                //    .From(0)
                //    .Size(10000)
                //    .Query(q => q
                //        .Match(m => m
                //            .Field(f => f.Message)
                //            .Query("one passenger record")
                //        )
                //    )
                //);

                //var response = client.Search<logevent>(s => s
                //    .From(0)
                //    .Size(10000)
                //    .Query(q => q
                //        .Regexp(m => m
                //            .Field(f => f.Message)
                //            .Value("one passenger record")
                //        )
                //    )
                //);

                //var response = client.Search<logevent>(s => s
                //    .From(0)
                //    .Size(10)
                //    .Query(q => q
                //        .Regexp(m => m
                //            .Name("output")
                //            .Field(f => f.Message)
                //            .Value("^Output")
                //        )
                //    )
                //);

                //var docs = client.Search<logevent>(b => b
                //    .Query(q => q
                //        .Regexp(fq => fq

                //        )
                //    )
                //);

                //var response = client.Search<logevent>(s => s
                //    .From(0)
                //    .Size(10000)
                //    .Query(q => q
                //        .Bool(b => b
                //            .Must(m => m.MatchPhrase(x => x.Field(f => f.Message).Query("Output for passenger search for Givenname")))
                //            .Must(m => m.MatchPhrase(x => x.Field(f => f.Message).Query("one passenger record")))
                //        )
                //    )
                //);

                var response = client.Search<logevent>(s => s
                    .From(0)
                    .Size(10000)
                    .Query(q => q
                        .MatchPhrase(m => m
                            .Field(f => f.Message)
                            .Query("one passenger record")
                        )
                    )
                );

                _logger.Info($"{index} got {response.Hits.Count} hits");
                foreach (var hit in response.Hits)
                {
                    var content = hit.Source.Message;
                    var mismatch = new DocumentParser().Parse(content);
                    if (!string.IsNullOrEmpty(mismatch.Item2))
                    {
                        _logger.Error($"Mismatch found: [{mismatch.Item1}, {mismatch.Item2}]");
                        new FileWriter(index).Write(hit.Id, content);
                    }
                }
            }
        }
    }
}