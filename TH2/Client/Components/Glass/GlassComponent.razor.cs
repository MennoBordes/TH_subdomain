using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace TH2.Client.Components.Glass
{
    using Newtonsoft.Json.Linq;
    using TH2.Shared.Modules.Glass.Entities;

    public partial class GlassComponent : ComponentBase
    {
        public Glass glass;

        protected override async Task OnInitializedAsync()
        {
            JObject json = new JObject();
            json["id"] = 1;
            //glass = await Http.GetFromJsonAsync<Glass>("api/Glass/Getglass", json);
            var res = await Http.PostAsJsonAsync<Glass>("api/Glass/GetGlass", new Glass() { Id = 1 });
            
        }
    }
}
