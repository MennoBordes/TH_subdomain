using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TH_Blazor.Shared.Models;

namespace TH_Blazor.Client
{
    public class PeopleClient
    {
        private readonly HttpClient _httpClient;

        public PeopleClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PeopleModel>> GetPeoples()
        {
            return await _httpClient.GetFromJsonAsync<List<PeopleModel>>("people");
        }

        public async Task<int> AddPerson(PeopleModel person)
        {
            var response = await _httpClient.PostAsJsonAsync("people", person);
            response.EnsureSuccessStatusCode();
            var personId = await response.Content.ReadFromJsonAsync<int>();
            return personId;
        }
    }
}
