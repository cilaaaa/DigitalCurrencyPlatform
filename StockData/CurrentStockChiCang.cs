
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockData
{
    public class CurrentStockChiCang
    {
        static Dictionary<string, Int32> currentStockChiCangs = new Dictionary<string, Int32>();

        
        public static void Update(string code,Int32 qty)
        {
            if (currentStockChiCangs.Keys.Contains(code))
            {
                currentStockChiCangs[code] = qty;
            }
            else
            {
                currentStockChiCangs.Add(code, qty);
            }
        }
        public static Int32 GetChiCangQty(string code)
        {
            if (currentStockChiCangs.Keys.Contains(code))
            {
                return currentStockChiCangs[code];
            }
            else
            {
                return 0;
            }
        }
    }
}
