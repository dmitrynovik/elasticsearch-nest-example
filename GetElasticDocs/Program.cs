using System;
using System.Linq;
using NLog;

namespace ElasticScanner
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            string url = args != null && args.Any() ? args[0] : null;
            if (url == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please supply ElasticSearch URI command line argument.");
            }
            else
            {
                var search = new Search(url);
                for (int i = 27; i <= 27; i++)
                {
                    var index = $"logstash-2019.01.{i:D2}";
                    try
                    {
                        search.SearchAndAnalyze(url, index, "one passenger record", response =>
                        {
                            foreach (var hit in response.Hits)
                            {
                                var content = hit.Source.Message;
                                var mismatch = new PaxDocumentParser().FindPassengerNameMismatch(content);
                                if (!string.IsNullOrEmpty(mismatch.Item2))
                                {
                                    Logger.Error($"Mismatch found: [{mismatch.Item1}, {mismatch.Item2}]");
                                    new FileWriter(index).Write(hit.Id, content);
                                }
                            }

                        });
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }

            }

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
