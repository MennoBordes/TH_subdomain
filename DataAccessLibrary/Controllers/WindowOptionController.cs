using DataAccessLibrary.DataBase;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public class WindowOptionController : IWindowOptionController
    {
        private readonly IMySqlDataAccess _db;

        public WindowOptionController(IMySqlDataAccess db)
        {
            _db = db;
        }

        public Task<List<WindowOptionModel>> GetWindowOptions()
        {
            string sql = "SELECT * FROM window_options";

            return _db.LoadDataList<WindowOptionModel, dynamic>(sql, new { });
        }

        public Task<WindowOptionModel> GetWindowOption(int id)
        {
            string sql = "SELECT * FROM window_options WHERE Id = @id";

            return _db.LoadData<WindowOptionModel, dynamic>(sql, new { Id = id });

        }
    }
}
