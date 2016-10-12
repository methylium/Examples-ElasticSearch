using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchFilter.Models;
using ElasticSearchFilter.Models.Documents;
using ElasticSearchFilter.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Nest;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ElasticSearchFilter.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index(string query, ICollection<string> filters)
        {
            var tagsFilter = filters.Select(value =>
            {
                Func<QueryContainerDescriptor<PublicationDocument>, QueryContainer> tagFilter = filter => filter
                    .Term(term => term
                        .Field(field => field.Tags)
                        .Value(value));

                return tagFilter;
            });

            var search = new SearchDescriptor<PublicationDocument>()
                .Query(qu => qu
                    .Bool(b => b
                        .Filter(tagsFilter)
                        .Must(must => must
                            .QueryString(queryString => queryString
                                .Query(query)))))
                .Aggregations(ag => ag
                    .Terms("tags", term => term
                        .Field(field => field.Tags)));

            var result = CreateClient().Search<PublicationDocument>(search);

            return View(new PublicationsResultDTO()
            {
                Results = result.Hits
                    .Select(hit => hit.Source)
                    .ToList(),
                Aggregation = new AggregationDTO()
                {
                    Name = "tags",
                    Buckets = result.Aggs
                        .Terms("tags")
                        .Buckets
                        .Select(bucket => new BucketDTO() { Key = bucket.Key, Count = bucket.DocCount })
                        .ToList()
                }
            });
        }

        private ElasticClient CreateClient()
        {
            var node = new Uri("http://localhost:9200");
            var indexName = new IndexName() { Name = "publications" };
            var settings = new ConnectionSettings(node)
                .PrettyJson(true)
                .DefaultIndex("publications")
                .DisableDirectStreaming(true)
                .OnRequestCompleted(request =>
                {
                    if (request.RequestBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(request.RequestBodyInBytes));
                    if (request.ResponseBodyInBytes != null) Debug.WriteLine(Encoding.UTF8.GetString(request.ResponseBodyInBytes));
                });
            var client = new ElasticClient(settings);

            //client.DeleteIndex(Indices.Index(indexName));
            client.CreateIndex(indexName, index => index
                    .Mappings(mappings => mappings
                        .Map<PublicationDocument>(mapping => mapping.AutoMap())));

            client.Index<PublicationDocument>(new PublicationDocument()
            {
                Title = "Test 1",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Si enim ad populum me vocas, eum. Aut haec tibi, Torquate, sunt vituperanda aut patrocinium voluptatis repudiandum.",
                Tags = new List<string>() { "Lorum", "Ipsum", "Quae" }
            });
            client.Index<PublicationDocument>(new PublicationDocument()
            {
                Title = "Test 2",
                Content = "Et quod est munus, quod opus sapientiae? Quae similitudo in genere etiam humano apparet.",
                Tags = new List<string>() { "Lorum", "Ipsum", "Munus" }
            });
            client.Index<PublicationDocument>(new PublicationDocument()
            {
                Title = "Test 3",
                Content = "Quae similitudo in genere etiam humano apparet. Parvi enim primo ortu sic iacent, tamquam omnino sine animo sint.",
                Tags = new List<string>() { "Lorum", "Vocas", "Similitudo" }
            });
            client.Index<PublicationDocument>(new PublicationDocument()
            {
                Title = "Test 4",
                Content = "Quamquam in hac divisione rem ipsam prorsus probo, elegantiam desidero.",
                Tags = new List<string>() { "Lorum", "Quae", "Similitudo" }
            });

            return client;
        }
    }
}
