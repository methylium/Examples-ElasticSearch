using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchFilter.Models.DTO
{
    public class AggregationDTO
    {
        public string Name { get; set; }
        public List<BucketDTO> Buckets { get; set; }
    }
}
