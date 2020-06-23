using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public interface IKozijnController
    {
        Task<KozijnKleurModel> GetKozijnKleurModel(int id);
        Task<List<KozijnKleurModel>> GetKozijnKleurModels();
        Task<List<KozijnStappenModel>> GetKozijnStappenModels(KozijnStappenModel.Types type);
    }
}