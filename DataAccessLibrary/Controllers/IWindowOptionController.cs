using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public interface IWindowOptionController
    {
        Task<WindowOptionModel> GetWindowOption(int id);
        Task<List<WindowOptionModel>> GetWindowOptions();
    }
}