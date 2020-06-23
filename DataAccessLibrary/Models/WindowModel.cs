using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public class WindowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        // Helpers
        public List<WindowOptionModel> WindowOptions { get; set; }
    }
}
