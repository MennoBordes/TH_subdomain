using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TH.Shared.Modules.People.Entities;

namespace TH.Client.Components.PeopleComponents
{
    public partial class PeopleComponent : ComponentBase
    {
        public List<People> Peoples;

        protected override async Task OnInitializedAsync()
        {
            Peoples = await Http.GetFromJsonAsync<List<People>>("api/People/GetPeople");
        }
    }
}
