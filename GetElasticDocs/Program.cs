using System;
using CommandLine;
using NLog;

namespace FindSinglePassengerNameMixup
{
    class Program
    {
        public class Options
        {
            [Option('u', "url", HelpText = "ElasticSearch url", Required = true)]
            public string Url { get; set; }

            [Option('i', "index", HelpText = "Index name")]
            public string Index { get; set; }

            [Option('d', "date", HelpText = "Date")]
            public string Date { get; set; }
        }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    string index;
                    if (string.IsNullOrWhiteSpace(o.Index))
                    {
                        if (string.IsNullOrWhiteSpace(o.Date) || !DateTime.TryParse(o.Date, out var date))
                        {
                            date = DateTime.Now.AddDays(-1);
                        }
                        index = $"logstash-{date.Year}.{date.Month}.{date.Day}";
                    }
                    else
                    {
                        index = o.Index;
                    }

                    var search = new Search(o.Url);
                    int total = 0;
                    try
                    {
                        search.SearchAndAnalyze(o.Url, index, "one passenger record", response =>
                        {
                            foreach (var hit in response.Hits)
                            {
                                var mismatch = new PaxDocumentParser().FindPassengerNameMismatch(hit.Source);
                                if (!string.IsNullOrEmpty(mismatch.Item2))
                                {
                                    total++;
                                    Logger.Error($"|Mismatch found: [{mismatch.Item1}, {mismatch.Item2}]|{mismatch.Item3}|{mismatch.Item4}");
                                    new FileWriter(index).Write(hit.Id, hit.Source.Message);
                                }
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }

                    Console.WriteLine("Total: {0} mismatches", total);

                    Console.WriteLine("Press any key to exit...");
                    Console.Read();
                });
        }
    }
}
