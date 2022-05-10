using Newtonsoft.Json;
using System;

namespace ADOCore.Models
{
    public class AttachmentsInfo
    {
        public int count { get; set; }

        [JsonProperty("Value")]
        public AttachmentValue[] value { get; set; }
    }

    public class AttachmentValue
    {
        public DateTime createdDate { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public string fileName { get; set; }
        public int size { get; set; }
    }
}
