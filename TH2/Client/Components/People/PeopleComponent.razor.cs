using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TH2.Client.Components.People
{
    using System.Net.Http.Json;
    using TH2.Shared.Modules.People.Entities;
    public partial class PeopleComponent : ComponentBase
    {
        public List<People> Peoples;

        protected override async Task OnInitializedAsync()
        {
            Peoples = await Http.GetFromJsonAsync<List<People>>("api/People/GetPeople");
        }
    }
}
