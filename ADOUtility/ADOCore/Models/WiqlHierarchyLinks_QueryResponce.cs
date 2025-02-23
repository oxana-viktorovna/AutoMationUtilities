using System;

namespace ADOCore.Models
{

    public class WiqlHierarchyLinks_QueryResponce
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }
        public ColumnHierarchyLinks[] columns { get; set; }
        public Sortcolumn[] sortColumns { get; set; }
        public Workitemrelation[] workItemRelations { get; set; }
    }

    public class ColumnHierarchyLinks
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Sortcolumn
    {
        public Field field { get; set; }
        public bool descending { get; set; }
    }

    public class Field
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Workitemrelation
    {
        public string rel { get; set; }
        public Source source { get; set; }
        public Target target { get; set; }
    }

    public class Source
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public class Target
    {
        public int id { get; set; }
        public string url { get; set; }
    }

}
