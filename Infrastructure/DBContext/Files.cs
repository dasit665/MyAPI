using System;
using System.Collections.Generic;

namespace MyAPI
{
    public partial class Files
    {
        public long Id { get; set; }
        public byte[] FileData { get; set; }
        public int? SystemId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? IsCompress { get; set; }
    }
}
