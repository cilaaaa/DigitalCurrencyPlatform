using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyFuture0522
{
    class MarkDp
    {
        public List<Signal> ListSig;

        private Dictionary<string,List<double>> getdp_add(List<double> data){
            Dictionary<string,List<double>> result = new Dictionary<string,List<double>>();
            result["data"] = new List<double>();
            int n = data.Count;
            int longest = 0;
            double biggest = 0;
            for (int x = 0;x < n-1;x++){
                double Max = data[x];
                List<double> temp = new List<double>(){ Max };
                List<double> tempseq = new List<double>(){ x };
                for(int y = x+1;y < n;y++){
                    if (Max < data[y]){
                        Max = data[y];
                        temp.Add(Max);
                        tempseq.Add(y);
                    }
                }
                if(temp.Count > longest){
                    result["data"] = temp;
                    result["seq"] = tempseq;
                    longest = temp.Count;
                    biggest = Max;
                }
                else if(temp.Count == longest){
                    if (Max >= biggest){
                        result["data"] = temp;
                        result["seq"] = tempseq;
                        longest = temp.Count;
                        biggest = Max;
                    }
                }
                    
            }
            if (result["data"].Count == 0){
                return null;
            }
            return result;
        }
        
        private Dictionary<string,List<double>> getdp_minus(List<double> data){
            Dictionary<string,List<double>> result = new Dictionary<string,List<double>>();
            int n = data.Count;
            int longest = 0;
            double biggest = 0;
            for (int x = 0;x < n-1;x++){
                double Max = data[x];
                List<double> temp = new List<double>(){ Max };
                List<double> tempseq = new List<double>(){ x };
                for(int y = x+1;y < n;y++){
                    if (Max > data[y]){
                        Max = data[y];
                        temp.Add(Max);
                        tempseq.Add(y);
                    }
                }
                if(temp.Count > longest){
                    result["data"] = temp;
                    result["seq"] = tempseq;
                    longest = temp.Count;
                    biggest = Max;
                }
                else if(temp.Count == longest){
                    if (Max <= biggest){
                        result["data"] = temp;
                        result["seq"] = tempseq;
                        longest = temp.Count;
                        biggest = Max;
                    }
                }
                    
            }
            if (result["data"].Count == 0){
                return null;
            }
            return result;
        }

        public void FindDpAdd(int xleft,Dictionary<string,List<double>> data,int quarterLen){
            Dictionary<string,List<double>> DpData = this.getdp_add(data["close"]);
            if (DpData != null){
                int SEHSEQ = DpData["seq"].Count;
                if(SEHSEQ > 0){
                    if(data.Count - 1 - SEHSEQ > quarterLen){
                        Dictionary<string,List<double>> rawData = new Dictionary<string,List<double>>();
                        foreach(var item in data){
                            rawData[item.Key] = data[item.Key].Skip(SEHSEQ).ToList();
                        }
                        FindDpAdd(xleft + SEHSEQ,rawData,quarterLen);
                    }
                    ListSig.Add(new Signal(xleft + SEHSEQ,DpData["data"][SEHSEQ - 1],"high"));
                    Dictionary<string,List<double>> rawData2 = new Dictionary<string,List<double>>();
                    foreach(var item in data){
                        rawData2[item.Key] = data[item.Key].Take(SEHSEQ).ToList();
                    }
                    double SEL = rawData2["low"].Min();
                    int SELIndex = rawData2["low"].LastIndexOf(SEL);
                    if (SEHSEQ - SELIndex > quarterLen){
                        Dictionary<string,List<double>> rawData3 = new Dictionary<string,List<double>>();
                        foreach(var item in data){
                            rawData3[item.Key] = data[item.Key].Skip(SELIndex).Take(SEHSEQ - SELIndex).ToList();
                        }
                        FindDpMinus(xleft + SELIndex,rawData3,quarterLen);
                    }
                    ListSig.Add(new Signal(xleft + SELIndex,SEL,"low"));
                    if (SELIndex > quarterLen){
                        Dictionary<string,List<double>> rawData4 = new Dictionary<string,List<double>>();
                        foreach(var item in data){
                            rawData4[item.Key] = data[item.Key].Take(SELIndex).ToList();
                        }
                        FindDpAdd(xleft,rawData4,quarterLen);
                    }
                }
            }
        }

        public void FindDpMinus(int xleft, Dictionary<string, List<double>> data, int quarterLen)
        {
            Dictionary<string, List<double>> DpData = this.getdp_minus(data["close"]);
            if (DpData != null)
            {
                int SELSEQ = DpData["seq"].Count;
                if (SELSEQ > 0)
                {
                    if (data.Count - 1 - SELSEQ > quarterLen)
                    {
                        Dictionary<string, List<double>> rawData = new Dictionary<string, List<double>>();
                        foreach (var item in data)
                        {
                            rawData[item.Key] = data[item.Key].Skip(SELSEQ).ToList();
                        }
                        FindDpMinus(xleft + SELSEQ, rawData, quarterLen);
                    }
                    ListSig.Add(new Signal(xleft + SELSEQ, DpData["data"][SELSEQ - 1], "low"));
                    Dictionary<string, List<double>> rawData2 = new Dictionary<string, List<double>>();
                    foreach (var item in data)
                    {
                        rawData2[item.Key] = data[item.Key].Take(SELSEQ).ToList();
                    }
                    double SEH = rawData2["high"].Max();
                    int SEHIndex = rawData2["high"].LastIndexOf(SEH);
                    if (SELSEQ - SEHIndex > quarterLen)
                    {
                        Dictionary<string, List<double>> rawData3 = new Dictionary<string, List<double>>();
                        foreach (var item in data)
                        {
                            rawData3[item.Key] = data[item.Key].Skip(SEHIndex).Take(SELSEQ - SEHIndex).ToList();
                        }
                        FindDpAdd(xleft + SEHIndex, rawData3, quarterLen);
                    }
                    ListSig.Add(new Signal(xleft + SEHIndex, SEH, "high"));
                    if (SEHIndex > quarterLen)
                    {
                        Dictionary<string, List<double>> rawData4 = new Dictionary<string, List<double>>();
                        foreach (var item in data)
                        {
                            rawData4[item.Key] = data[item.Key].Take(SEHIndex).ToList();
                        }
                        FindDpMinus(xleft, rawData4, quarterLen);
                    }
                }
            }
        }

        public List<Signal> GetDpData(Dictionary<string, List<double>> data)
        {
            ListSig = new List<Signal>();
            int quarterLen = (int)data["close"].Count / 4;
            double high = data["high"].Max();
            int highIndex = data["high"].LastIndexOf(high);
            double low = data["low"].Min();
            int lowIndex = data["low"].LastIndexOf(low);
            ListSig.Add(new Signal(highIndex, high, "high"));
            ListSig.Add(new Signal(lowIndex, low, "low"));
            if (highIndex > lowIndex)
            {
                Dictionary<string, List<double>> rawData = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData[item.Key] = data[item.Key].Take(lowIndex).ToList();
                }
                FindDpAdd(0, rawData, quarterLen);

                Dictionary<string, List<double>> rawData2 = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData2[item.Key] = data[item.Key].Skip(lowIndex).Take(highIndex - lowIndex).ToList();
                }
                FindDpMinus(lowIndex, rawData2, quarterLen);

                Dictionary<string, List<double>> rawData3 = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData3[item.Key] = data[item.Key].Skip(highIndex).ToList();
                }
                FindDpAdd(highIndex, rawData3, quarterLen);
            }
            else
            {
                Dictionary<string, List<double>> rawData = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData[item.Key] = data[item.Key].Take(highIndex).ToList();
                }
                FindDpAdd(0, rawData, quarterLen);

                Dictionary<string, List<double>> rawData2 = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData2[item.Key] = data[item.Key].Skip(highIndex).Take(lowIndex - highIndex).ToList();
                }
                FindDpMinus(highIndex, rawData2, quarterLen);

                Dictionary<string, List<double>> rawData3 = new Dictionary<string, List<double>>();
                foreach (var item in data)
                {
                    rawData3[item.Key] = data[item.Key].Skip(lowIndex).ToList();
                }
                FindDpAdd(lowIndex, rawData3, quarterLen);
            }
            return ListSig;
        }
    }
}
