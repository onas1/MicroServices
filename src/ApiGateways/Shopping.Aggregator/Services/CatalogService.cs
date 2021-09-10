using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;

        public CatalogService(HttpClient client)
        {
            _client = client;
        }
        public async Task<IEnumerable<CatalogModel>> GetCatalogs()
        {
            var response = await _client.GetAsync("/api/v1/Catalog");

            //mapping response to iur catalogModel Dto with the extension method we created in Aggregator extensions.
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogsByCategory(string category)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}");

            //mapping response to iur catalogModel Dto with the extension method we created in Aggregator extensions.
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _client.GetAsync($"/api/v1/Catalog/{id}");

            //mapping response to iur catalogModel Dto with the extension method we created in Aggregator extensions.
            return await response.ReadContentAs<CatalogModel>();
        }
    }
}
