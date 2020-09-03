using System;
using System.Collections.Generic;
using System.Text;

namespace TH2.Shared.Modules.Order.Getter
{
    public class GetOrderDatas
    {
        public int? OrderId { get; set; } = null;
        public int[] Ids { get; set; } = null;

        public bool UseOrderId { get; set; } = true;
    }
}
