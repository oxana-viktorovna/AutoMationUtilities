using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharedCore.FileUtilities
{
    public class CsvWorker
    {
        public CsvWorker(string filePath, string splitter = ",")
        {
            this.filePath = filePath;
            Splitter = splitter;
        }

        private readonly string filePath;
        public string Splitter { get; private set; }

        public void Write(StringBuilder csv)
            => File.WriteAllText(filePath, csv.ToString());

        public void Write(IEnumerable<string> csv)
            => File.WriteAllLines(filePath, csv);

        public List<string[]> Read()
        {
            var lines = File.ReadAllLines(filePath);
            var resultData = lines.Select(line => line.Split(Splitter)).ToList();

            return resultData;
        }
    }
}
