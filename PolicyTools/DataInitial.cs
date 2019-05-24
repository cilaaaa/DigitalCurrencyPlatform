using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyTools
{
    public class DataInitial
    {
        public static double[] ZScore(double[] cacluateArray)
        {
            double[] result = new double[cacluateArray.Length];
            double average = 0;
            double bigSum = 0;
            double stdDev = 0;
            average = result.Sum() / result.Length;
            for (int i = 0; i < cacluateArray.Length; i++)
            {
                bigSum += Math.Pow(cacluateArray[i] - average, 2);
            }
            stdDev = Math.Sqrt(bigSum / (cacluateArray.Length - 1));

            for (int i = 0; i < result.Length; i++)
            {
                //result[i] = Math.Round((cacluateArray[i] - average) / stdDev, 10);
                result[i] = (cacluateArray[i] - average) / stdDev;
            }
            return result;
        }

        public static double[] DividFirst(double[] cacluateArray)
        {
            double firsttest = cacluateArray[0];
            for (int i = 0; i < cacluateArray.Length; i++)
            {
                cacluateArray[i] = cacluateArray[i] / firsttest;
                //cacluateArray[i] = Math.Round(cacluateArray[i] / firsttest, 10);
            }
            return cacluateArray;
        }
    }
}
