using DataAccessLibrary.DataBase;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Controllers
{
    public class KozijnController : IKozijnController
    {
        private readonly IMySqlDataAccess _db;

        public KozijnController(IMySqlDataAccess db)
        {
            _db = db;
        }


        // === Kozijn stappen
        public Task<List<KozijnStappenModel>> GetKozijnStappenModels(KozijnStappenModel.Types type)
        {
            string sql = "SELECT * FROM kozijn_stappen WHERE Type = @Type ORDER BY Id;";

            return _db.LoadDataList<KozijnStappenModel, dynamic>(sql, new { Type = (int)type });
        }

        // === Kozijn kleur
        public Task<List<KozijnKleurModel>> GetKozijnKleurModels()
        {
            string sql = "SELECT * FROM kozijn_kleur ORDER BY Id;";

            return _db.LoadDataList<KozijnKleurModel, dynamic>(sql, new { });
        }

        public Task<KozijnKleurModel> GetKozijnKleurModel(int id)
        {
            string sql = "SELECT * FROM kozijn_kleur WHERE Id = @Id;";

            return _db.LoadData<KozijnKleurModel, dynamic>(sql, new { Id = id });
        }
    }
}
