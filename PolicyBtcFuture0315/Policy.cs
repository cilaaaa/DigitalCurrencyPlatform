using DataAPI;
using DataBase;
using Newtonsoft.Json.Linq;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PolicyBtcFuture0315
{   
    /*
     * 期货趋势策略
     * 
    */
    public class Policy : StockPolicies.RunningPolicy
    {
        Parameter parameter;
        DateTime currentDay;
        List<PTradePoints> tps;
        string stockAccount;

        bool isSim;

        //static string pName;
        TickData currentTick;

        double signInHighPrice;
        double signOutHighPrice;
        double signInLowPrice;
        double signOutLowPrice;

        Decimal holdHands;
        Decimal frozenBuyHands;
        Decimal frozenSellHands;
        List<DieBar> LiveBarsIn;
        List<DieBar> LiveBarsOut;
        //累计盈亏
        double total;
        int tradeCount;
        int winCount;

        double Qty;
        
        //开仓使用的N
        double N;
        //实时计算的N
        double LastN;

        int OpenTimes;

        OpenType PreOpenType;
        bool LianXuZhiSun;
        double PreZhiYingDian;
        DateTime PreCloseTime;

        bool init = false;
        
        public static string PName
        {
            get { return "PolicyBtcFuture0315"; }
        }
        DieBar CurrentBar;

        //启动止盈
        //bool StartZhiYing;
        //最大盈利点
        //double MaxYingLi;
        //bool PanDuanPreTr;
        //double PreTr;
        //double MaxTR;
        //开仓时bar振幅
        double KaiCangZhenFu;

        bool StartTrade;
        bool LockOpenTrade = false;
        double MarkBarInHighLowPrice = 0;
        bool initOut;

        //移动N
        //int ZhiYingIndex = 0;
        //DieBar PingCangBar;
        //DieBar PingCangNextBar;
        //bool StartPingCang = false;
        //bool firstBar;
        //bool SecondBar;
        //bool otherBar;

        //振幅
        double PreClose;
        double DayHigh;
        double DayLow;
        double TongDaoValue;
        double KaiCangTongDaoValue;

        //加仓
        OpenType ot;
        double BuyPrice;
        bool lockJiaCang;
        
        //List<int> ZhiYingBeiShu;
        //List<int> HuiCeBiLi;
        


        public Policy(SecurityInfo si,Parameter rpp,PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = PName;
            this.startDate = rpp.StartDate;
            this.endDate = rpp.EndDate;
            this.inteval = rpp.Inteval;
            this.isReal = rpp.IsReal;
            this.policyguid = Guid.NewGuid();
            if (parameter.FunTongDao)
            {
                StartTrade = false;
            }
            else
            {
                StartTrade = true;
            }
            initialDataReceiver();
            InitialDataProcessor();
            currentDay = DateTime.MinValue.Date;
            openPoints = new List<OpenPoint>();
            IsSimulateFinished = false;
            //isOpened = false;
            if(rpp.save)
            {
                SaveParameter(rpp);
            }

            currentTick = new TickData();
            this.isLiveDataProcessor = true;

            signInHighPrice = 0;
            signOutHighPrice = 0;
            signInLowPrice = 1000000;
            signOutLowPrice = 1000000;
            LiveBarsIn = new List<DieBar>();
            LiveBarsOut = new List<DieBar>();
            tps = new List<PTradePoints>();
            tradeCount = 0;
            winCount = 0;
            LastN = 0;
            //MaxTR = 0;
            //PreTr = 0;
            //PanDuanPreTr = false;
            //StartZhiYing = false;
            //MaxYingLi = 0;
            initOut = false;
            
            //ZhiYingBeiShu = new List<int>(){
            //    20,30,40,50,
            //};
            //HuiCeBiLi = new List<int>(){
            //    50,40,30,20,
            //};
            if (!parameter.DebugModel)
            {
                initPDN();
            }
            
        }

        public override void InitArgs()
        {
            throw new NotImplementedException();
        }

        private void initPDN()
        {
            string start = DateTime.UtcNow.AddMinutes(-parameter.BarCountIn-1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string end = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            JArray history = null;
            double sum = 0;
            for (int i = 1; i < history.Count; i++)
            {
                double close = System.Convert.ToDouble(history[i][4].ToString());
                double high = System.Convert.ToDouble(history[i][2].ToString());
                double low = System.Convert.ToDouble(history[i][3].ToString());
                double preclose = System.Convert.ToDouble(history[i - 1][4].ToString());
                double a = Math.Max(high - low, high - preclose);
                double tr = Math.Max(a, preclose - low);
                sum += tr;
                DieBar bar = new DieBar();
                bar.High = high;
                bar.Low = low;
                bar.Close = close;
                LiveBarsIn.Add(bar);
                if (high > signInHighPrice)
                {
                    signInHighPrice = high;
                }
                if (low < signInLowPrice)
                {
                    signInLowPrice = low;
                }
                if (!parameter.FunJinChangJiSuanChuChangBar)
                {
                    if (i + parameter.BarCountOut >= history.Count)
                    {
                        LiveBarsOut.Add(bar);
                        if (high > signOutHighPrice)
                        {
                            signOutHighPrice = high;
                        }
                        if (low < signOutLowPrice)
                        {
                            signOutLowPrice = low;
                        }
                    }
                }
                
            }
            LastN = N = sum / parameter.BarCountIn;
        }


        private void SaveParameter(Parameter rpp)
        {
            rpp.Save();
        }



        void liveDataProcessor_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int inteval = ((LiveBars)sender).Inteval;
            #region 策略bar逻辑
            if (inteval == parameter.BarInteval)
            {
                DieBar bar = new DieBar();
                bar.Open = args.Bar.Open;
                bar.High = args.Bar.High;
                bar.Low = args.Bar.Low;
                bar.Close = args.Bar.Close;
                bar.BarOpenTime = args.Bar.BarOpenTime;
                bar.BarCloseTime = args.Bar.BarCloseTime;
                bar.LastTickTime = args.Bar.LastTickTime;
                bar.Finish = args.Bar.Finish;
                CurrentBar = bar;
                if (bar.Close == 0)
                {
                    return;
                }
                if (LiveBarsIn.Count < parameter.BarCountIn)
                {
                    LiveBarsIn.Add(bar);
                }
                else
                {
                    if (!init)
                    {
                        init = true;
                        foreach (DieBar tempBar in LiveBarsIn)
                        {
                            if (tempBar.High > signInHighPrice)
                            {
                                signInHighPrice = tempBar.High;
                            }
                            if (tempBar.Low < signInLowPrice)
                            {
                                signInLowPrice = tempBar.Low;
                            }
                        }
                        if (!parameter.FunJinChangJiSuanChuChangBar)
                        {
                            foreach (DieBar tempBar in LiveBarsOut)
                            {
                                if (tempBar.High > signOutHighPrice)
                                {
                                    signOutHighPrice = tempBar.High;
                                }
                                if (tempBar.Low < signOutLowPrice)
                                {
                                    signOutLowPrice = tempBar.Low;
                                }
                            }
                        }
                        
                    }
                    if (LastN == 0)
                    {
                        double sum = 0;
                        for (int i = 0; i < LiveBarsIn.Count; i++)
                        {
                            double tr = 0;
                            double high = LiveBarsIn[i].High;
                            double low = LiveBarsIn[i].Low;
                            if (i == 0)
                            {
                                tr = high - low;
                            }
                            else
                            {
                                double PreClose = LiveBarsIn[i - 1].Close;
                                double a = Math.Max(high - low, Math.Abs(high - PreClose));
                                tr = Math.Max(a, Math.Abs(PreClose - low));
                            }
                            sum += tr;
                        }
                        LastN = sum / (LiveBarsIn.Count - 1);
                    }
                    else
                    {
                        if (parameter.FunTongDao)
                        {
                            double high = signInHighPrice;
                            if (bar.High > signInHighPrice)
                            {
                                high = bar.High;
                            }
                            double low = signInLowPrice;
                            if (bar.Low < signInLowPrice)
                            {
                                low = bar.Low;
                            }
                            TongDaoValue = high - low;
                            if (TongDaoValue >= parameter.TongDao / 100 * currentTick.Last)
                            {
                                StartTrade = true;
                            }else{
                                StartTrade = false;
                            }
                        }
                        //计算是否启动止盈（反向TR策略）
                        #region 反向TR
                        //if (holdHands > 0 && StartZhiYing == false && parameter.FunFanXiangTR)
                        //{
                        //    double a = Math.Max(bar.High - bar.Low, Math.Abs(bar.High - LiveBarsIn.Last().Close));
                        //    double LastTr = Math.Max(a, Math.Abs(LiveBarsIn.Last().Close - bar.Low));
                        //    //阳线bar
                        //    if (bar.Close - bar.Open > 0 && ot == OpenType.Sell)
                        //    {
                        //        if (LastTr > 5 * LastN)
                        //        {
                        //            StartZhiYing = true;
                        //            MaxYingLi = LiveBarsIn.Last().Low;
                        //        }
                        //    }
                        //    //阴线bar
                        //    if (bar.Close - bar.Open < 0 && ot == OpenType.Buy)
                        //    {
                        //        if (LastTr > 5 * LastN)
                        //        {
                        //            StartZhiYing = true;
                        //            MaxYingLi = LiveBarsIn.Last().High;
                        //        }
                        //    }
                        //}
                        #endregion
                        
                        {
                            LockOpenTrade = false;
                            lockJiaCang = false;
                            //if (StartPingCang)
                            //{
                            //    //获取当前平仓的bar
                            //    if (firstBar)
                            //    {
                            //        PingCangBar = bar;
                            //        firstBar = false;
                            //        SecondBar = true;
                            //    }
                            //    else
                            //    {
                            //        //获取平仓后一根bar
                            //        if (SecondBar)
                            //        {
                            //            SecondBar = false;
                            //            otherBar = true;
                            //            PingCangNextBar = bar;
                            //        }
                            //    }
                            //}
                            double a2 = Math.Max(bar.High - bar.Low, Math.Abs(bar.High - LiveBarsIn.Last().Close));
                            double tr = Math.Max(a2, Math.Abs(LiveBarsIn.Last().Close - bar.Low));
                            LastN = ((parameter.BarCountIn - 1) * LastN + tr) / parameter.BarCountIn;
                            if (holdHands == 0)
                            {
                                N = LastN;
                            }
                            Console.WriteLine(string.Format("TR：{0}，止盈倍数的N：{1}", tr, parameter.ZhiYingBeiShu * N));
                            #region 正向TR-浮动止盈
                            //if (holdHands > 0 && parameter.FunZhengXiangTR)
                            //{
                            //    //做多
                            //    if (ot == OpenType.Buy)
                            //    {
                            //        //正向K线
                            //        if (bar.Close - bar.Open > 0)
                            //        {
                            //            if (!StartZhiYing)
                            //            {
                            //                if (tr > ZhiYingBeiShu[0] * N)
                            //                {
                            //                    for (int i = ZhiYingBeiShu.Count - 1; i >= 0 ; i--)
                            //                    {
                            //                        if (tr / N >= ZhiYingBeiShu[i])
                            //                        {
                            //                            ZhiYingIndex = i;
                            //                            StartZhiYing = true;
                            //                            StartPingCang = false;
                            //                            MaxTR = tr;
                            //                            MaxYingLi = bar.High - tr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //                else if (PanDuanPreTr)
                            //                {
                            //                    PanDuanPreTr = false;
                            //                    if (PreTr > ZhiYingBeiShu[0] * N)
                            //                    {
                            //                        for (int i = ZhiYingBeiShu.Count - 1; i >= 0; i--)
                            //                        {
                            //                            if (PreTr / N >= ZhiYingBeiShu[i])
                            //                            {
                            //                                ZhiYingIndex = i;
                            //                                StartZhiYing = true;
                            //                                StartPingCang = false;
                            //                                MaxTR = PreTr;
                            //                                MaxYingLi = LiveBarsIn[parameter.BarCountIn - 2].High - PreTr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                                break;
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //            else
                            //            {
                            //                if (tr > MaxTR)
                            //                {
                            //                    for (int i = ZhiYingBeiShu.Count - 1; i >= 0; i--)
                            //                    {
                            //                        if (tr / N >= ZhiYingBeiShu[i])
                            //                        {
                            //                            ZhiYingIndex = i;
                            //                            StartPingCang = false;
                            //                            firstBar = false;
                            //                            SecondBar = false;
                            //                            otherBar = false;
                            //                            MaxTR = tr;
                            //                            MaxYingLi = bar.High - tr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            if (SecondBar)
                            //            {
                            //                foreach (TradePoints tp in tps)
                            //                {
                            //                    if (!tp.Closed)
                            //                    {
                            //                        if (!currentTick.IsReal)
                            //                        {
                            //                            holdHands -= (decimal)tp.EnterPoint.OpenQty;
                            //                        }
                            //                        Leave(currentTick.Bid, tp.TradeGuid);
                            //                    }
                            //                }
                            //            }
                            //        }
                                    
                            //    }
                            //    //做空
                            //    else if (ot == OpenType.Sell)
                            //    {
                            //        //正向K线
                            //        if (bar.Close - bar.Open < 0)
                            //        {
                            //            if (!StartZhiYing)
                            //            {
                            //                if (tr > ZhiYingBeiShu[0] * N)
                            //                {
                            //                    for (int i = ZhiYingBeiShu.Count - 1; i >= 0; i--)
                            //                    {
                            //                        if (tr / N >= ZhiYingBeiShu[i])
                            //                        {
                            //                            ZhiYingIndex = i;
                            //                            StartZhiYing = true;
                            //                            StartPingCang = false;
                            //                            MaxTR = tr;
                            //                            MaxYingLi = bar.Low - tr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //                else if (PanDuanPreTr)
                            //                {
                            //                    PanDuanPreTr = false;
                            //                    if (PreTr > ZhiYingBeiShu[0] * N)
                            //                    {
                            //                        for (int i = ZhiYingBeiShu.Count - 1; i >= 0; i--)
                            //                        {
                            //                            if (PreTr / N >= ZhiYingBeiShu[i])
                            //                            {
                            //                                ZhiYingIndex = i;
                            //                                StartZhiYing = true;
                            //                                StartPingCang = false;
                            //                                MaxTR = PreTr;
                            //                                MaxYingLi = LiveBarsIn[parameter.BarCountIn - 2].Low - PreTr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                                break;
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //            }else
                            //            {
                            //                if (tr > MaxTR)
                            //                {
                            //                    for (int i = ZhiYingBeiShu.Count - 1; i >= 0; i--)
                            //                    {
                            //                        if (tr / N >= ZhiYingBeiShu[i])
                            //                        {
                            //                            ZhiYingIndex = i;
                            //                            StartPingCang = false;
                            //                            firstBar = false;
                            //                            SecondBar = false;
                            //                            otherBar = false;
                            //                            MaxTR = tr;
                            //                            MaxYingLi = bar.Low - tr * HuiCeBiLi[ZhiYingIndex] / 100;
                            //                            break;
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        }else
                            //        {
                            //            if (SecondBar)
                            //            {
                            //                foreach (TradePoints tp in tps)
                            //                {
                            //                    if (!tp.Closed)
                            //                    {
                            //                        if (!currentTick.IsReal)
                            //                        {
                            //                            holdHands -= (decimal)tp.EnterPoint.OpenQty;
                            //                        }
                            //                        Leave(currentTick.Ask, tp.TradeGuid);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                            #region 正向TR-固定止盈
                            if (holdHands > 0 && parameter.FunZhengXiangTR)
                            {
                                //做多
                                foreach (PTradePoints tp in tps)
                                {
                                    if (!tp.Finished)
                                    {
                                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                        {
                                            
                                            //正向K线
                                            if (bar.Close - bar.Open > 0 && (currentTick.Last - tp.EnterPoint.OpenPrice) > 0)
                                            {
                                                if (!tp.StartZhiYing)
                                                {
                                                    if (tr > parameter.ZhiYingBeiShu * N)
                                                    {
                                                        tp.StartZhiYing = true;
                                                        tp.MaxTr = tr;
                                                        if (parameter.FunZongYingLiZhiYing)
                                                        {
                                                            tp.ZhiYingDian = tp.EnterPoint.OpenPrice + (currentTick.Last - tp.EnterPoint.OpenPrice) * parameter.HuiCeBiLi / 100;
                                                        }
                                                        else
                                                        {
                                                            tp.ZhiYingDian = bar.High - tr * parameter.HuiCeBiLi / 100;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (tr > tp.MaxTr)
                                                    {
                                                        tp.MaxTr = tr;
                                                        if (parameter.FunZongYingLiZhiYing)
                                                        {
                                                            tp.ZhiYingDian = tp.EnterPoint.OpenPrice + (currentTick.Last - tp.EnterPoint.OpenPrice) * parameter.HuiCeBiLi / 100;
                                                        }
                                                        else
                                                        {
                                                            tp.ZhiYingDian = bar.High - tr * parameter.HuiCeBiLi / 100;
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        //做空
                                        else if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                        {
                                            //正向K线
                                            if (bar.Close - bar.Open < 0 && (tp.EnterPoint.OpenPrice - currentTick.Last) > 0)
                                            {
                                                if (!tp.StartZhiYing)
                                                {
                                                    if (tr > parameter.ZhiYingBeiShu * N)
                                                    {
                                                        tp.StartZhiYing = true;
                                                        tp.MaxTr = tr;
                                                        if (parameter.FunZongYingLiZhiYing)
                                                        {
                                                            tp.ZhiYingDian = tp.EnterPoint.OpenPrice - (tp.EnterPoint.OpenPrice - currentTick.Last) * parameter.HuiCeBiLi / 100;
                                                        }
                                                        else
                                                        {
                                                            tp.ZhiYingDian = bar.Low + tr * parameter.HuiCeBiLi / 100;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    if (tr > tp.MaxTr)
                                                    {
                                                        if (parameter.FunZongYingLiZhiYing)
                                                        {
                                                            tp.ZhiYingDian = tp.EnterPoint.OpenPrice - (tp.EnterPoint.OpenPrice - currentTick.Last) * parameter.HuiCeBiLi / 100;
                                                        }
                                                        else
                                                        {
                                                            tp.ZhiYingDian = bar.Low + tr * parameter.HuiCeBiLi / 100;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                                
                            #endregion
                            DieBar removeBar = LiveBarsIn[0];
                            LiveBarsIn.RemoveAt(0);
                            LiveBarsIn.Add(bar);
                            if (bar.High > signInHighPrice)
                            {
                                signInHighPrice = bar.High;
                            }
                            else
                            {
                                if (removeBar.High == signInHighPrice)
                                {
                                    signInHighPrice = 0;
                                    foreach (DieBar tempBar in LiveBarsIn)
                                    {
                                        if (tempBar.High > signInHighPrice)
                                        {
                                            signInHighPrice = tempBar.High;
                                        }
                                    }
                                }
                            }
                            if (bar.Low < signInLowPrice)
                            {
                                signInLowPrice = bar.Low;
                            }
                            else
                            {
                                if (removeBar.Low == signInLowPrice)
                                {
                                    signInLowPrice = 1000000;
                                    foreach (DieBar tempBar in LiveBarsIn)
                                    {
                                        if (tempBar.Low < signInLowPrice)
                                        {
                                            signInLowPrice = tempBar.Low;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #region 均线相关
                    //List<DieBar> bar20 = LiveBarsIn;
                    //double avg = bar20.Average(d => d.Close);
                    //bar.MA.MA20 = Math.Round(avg, 3, MidpointRounding.AwayFromZero);
                    //bar.MA.MA5 = LiveBarsIn.SkipWhile((n, index) => index < (parameter.BarCountIn - 5)).ToList().Average(d => d.Close);
                    //double stdsum = bar20.Sum(d => Math.Pow(d.Close - avg, 2));
                    //bar.MA.STD = Math.Sqrt(stdsum / parameter.BarCountIn);
                    //if (parameter.JunXianKaiCang)
                    //{
                    //    if (preBar != null)
                    //    {
                    //        double BarShangJie = bar.MA.MA20;
                    //        double BarXiaJie = bar.MA.MA20;
                    //        double PreBarShangJie = preBar.MA.MA20;
                    //        double PreBarXiaJie = preBar.MA.MA20;
                    //        if (parameter.UseStd)
                    //        {
                    //            BarShangJie += parameter.Stdbeishu * bar.MA.STD;
                    //            BarXiaJie -= parameter.Stdbeishu * bar.MA.STD;
                    //            PreBarShangJie += parameter.Stdbeishu * preBar.MA.STD;
                    //            PreBarXiaJie -= parameter.Stdbeishu * preBar.MA.STD;
                    //        }
                    //        if ((bar.MA.MA5 - bar.MA.MA20) * (preBar.MA.MA5 - preBar.MA.MA20) <= 0)
                    //        {
                    //            if (holdHands - frozenSellHands > 0)
                    //            {
                    //                foreach (TradePoints tp in tps)
                    //                {
                    //                    if (!tp.Closed)
                    //                    {
                    //                        if (!currentTick.IsReal)
                    //                        {
                    //                            holdHands -= (decimal)tp.EnterPoint.OpenQty;
                    //                        }
                    //                        Leave(currentTick.Last, tp.TradeGuid);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        if (bar.MA.MA5 >= BarShangJie && preBar.MA.MA5 < PreBarShangJie)
                    //        {
                    //            if ((decimal)parameter.TotalHands - (holdHands + frozenBuyHands) > (decimal)0.001)
                    //            {
                    //                Qty = (decimal)parameter.TotalHands - (holdHands + frozenBuyHands);
                    //                Qty = Math.Floor(Qty * 10000) / 10000;
                    //                if (currentTick.IsReal)
                    //                {
                    //                    frozenBuyHands += (decimal)Qty;
                    //                }
                    //                else
                    //                {
                    //                    holdHands += (decimal)Qty;
                    //                }
                    //                BuyPrice = currentTick.Last;
                    //                Open(currentTick.Last, (double)Qty, OpenType.Buy);
                    //            }
                    //        }
                    //        if (bar.MA.MA5 <= BarXiaJie && preBar.MA.MA5 > PreBarXiaJie)
                    //        {
                    //            if ((decimal)parameter.TotalHands - (holdHands + frozenBuyHands) > (decimal)0.001)
                    //            {
                    //                Qty = (decimal)parameter.TotalHands - (holdHands + frozenBuyHands);
                    //                Qty = Math.Floor(Qty * 10000) / 10000;
                    //                if (currentTick.IsReal)
                    //                {
                    //                    frozenBuyHands += (decimal)Qty;
                    //                }
                    //                else
                    //                {
                    //                    holdHands += (decimal)Qty;
                    //                }
                    //                BuyPrice = currentTick.Last;
                    //                Open(currentTick.Last, (double)Qty, OpenType.Sell);
                    //            }
                    //        }
                    //    }
                    //    preBar = bar;
                    //}
                    #endregion
                }
                if (bar.Finish)
                {
                    if ((holdHands > 0 && parameter.FunJinChangJiSuanChuChangBar) || !parameter.FunJinChangJiSuanChuChangBar)
                    {
                        if (LiveBarsOut.Count < parameter.BarCountOut)
                        {
                            if (LiveBarsOut.Count == 0 && parameter.FunJinChangJiSuanChuChangBar)
                            {
                                LiveBarsOut.Add(LiveBarsIn[parameter.BarCountIn - 2]);
                            }
                            LiveBarsOut.Add(bar);
                        }
                        else
                        {
                            if (!initOut && parameter.FunJinChangJiSuanChuChangBar)
                            {
                                initOut = true;
                                foreach (DieBar tempBar in LiveBarsOut)
                                {
                                    if (tempBar.High > signOutHighPrice)
                                    {
                                        signOutHighPrice = tempBar.High;
                                    }
                                    if (tempBar.Low < signOutLowPrice)
                                    {
                                        signOutLowPrice = tempBar.Low;
                                    }
                                }
                            }
                            DieBar removeBar = LiveBarsOut[0];
                            LiveBarsOut.RemoveAt(0);
                            LiveBarsOut.Add(bar);

                            if (bar.High > signOutHighPrice)
                            {
                                signOutHighPrice = bar.High;
                            }
                            else
                            {
                                if (removeBar.High == signOutHighPrice)
                                {
                                    signOutHighPrice = 0;
                                    foreach (DieBar tempBar in LiveBarsOut)
                                    {
                                        if (tempBar.High > signOutHighPrice)
                                        {
                                            signOutHighPrice = tempBar.High;
                                        }
                                    }
                                }
                            }
                            if (bar.Low < signOutLowPrice)
                            {
                                signOutLowPrice = bar.Low;
                            }
                            else
                            {
                                if (removeBar.Low == signOutLowPrice)
                                {
                                    signOutLowPrice = 1000000;
                                    foreach (DieBar tempBar in LiveBarsOut)
                                    {
                                        if (tempBar.Low < signOutLowPrice)
                                        {
                                            signOutLowPrice = tempBar.Low;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            #endregion
        }

        public override void Reset()
        {
            liveDataProcessor.Reset();
        }

        protected override void dataReceiver_Data_Arrival(object sender, StockData.TickData tickdata)
        {

            if (tickdata.Code == string.Empty)
            {
                IsSimulateFinished = true;
                HistoryFinished();
                LiveDataUpdate(tickdata);
            }
            else
            {
                if (currentDay != tickdata.Time.Date)
                {
                    PreClose = DayHigh = DayLow = tickdata.Last;
                    currentDay = tickdata.Time.Date;
                    this.Reset();
                }
                if (tickdata.Last == 0)
                {
                    return;
                }

                if (tickdata.Last > DayHigh)
                {
                    DayHigh = tickdata.Last;
                }
                if (tickdata.Last < DayLow)
                {
                    DayLow = tickdata.Last;
                }

                currentTick = tickdata;
                //dataAnalisys.ReceiveTick(tickdata);

                liveDataProcessor.ReceiveTick(tickdata);
                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {

                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        #region 基本海龟策略
                        //未持仓
                        if (holdHands + frozenBuyHands == 0 && signInHighPrice != 0 && StartTrade && !LockOpenTrade)
                        {

                            bool kaiDuo = false;
                            bool kaiKong = false;
                            if (parameter.LimitYinYang || parameter.LimitZhenFu)
                            {
                                double dangQianZhengZhenFu = (DayHigh - PreClose) / PreClose * 100;
                                double dangQianFuZhenFu = (PreClose - DayLow) / PreClose * 100;
                                if (parameter.LimitYinYang && parameter.LimitZhenFu)
                                {
                                    if (dangQianZhengZhenFu >= parameter.ZhenFuDaXiao || dangQianFuZhenFu >= parameter.ZhenFuDaXiao)
                                    {
                                        if (tickdata.Ask > PreClose)
                                        {
                                            kaiDuo = true;
                                        }
                                        if (tickdata.Bid < PreClose)
                                        {
                                            kaiKong = true;
                                        }
                                    }
                                }else{
                                    if (parameter.LimitYinYang)
                                    {
                                        if (tickdata.Ask > PreClose)
                                        {
                                            kaiDuo = true;
                                        }
                                        if (tickdata.Bid < PreClose)
                                        {
                                            kaiKong = true;
                                        }
                                    }
                                    if (parameter.LimitZhenFu)
                                    {
                                        if (dangQianZhengZhenFu >= parameter.ZhenFuDaXiao || dangQianFuZhenFu >= parameter.ZhenFuDaXiao)
                                        {
                                            kaiDuo = true;
                                            kaiKong = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                kaiDuo = true;
                                kaiKong = true;
                            }
                            if (parameter.ZuoDuo && kaiDuo)
                            {
                                if (tickdata.Last > signInHighPrice)
                                {
                                    ot = OpenType.KaiDuo;
                                    Qty = parameter.Qty;
                                    frozenBuyHands += (decimal)Qty;
                                    OpenTimes = 1;
                                    BuyPrice = tickdata.Bid;
                                    KaiCangTongDaoValue = TongDaoValue;
                                    MarkBarInHighLowPrice = CurrentBar.Low;
                                    lockJiaCang = true;
                                    Open(tickdata.Bid, Qty, OpenType.KaiDuo);
                                }
                            }
                            if (parameter.ZuoKong && kaiKong)
                            {
                                if (tickdata.Last < signInLowPrice)
                                {
                                    ot = OpenType.KaiKong;
                                    Qty = parameter.Qty;
                                    frozenBuyHands += (decimal)Qty;
                                    OpenTimes = 1;
                                    BuyPrice = tickdata.Ask;
                                    KaiCangTongDaoValue = TongDaoValue;
                                    MarkBarInHighLowPrice = CurrentBar.High;
                                    lockJiaCang = true;
                                    Open(tickdata.Ask, Qty, OpenType.KaiKong);
                                }
                            }
                        }
                        //持仓中
                        
                        #region 止盈
                        if (holdHands - frozenSellHands > 0)
                        {
                            if (parameter.FunZhiYing)
                            {
                                foreach (TradePoints tp in tps)
                                {
                                    if (!tp.Finished)
                                    {
                                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                        {
                                            if (tickdata.Bid - tp.EnterPoint.OpenPrice >= tp.EnterPoint.OpenPrice * parameter.ZhiYingDianShu / 100)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Ask,tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }else{
                                            if (tp.EnterPoint.OpenPrice - tickdata.Ask >= tp.EnterPoint.OpenPrice * parameter.ZhiYingDianShu / 100)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Bid, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 加仓
                        if (parameter.FunJiaCang)
                        {
                            if (OpenTimes < parameter.JiaCangCiShu && !lockJiaCang)
                            {
                                switch (parameter.JiaCangLeiXing)
                                {
                                    case JiaCangLeiXing.BarDeTR:
                                        {
                                            double a2 = Math.Max(CurrentBar.High - CurrentBar.Low, Math.Abs(CurrentBar.High - LiveBarsIn.Last().Close));
                                            double tr = Math.Max(a2, Math.Abs(LiveBarsIn.Last().Close - CurrentBar.Low));
                                            if (tr > parameter.JiaCangPriceBeiShu * N)
                                            {

                                                if (ot == OpenType.KaiDuo)
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Bid, Qty, OpenType.KaiDuo);
                                                }
                                                else
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Ask, Qty, OpenType.KaiKong);
                                                }
                                            }
                                            break;
                                        }
                                    case JiaCangLeiXing.NdeBeiShu:
                                        {
                                            if (ot == OpenType.KaiDuo)
                                            {
                                                if (tickdata.Bid > BuyPrice + parameter.JiaCangPriceBeiShu * N)
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    BuyPrice = tickdata.Bid;
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Bid, Qty, OpenType.KaiDuo);
                                                }
                                            }
                                            else
                                            {
                                                if (tickdata.Ask < BuyPrice - parameter.JiaCangPriceBeiShu * N)
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    BuyPrice = tickdata.Ask;
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Ask, Qty, OpenType.KaiKong);
                                                }
                                            }
                                            break;
                                        }
                                    case JiaCangLeiXing.TongDao:
                                        {
                                            if (ot == OpenType.KaiDuo)
                                            {
                                                if (tickdata.Bid > BuyPrice + parameter.JiaCangPriceBeiShu * KaiCangTongDaoValue)
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    BuyPrice = tickdata.Bid;
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Bid, Qty, OpenType.KaiDuo);
                                                }
                                            }
                                            else
                                            {
                                                if (tickdata.Ask < BuyPrice - parameter.JiaCangPriceBeiShu * KaiCangTongDaoValue)
                                                {
                                                    if (tickdata.IsReal)
                                                    {
                                                        frozenBuyHands += (decimal)Qty;
                                                    }
                                                    else
                                                    {
                                                        holdHands += (decimal)Qty;
                                                    }
                                                    BuyPrice = tickdata.Ask;
                                                    OpenTimes += 1;
                                                    Qty = Qty * parameter.JiaCangQtyBeiShu;
                                                    Open(tickdata.Ask, Qty, OpenType.KaiKong);
                                                }
                                            }
                                            break;
                                        }
                                    default: break;
                                }
                            }
                        }
                        #endregion
                        #region 平仓
                        if (holdHands - frozenSellHands > 0)
                        {
                            if (parameter.ZuoDuo && ot == OpenType.KaiDuo)
                            {
                                foreach (TradePoints tp in tps)
                                {
                                    if (!tp.Finished)
                                    {
                                        //if (LianXuZhiSun)
                                        //{
                                        //    if (ot == PreOpenType)
                                        //    {
                                        //        if ((currentTick.Time - PreCloseTime).TotalMinutes <= 120)
                                        //        {
                                        //            if (tickdata.Ask < PreZhiYingDian)
                                        //            {
                                        //                if (!tickdata.IsReal)
                                        //                {
                                        //                    holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                        //                }
                                        //                Leave(tickdata.Bid, tp.TradeGuid);
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        if (tickdata.Ask < signOutLowPrice && signOutLowPrice != 1000000)
                                        {
                                            //if (parameter.FunXianZhiJinChangBar)
                                            //{
                                            //    if (tickdata.Ask < OpenPrice)
                                            //    {
                                            //        if (tickdata.Ask < MarkBarInHighLowPrice)
                                            //        {
                                            //            if (!tickdata.IsReal)
                                            //            {
                                            //                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                            //            }
                                            //            Leave(tickdata.Bid, tp.TradeGuid);
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        if (!tickdata.IsReal)
                                            //        {
                                            //            holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                            //        }
                                            //        Leave(tickdata.Bid, tp.TradeGuid);
                                            //    }
                                            //}
                                            //else
                                            //{
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Ask, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            //}
                                        }
                                    }
                                }
                            }
                            
                            if (parameter.ZuoKong && ot == OpenType.KaiKong)
                            {
                                foreach (TradePoints tp in tps)
                                {
                                    if (!tp.Finished)
                                    {
                                        //if (LianXuZhiSun)
                                        //{
                                        //    if (ot == PreOpenType)
                                        //    {
                                        //        if ((currentTick.Time - PreCloseTime).TotalMinutes <= 120)
                                        //        {
                                        //            if (tickdata.Bid > PreZhiYingDian)
                                        //            {
                                        //                if (!tickdata.IsReal)
                                        //                {
                                        //                    holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                        //                }
                                        //                Leave(tickdata.Ask, tp.TradeGuid);
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        if (tickdata.Bid > signOutHighPrice && signOutHighPrice != 0)
                                        {
                                            //if (parameter.FunXianZhiJinChangBar)
                                            //{
                                            //    if (tickdata.Bid > OpenPrice)
                                            //    {
                                            //        if (tickdata.Bid > MarkBarInHighLowPrice)
                                            //        {
                                            //            if (!tickdata.IsReal)
                                            //            {
                                            //                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                            //            }
                                            //            Leave(tickdata.Ask, tp.TradeGuid);
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        if (!tickdata.IsReal)
                                            //        {
                                            //            holdHands -= (decimal)tp.EnterPoint.OpenQty;
                                            //        }
                                            //        Leave(tickdata.Ask, tp.TradeGuid);
                                            //    }
                                            //}
                                            //else
                                            //{
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Bid, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 正向止盈功能
                        #region 连续bar止盈
                        //if (parameter.FunZhengXiangTR)
                        //{
                        //    if (StartZhiYing && !StartPingCang)
                        //    {
                        //        if (ot == OpenType.Buy)
                        //        {
                        //            foreach (TradePoints tp in tps)
                        //            {
                        //                if (!tp.Closed)
                        //                {
                        //                    if (tickdata.Bid < MaxYingLi)
                        //                    {
                        //                        StartPingCang = true;
                        //                        firstBar = true;
                        //                        SecondBar = false;
                        //                        otherBar = false;
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            foreach (TradePoints tp in tps)
                        //            {
                        //                if (!tp.Closed)
                        //                {
                        //                    if (tickdata.Ask > MaxYingLi)
                        //                    {
                        //                        StartPingCang = true;
                        //                        firstBar = true;
                        //                        SecondBar = false;
                        //                        otherBar = false;
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //    if (StartPingCang)
                        //    {
                        //        if (SecondBar)
                        //        {
                        //            if (ot == OpenType.Buy)
                        //            {
                        //                foreach (TradePoints tp in tps)
                        //                {
                        //                    if (!tp.Closed)
                        //                    {
                        //                        if (tickdata.Bid > PingCangBar.High)
                        //                        {
                        //                            if (!tickdata.IsReal)
                        //                            {
                        //                                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                        //                            }
                        //                            Leave(tickdata.Bid, tp.TradeGuid);
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                foreach (TradePoints tp in tps)
                        //                {
                        //                    if (!tp.Closed)
                        //                    {
                        //                        if (tickdata.Ask < PingCangBar.Low)
                        //                        {
                        //                            if (!tickdata.IsReal)
                        //                            {
                        //                                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                        //                            }
                        //                            Leave(tickdata.Ask, tp.TradeGuid);
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        if (otherBar)
                        //        {
                        //            if (ot == OpenType.Buy)
                        //            {
                        //                foreach (TradePoints tp in tps)
                        //                {
                        //                    if (!tp.Closed)
                        //                    {
                        //                        if (tickdata.Bid > PingCangBar.Close)
                        //                        {
                        //                            if (!tickdata.IsReal)
                        //                            {
                        //                                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                        //                            }
                        //                            Leave(tickdata.Bid, tp.TradeGuid);
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                foreach (TradePoints tp in tps)
                        //                {
                        //                    if (!tp.Closed)
                        //                    {
                        //                        if (tickdata.Ask < PingCangBar.Close)
                        //                        {
                        //                            if (!tickdata.IsReal)
                        //                            {
                        //                                holdHands -= (decimal)tp.EnterPoint.OpenQty;
                        //                            }
                        //                            Leave(tickdata.Ask, tp.TradeGuid);
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion
                        #region 止盈平仓
                        if (holdHands - frozenSellHands > 0)
                        {
                            if (parameter.FunZhengXiangTR)
                            {
                                if (ot == OpenType.KaiDuo)
                                {
                                    foreach (PTradePoints tp in tps)
                                    {
                                        if (!tp.Finished && tp.StartZhiYing)
                                        {
                                            if (tickdata.Bid < tp.ZhiYingDian)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Ask, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (PTradePoints tp in tps)
                                    {
                                        if (!tp.Finished && tp.StartZhiYing)
                                        {
                                            if (tickdata.Ask > tp.ZhiYingDian)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Bid, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #endregion
                        #region 反向止盈功能
                        if (parameter.FunFanXiangTR)
                        {
                            //if (StartZhiYing)
                            //{
                            //    if (ot == OpenType.Buy)
                            //    {
                            //        foreach (TradePoints tp in tps)
                            //        {
                            //            if (!tp.Closed)
                            //            {
                            //                //if (tickdata.Bid - tp.EnterPoint.OpenPrice < parameter.ZhiYingHuiCePercent / 100 * (MaxYingLi - tp.EnterPoint.OpenPrice))
                            //                {
                            //                    StartZhiYing = false;
                            //                    if (!tickdata.IsReal)
                            //                    {
                            //                        holdHands -= (decimal)tp.EnterPoint.OpenQty;
                            //                    }
                            //                    Leave(tickdata.Bid, tp.TradeGuid);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        foreach (TradePoints tp in tps)
                            //        {
                            //            if (!tp.Closed)
                            //            {
                            //                //if (tp.EnterPoint.OpenPrice - tickdata.Ask < parameter.ZhiYingHuiCePercent / 100 * (tp.EnterPoint.OpenPrice - MaxYingLi))
                            //                {
                            //                    StartZhiYing = false;
                            //                    if (!tickdata.IsReal)
                            //                    {
                            //                        holdHands -= (decimal)tp.EnterPoint.OpenQty;
                            //                    }
                            //                    Leave(tickdata.Ask, tp.TradeGuid);
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        #endregion
                        #region 被动止损
                        if (holdHands - frozenSellHands > 0)
                        {
                            if (parameter.FunBeiDongZhiSun)
                            {
                                foreach (TradePoints tp in tps)
                                {
                                    if (!tp.Finished)
                                    {
                                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                        {
                                            if (tp.EnterPoint.OpenPrice - tickdata.Bid >= tp.EnterPoint.OpenPrice * parameter.BeiDongZhiSunPercent / 100)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Ask, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }
                                        else
                                        {
                                            if (tickdata.Ask - tp.EnterPoint.OpenPrice >= tp.EnterPoint.OpenPrice * parameter.BeiDongZhiSunPercent / 100)
                                            {
                                                frozenSellHands += (decimal)tp.EnterPoint.DealQty;
                                                Leave(tickdata.Bid, tp.EnterPoint.DealQty, tp.TradeGuid);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }
                }
            }
        }

        private void Open(double price, double qty, OpenType ot)
        {
            PolicyResultEventArgs arg = new PolicyResultEventArgs();
            arg.PolicyName1 = this.policyName;
            arg.SecInfo = this.SecInfo;
            arg.IsReal = currentTick.IsReal;
            OpenPoint op = new OpenPoint();
            op.SecInfo = currentTick.SecInfo;
            op.OpenTime = currentTick.Time;
            op.OpenPrice = price;
            op.OpenType = ot;
            op.OpenQty = qty;
            op.DealQty = 0;
            op.Openop = true;
            op.FirstTradePriceType = parameter.EnterOrderPriceType;
            op.CancelLimitTime = parameter.EnterOrderWaitSecond;
            op.ReEnterPecentage = parameter.ReEnterPercent;
            //op.CancelLimitTime = 60;
            //op.ReEnterPecentage = 0.05;
            openPoints.Add(op);

            PTradePoints tp = new PTradePoints(op, 0);
            tp.Fee = parameter.Fee;
            /////////////add//////////////////////
            tp.IsReal = arg.IsReal;
            this.tps.Add(tp);
            arg.PairePoint = tp;
            arg.Tickdata = currentTick;
            RaiseResult(arg);
        }

        private void LeaveRestStatus()
        {
            if (parameter.FunXianZhiKaiCang)
            {
                LockOpenTrade = true;
            }
            //if (parameter.FunTongXiangZhiSun)
            //{
            //    LianXuZhiSun = false;
            //    if (StartZhiYing)
            //    {
            //        LianXuZhiSun = true;
            //        PreZhiYingDian = MaxYingLi;
            //        PreOpenType = ot;
            //        PreCloseTime = currentTick.Time;
            //    }
            //}
            //StartZhiYing = false;
            //StartPingCang = false;
            //firstBar = false;
            //SecondBar = false;
            //otherBar = false;
            initOut = false;
            if(parameter.FunJinChangJiSuanChuChangBar){
                LiveBarsOut.Clear();
                signOutHighPrice = 0;
                signOutLowPrice = 1000000;
            }
            
        }

        private void Leave(double closePrice,double qty, Guid tradeGuid)
        {
            if (holdHands == 0)
            {
                LeaveRestStatus();
            }

            foreach (var tp in tps)
            {
                if (tp.TradeGuid == tradeGuid)
                {
                    if (tp.OutPoint == null)
                    {
                        OpenPoint op = new OpenPoint();
                        op.SecInfo = currentTick.SecInfo;
                        //op.FirstTradePriceType = TradeSendOrderPriceType.ShiJiaWeiTuo_SH_SZ_WuDangJiChenShenChe;
                        op.FirstTradePriceType = parameter.CloseOrderPriceType;
                        op.CancelLimitTime = parameter.CloseOrderWaitSecond;
                        op.ReTradePriceType = parameter.ReCloseOrderPriceType;
                        op.ReEnterPecentage = 1;
                        op.Openop = false;
                        //op.Remark = string.Format("MA2:{0},Close{1}", args.Bar.MA.MA2, args.Bar.Close);
                        tp.OutPoint = op;
                        openPoints.Add(op);
                        tp.HowToClose = CloseType.Normal;
                        tp.Status = OpenStatus.Closed;
                    }
                    tp.OutPoint.OpenTime = currentTick.Time;
                    tp.OutPoint.OpenQty = qty;
                    if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                    {
                        tp.OutPoint.OpenType = OpenType.PingDuo;
                        tp.OutPoint.OpenPrice = closePrice;
                    }
                    else
                    {
                        tp.OutPoint.OpenType = OpenType.PingKong;
                        tp.OutPoint.OpenPrice = closePrice;
                    }
                    PolicyResultEventArgs arg = new PolicyResultEventArgs();
                    arg.Tickdata = currentTick;
                    arg.PolicyName1 = this.policyName;
                    arg.SecInfo = this.SecInfo;
                    arg.IsReal = tp.IsReal;
                    arg.PairePoint = tp;
                    arg.OpenRmks = tp.EnterPoint.Remark;

                    RaiseResult(arg);
                    break;
                }
            }
        }



        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(string.Format("{0},", parameter.BarCountIn));
            stb.Append(string.Format("{0},", parameter.BarCountOut));
            stb.Append(string.Format("{0},", parameter.TongDao));
            stb.Append(string.Format("{0},", parameter.ZhiYingBeiShu));
            stb.Append(string.Format("{0},", parameter.HuiCeBiLi));
            stb.Append(string.Format("{0},", parameter.FunTongDao.ToString()));
            stb.Append(string.Format("{0},", parameter.FunZhiYing.ToString()));
            stb.Append(string.Format("{0},", parameter.ZhiYingDianShu.ToString()));
            stb.Append(string.Format("{0},", parameter.FunBeiDongZhiSun.ToString()));
            stb.Append(string.Format("{0},", parameter.BeiDongZhiSunPercent.ToString()));
            stb.Append(string.Format("{0},", parameter.FunZongYingLiZhiYing.ToString()));
            stb.Append(string.Format("{0},", parameter.FunJinChangJiSuanChuChangBar.ToString()));
            stb.Append(string.Format("{0},", parameter.FunTongXiangZhiSun.ToString()));
            stb.Append(string.Format("{0},", parameter.FunXianZhiKaiCang.ToString()));
            stb.Append(string.Format("{0},", parameter.FunXianZhiJinChangBar.ToString()));
            stb.Append(string.Format("{0},", parameter.JiaCangLeiXing.ToString()));
            stb.Append(string.Format("{0},", parameter.JiaCangPriceBeiShu.ToString()));
            stb.Append(string.Format("{0},", parameter.BarInteval));
            stb.Append(string.Format("{0},", parameter.Qty));
            return stb.ToString();
        }

        public override void ManualOpen()
        {
            
        }

        protected override PolicyParameter getParameter()
        {
            return this.parameter;
        }

        public override void ManualClose(Guid g)
        {
            
        }

        public override void Notify(Guid tradeGuid, OpenStatus status, double dealQty = 0, double dealPrice = 0, string weituobianhao = "", string pendWeituobianhao = "")
        {
            for (int i = 0; i < tps.ToArray().Count(); i++)
            {
                var t = tps[i];
                if (t.TradeGuid == tradeGuid)
                {
                    if (status == OpenStatus.Opened)
                    {
                        decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                        t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                        t.EnterPoint.DealPrice = (dealPrice + t.EnterPoint.OpenPrice) / t.EnterPoint.DealQty;
                        holdHands += realDealQty;
                        t.Status = OpenStatus.Opened;
                        frozenBuyHands -= realDealQty;
                        RaiseMessage(new PolicyMessageEventArgs("策略已接受开仓通知"));
                    }
                    else if (status == OpenStatus.Open)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓下单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.Close)
                    {
                        OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                        opwt.Weituobianhao = weituobianhao;
                        opwt.OpenQty = dealQty;
                        t.OutPoint.OpenPointWeiTuo.Add(opwt);
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓下单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.Closed)
                    {
                        double RealTotalDeal = 0;
                        bool find = false;
                        for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                        {
                            if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                            {
                                OpenPointWeiTuo opwt = t.OutPoint.OpenPointWeiTuo[j];
                                decimal realDealQty = (decimal)dealQty + (decimal)opwt.PartDealQty - (decimal)opwt.DealQty;
                                frozenSellHands -= realDealQty;
                                holdHands -= realDealQty;
                                t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                find = true;
                            }
                            RealTotalDeal += t.OutPoint.OpenPointWeiTuo[j].DealQty;

                        }
                        if (t.EnterPoint.DealQty == RealTotalDeal)
                        {
                            t.Finished = true;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓通知-{1}-有单子：{2}", this.policyName, weituobianhao, find.ToString())));
                    }
                    else if (status == OpenStatus.OpenPending)
                    {
                        t.EnterPoint.PartDealQty = t.EnterPoint.DealQty;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收入场追单通知-HoldHands:{1}", this.policyName, holdHands)));
                    }
                    else if (status == OpenStatus.ClosePending)
                    {
                        OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                        opwt.Weituobianhao = pendWeituobianhao;
                        opwt.OpenQty = dealQty;
                        t.OutPoint.OpenPointWeiTuo.Add(opwt);
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收出场追单通知HoldHands:{1}", this.policyName, holdHands)));
                    }
                    else if (status == OpenStatus.Failed)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受失败通知", this.policyName)));
                    }
                    else if (status == OpenStatus.PartOpend)
                    {
                        if (dealQty > (t.EnterPoint.DealQty - t.EnterPoint.PartDealQty))
                        {
                            decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                            t.EnterPoint.DealQty = dealQty + t.EnterPoint.PartDealQty;
                            frozenBuyHands -= realDealQty;
                            holdHands += realDealQty;
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分入场成交通知，成交数量:{1}", this.policyName, dealQty)));
                        }
                    }
                    else if (status == OpenStatus.PartClosed)
                    {
                        for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                        {
                            if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                            {
                                OpenPointWeiTuo opwt = t.OutPoint.OpenPointWeiTuo[j];
                                if (dealQty > (opwt.DealQty - opwt.PartDealQty))
                                {
                                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分出场成交通知，成交数量:{1}", this.policyName, dealQty)));
                                    decimal realDealQty = (decimal)dealQty + (decimal)opwt.PartDealQty - (decimal)opwt.DealQty;
                                    frozenSellHands -= realDealQty;
                                    holdHands -= realDealQty;
                                    t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                }
                                break;
                            }
                        }
                    }
                    else if (status == OpenStatus.OpenCanceled)
                    {
                        frozenBuyHands -= (decimal)dealQty;
                        if (t.EnterPoint.DealQty == 0)
                        {
                            t.Finished = true;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.CloseCanceled)
                    {
                        frozenSellHands -= (decimal)dealQty;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受出场撤单通知", this.policyName)));
                    }
                }
            }
        }

        public override void InitialDataProcessor()
        {
            //this.marketTimeRange = MarketTimeRange.getTimeRange(this.SecInfo.Market1);
            List<int> intevals = new List<int>();
            if (parameter.BarInteval != 60)
            {
                intevals.Add(parameter.BarInteval);
            }
            liveDataProcessor = new LiveDataProcessor(intevals, this.SecInfo,DateTime.Now);
            liveDataProcessor.OnLiveBarArrival += liveDataProcessor_OnLiveBarArrival;
            this.isLiveDataProcessor = true;
        }
    }  
}
