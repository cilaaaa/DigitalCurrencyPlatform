using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyTools
{
    public class DataCompare
    {

        public static double DirectDistence(double[] _sample, double[] _test)
        {
            try
            {
                double distence = 0;
                for (int i = 0; i < _sample.Length; i++)
                {
                    distence += Math.Abs(_sample[i] - _test[i]);
                }
                return distence / _sample.Length;
            }
            catch
            {
                return double.MaxValue;
            }
        }
        public static double OuDistence(double[] _sample, double[] _test)
        {
            try
            {
                double distence = 0;
                for (int i = 0; i < _sample.Length; i++)
                {
                    distence += (_sample[i] - _test[i]) * (_sample[i] - _test[i]);
                }
                return Math.Sqrt(distence);
            }
            catch
            {
                return double.MaxValue;
            }
        }
        public static double DTWUseInteger(double[] _sample, double[] _test)
        {

            uint seed = 50000;
            uint[] sample = new uint[_sample.Length];
            uint[] test = new uint[_test.Length];
            for (int i = 0; i < _sample.Length; i++)
            {
                sample[i] = System.Convert.ToUInt32(_sample[i] * seed);
            }
            for (int i = 0; i < _test.Length; i++)
            {
                test[i] = System.Convert.ToUInt32(_test[i] * seed);
            }
            uint _dtw;
            //int[,] distence = new int[sample.Length, test.Length];
            uint[,] dtw = new uint[sample.Length, test.Length];

            for (int i = 0; i < _sample.Length; i++)
            {
                for (int j = 0; j < _test.Length; j++)
                {
                    uint dis = (sample[i] - test[j]) * (sample[i] - test[j]);

                    //distence[i, j] = dis;

                    if (i == 0 && j == 0)
                    {
                        _dtw = dis;
                    }
                    else if (i == 0)
                    {
                        _dtw = dis + dtw[0, j - 1];
                    }
                    else if (j == 0)
                    {
                        _dtw = dis + dtw[i - 1, 0];
                    }
                    else
                    {
                        _dtw = Math.Min(Math.Min(dtw[i - 1, j], dtw[i - 1, j - 1]), dtw[i, j - 1]) + dis;
                    }
                    dtw[i, j] = _dtw;
                    //if (_maxdtw <= _dtw)
                    //{
                    //    _maxdtw = _dtw;
                    //}
                }
            }
            return System.Convert.ToDouble(dtw[sample.Length - 1, test.Length - 1]) / seed / seed;
            //int seed = 100000000;
            //int[] sample = new int[_sample.Length];
            //int[] test = new int[_test.Length];
            //for (int i = 0; i < _sample.Length; i++)
            //{
            //    sample[i] = System.Convert.ToInt32(_sample[i] * seed);
            //}
            //for (int i = 0; i < _test.Length; i++)
            //{
            //    test[i] = System.Convert.ToInt32(_test[i] * seed);
            //}
            //int _dtw;
            //int[,] distence = new int[sample.Length, test.Length];
            //int[,] dtw = new int[sample.Length, test.Length];

            //for (int i = 0; i < _sample.Length; i++)
            //{
            //    for (int j = 0; j < _test.Length; j++)
            //    {
            //        int dis = Math.Abs(sample[i] - test[j]);

            //        distence[i, j] = dis;

            //        if (i == 0 && j == 0)
            //        {
            //            _dtw = dis * 2;
            //        }
            //        else if (i == 0)
            //        {
            //            _dtw = dis + dtw[0, j - 1];
            //        }
            //        else if (j == 0)
            //        {
            //            _dtw = dis + dtw[i - 1, 0];
            //        }
            //        else
            //        {
            //            _dtw = Math.Min(Math.Min(dtw[i - 1, j] + dis, dtw[i - 1, j - 1] + 2 * dis), dtw[i, j - 1] + dis);
            //        }
            //        dtw[i, j] = _dtw;
            //        //if (_maxdtw <= _dtw)
            //        //{
            //        //    _maxdtw = _dtw;
            //        //}
            //    }
            //}
            //return System.Convert.ToDouble(dtw[sample.Length - 1, test.Length - 1]) / seed;

        }
        public static double DTW(double[] _sample, double[] _test)
        {
            double[,] distence = new double[_sample.Length, _test.Length];
            double[,] dtw = new double[_sample.Length, _test.Length];
            double _dtw;
            for (int i = 0; i < _sample.Length; i++)
            {
                for (int j = 0; j < _test.Length; j++)
                {
                    double dis = Math.Abs(_sample[i] - _test[j]);

                    distence[i, j] = dis;

                    if (i == 0 && j == 0)
                    {
                        _dtw = dis * 2;
                    }
                    else if (i == 0)
                    {
                        _dtw = dis + dtw[0, j - 1];
                    }
                    else if (j == 0)
                    {
                        _dtw = dis + dtw[i - 1, 0];
                    }
                    else
                    {
                        _dtw = Math.Min(Math.Min(dtw[i - 1, j] + dis, dtw[i - 1, j - 1] + 2 * dis), dtw[i, j - 1] + dis);
                    }
                    dtw[i, j] = _dtw;
                    //if (_maxdtw <= _dtw)
                    //{
                    //    _maxdtw = _dtw;
                    //}
                }
            }
            //return _maxdtw;
            return dtw[_sample.Length - 1, _test.Length - 1];
        }
        public static double DTWH(double[] _sample, double[] _test)
        {


            //double[,] distence = new double[_sample.Length, _test.Length];
            double[,] dtw = new double[_sample.Length, _test.Length];
            double _dtw;
            int _il = _sample.Length;
            int _jl = _test.Length;

            double gap;
            gap = _sample[0] - _test[0];
            gap = gap * gap;
            //gap = Math.Pow(gap, 2);// * gap;

            dtw[0, 0] = gap;

            for (int i = 1; i < _il; i++)
            {
                gap = _sample[i] - _test[0];
                gap = gap * gap;
                dtw[i, 0] = gap + dtw[i - 1, 0];
            }
            for (int j = 1; j < _jl; j++)
            {
                gap = _sample[0] - _test[j];
                gap = gap * gap;
                dtw[0, j] = gap + dtw[0, j - 1];
            }

            for (int i = 1; i < _il; i++)
            {
                for (int j = 1; j < _jl; j++)
                {
                    gap = _sample[i] - _test[j];// * (_sample[i] - _test[j]);
                    gap = gap * gap;

                    //distence[i, j] = dis;

                    //if (i == 0 && j == 0)
                    //{
                    //    _dtw = dis;

                    //}
                    //else if (i == 0)
                    //{
                    //    _dtw = dis + dtw[0, j - 1];

                    //}
                    //else if (j == 0)
                    //{
                    //    _dtw = dis + dtw[i - 1, 0];

                    //}
                    //else
                    //{
                    _dtw = Math.Min(Math.Min(dtw[i - 1, j], dtw[i - 1, j - 1]), dtw[i, j - 1]) + gap;

                    //}
                    dtw[i, j] = _dtw;
                }
            }

            return dtw[_sample.Length - 1, _test.Length - 1];

            //double[,] distence = new double[_sample.Length, _test.Length];
            //double[,] dtw = new double[_sample.Length, _test.Length];
            //double _dtw;
            //for (int i = 0; i < _sample.Length; i++)
            //{
            //    for (int j = 0; j < _test.Length; j++)
            //    {
            //        double dis = Math.Abs(_sample[i] - _test[j]);

            //        distence[i, j] = dis;

            //        if (i == 0 && j == 0)
            //        {
            //            _dtw = dis * 2;
            //        }
            //        else if (i == 0)
            //        {
            //            _dtw = dis + dtw[0, j - 1];
            //        }
            //        else if (j == 0)
            //        {
            //            _dtw = dis + dtw[i - 1, 0];
            //        }
            //        else
            //        {
            //            _dtw = Math.Min(Math.Min(dtw[i - 1, j] + dis, dtw[i - 1, j - 1] + 2 * dis), dtw[i, j - 1] + dis);
            //        }
            //        dtw[i, j] = _dtw;
            //        //if (_maxdtw <= _dtw)
            //        //{
            //        //    _maxdtw = _dtw;
            //        //}
            //    }
            //}
            ////return _maxdtw;
            //return dtw[_sample.Length - 1, _test.Length - 1];
        }
        public static double DTWX(double[] _sample, double[] _test)
        {
            double[,] arr1 = new double[_sample.Length, _test.Length];
            double[,] arr2 = new double[_sample.Length, _test.Length];
            double[,] ret = new double[_sample.Length, _test.Length];

            int len = _sample.Length;

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    arr1[i, j] = _sample[i];
                    arr2[i, j] = _test[j];
                }
            }


            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                    ret[i, j] = (arr1[i, j] - arr2[i, j]) * (arr1[i, j] - arr2[i, j]);



            for (int i = 1; i < len; i++)
            {
                ret[0, i] = ret[0, i] + ret[0, i - 1];
                ret[i, 0] = ret[i, 0] + ret[i - 1, 0];
            }

            for (int i = 1; i < len; i++)
            {
                for (int j = 1; j < len; j++)
                {
                    double min = ret[i - 1, j];
                    if (min > ret[i - 1, j - 1])
                        min = ret[i - 1, j - 1];
                    if (min > ret[i, j - 1])
                        min = ret[i, j - 1];

                    ret[i, j] = ret[i, j] + min;
                }
            }

            return ret[len - 1, len - 1];

        }
        public static double Calculate(CompareMethod compareMethod, double[] sample, double[] test)
        {
            if (compareMethod == CompareMethod.DirectDistence)
            {
                return DirectDistence(sample, test);
            }
            else if (compareMethod == CompareMethod.DTW)
            {
                return DTW(sample, test);
            }
            else if (compareMethod == CompareMethod.DTWH)
            {
                return DTWH(sample, test);
            }
            else
            {
                return OuDistence(sample, test);
            }

        }
    }
}
