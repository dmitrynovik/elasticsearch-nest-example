using System;
using System.Linq;

namespace ElasticScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string elasticSearchUrl = args != null && args.Any() ?
                args[0] :
                @"https://search-qfkioskprod63-apcl5ttqomjebula455bb5ia5a.ap-southeast-2.es.amazonaws.com";

            var search = new Search(elasticSearchUrl);

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
