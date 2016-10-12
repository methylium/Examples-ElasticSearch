using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;

namespace ElasticSearchFilter.Models.Documents
{
    public class PublicationDocument
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
    }
}
