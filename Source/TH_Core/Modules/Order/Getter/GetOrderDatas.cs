using System;
using System.Collections.Generic;
using System.Text;

namespace TH.Core.Modules.Order.Getter
{
    public class GetOrderDatas
    {
        public int? OrderId { get; set; } = null;
        public int[] Ids { get; set; } = null;

        public bool UseOrderId { get; set; } = true;
    }
}
