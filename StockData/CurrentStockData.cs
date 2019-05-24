
using System.Collections.Generic;
using System.Linq;

namespace StockData
{
    public class CurrentStockData
    {
        static Dictionary<string, CurrentStockData> currentStockdatas = new Dictionary<string, CurrentStockData>();
        SecurityInfo si;

        public SecurityInfo Si
        {
            get { return si; }
            set { si = value; }
        }
        private SecurityInfo sinfo;
        private TickData tick;

        public CurrentStockData(SecurityInfo sinfo, TickData tick)
        {
            // TODO: Complete member initialization
            this.sinfo = sinfo;
            this.tick = tick;
        }

        
        public static void Update(SecurityInfo sinfo, TickData tick)
        {
            if (currentStockdatas.Keys.Contains(sinfo.Key))
            {
                currentStockdatas[sinfo.Key].tick = tick;
            }
            else
            {
                currentStockdatas.Add(sinfo.Key, new CurrentStockData(sinfo, tick));
            }
        }
        public static TickData GetTick(SecurityInfo sinfo)
        {
            if (currentStockdatas.Keys.Contains(sinfo.Key))
            {
                return currentStockdatas[sinfo.Key].tick;
            }
            else
            {
                return new TickData();
            }
        }
    }
}
