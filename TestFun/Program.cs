using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TestFun
{
    class Program
    {
        static List<double> a = new List<double>() { 1, 2, 3, 6, 4, 5 ,7};
        static void Main(string[] args)
        {

            var test = getdp_add(a);
            Console.WriteLine(test);
        }

        private static Dictionary<string, List<double>> getdp_add(List<double> data)
        {
            Dictionary<string, List<double>> result = new Dictionary<string, List<double>>();
            int n = data.Count;
            int longest = 0;
            double biggest = 0;
            for (int x = 0; x < n - 1; x++)
            {
                double Max = data[x];
                List<double> temp = new List<double>() { Max };
                List<double> tempseq = new List<double>() { x };
                for (int y = x + 1; y < n; y++)
                {
                    if (Max < data[y])
                    {
                        Max = data[y];
                        temp.Add(Max);
                        tempseq.Add(y);
                    }
                }
                if (temp.Count > longest)
                {
                    result["data"] = temp;
                    result["seq"] = tempseq;
                    longest = temp.Count;
                    biggest = Max;
                }
                else if (temp.Count == longest)
                {
                    if (Max >= biggest)
                    {
                        result["data"] = temp;
                        result["seq"] = tempseq;
                        longest = temp.Count;
                        biggest = Max;
                    }
                }

            }
            if (result["data"].Count == 0)
            {
                return null;
            }
            return result;
        }

        private Dictionary<string, List<double>> getdp_minus(List<double> data)
        {
            Dictionary<string, List<double>> result = new Dictionary<string, List<double>>();
            int n = data.Count;
            int longest = 0;
            double biggest = 0;
            for (int x = 0; x < n - 1; x++)
            {
                double Max = data[x];
                List<double> temp = new List<double>() { Max };
                List<double> tempseq = new List<double>() { x };
                for (int y = x + 1; y < n; y++)
                {
                    if (Max > data[y])
                    {
                        Max = data[y];
                        temp.Add(Max);
                        tempseq.Add(y);
                    }
                }
                if (temp.Count > longest)
                {
                    result["data"] = temp;
                    result["seq"] = tempseq;
                    longest = temp.Count;
                    biggest = Max;
                }
                else if (temp.Count == longest)
                {
                    if (Max <= biggest)
                    {
                        result["data"] = temp;
                        result["seq"] = tempseq;
                        longest = temp.Count;
                        biggest = Max;
                    }
                }

            }
            if (result["data"].Count == 0)
            {
                return null;
            }
            return result;
        }
    }
}
