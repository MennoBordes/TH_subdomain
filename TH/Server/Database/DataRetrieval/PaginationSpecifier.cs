using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TH.Server.Database.DataRetrieval
{
    public class PaginationSpecifier
    {
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
