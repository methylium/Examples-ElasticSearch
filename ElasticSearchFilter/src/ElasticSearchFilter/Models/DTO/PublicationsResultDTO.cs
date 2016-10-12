using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchFilter.Models.Documents;

namespace ElasticSearchFilter.Models.DTO
{
    public class PublicationsResultDTO
    {
        public List<PublicationDocument> Results { get; set; }
        public AggregationDTO Aggregation { get; set; }
    }
}
