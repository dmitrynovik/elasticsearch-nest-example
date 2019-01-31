using System;
using System.IO;
using NLog;

namespace ElasticScanner
{
    internal class FileWriter
    {
        private readonly DirectoryInfo _dir;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public FileWriter(string index)
        {
            if (!Directory.Exists(index))
                Directory.CreateDirectory(index);

            _dir = new DirectoryInfo(index);
        }

        public void Write(string fileName, string content)
        {
            try
            {
                var path = Path.Combine(_dir.FullName, fileName);
                using (var stream = File.OpenWrite(path))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(content);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}