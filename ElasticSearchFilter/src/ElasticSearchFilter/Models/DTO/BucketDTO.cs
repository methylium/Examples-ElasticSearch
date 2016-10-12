using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchFilter.Models.DTO
{
    public class BucketDTO
    {
        public string Key { get; set; }
        public long? Count { get; set; }
    }
}
