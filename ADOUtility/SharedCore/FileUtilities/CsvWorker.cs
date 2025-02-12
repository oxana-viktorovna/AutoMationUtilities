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
        {
            CreateFile(filePath);
            File.WriteAllText(filePath, csv.ToString());
        }

        public void Write(IEnumerable<string> csv)
        {
            CreateFile(filePath);
            File.WriteAllLines(filePath, csv);
        }

        public List<string[]> Read()
        {
            if (!File.Exists(filePath))
                return null;

            var lines = File.ReadAllLines(filePath);
            var resultData = lines.Select(line => line.Split(Splitter)).ToList();

            return resultData;
        }

        private void CreateFile(string filePath)
        {
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }
        }
    }
}
