using System;
using System.Collections.Generic;

namespace StockTrade
{
    public class TradeHistory
    {
        //private static frm_TradeMonitor tradeForm;

        //public static frm_TradeMonitor TradeForm

        public delegate void TradeDetailUpdate(object sender, TradeDetailUpdateEventArgs args);
        public event TradeDetailUpdate TradeDetail_Update;

        private void RaiseTradeDetailUpdate(TradeDetailUpdateEventArgs args)
        {
            if (TradeDetail_Update != null)
            {
                TradeDetail_Update(this, args);
            }
        }

        public List<TradeDetail> tradeDetail;

        public List<string> List_ChenJiaoBianHao;


        public TradeHistory()
        {
            this.tradeDetail = new List<TradeDetail>();
            this.List_ChenJiaoBianHao = new List<string>();
        }


        public void Add(TradeDetail td)
        {
            tradeDetail.Add(td);
            //RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(td));
        }

        public double getDealQty(Guid guid)
        {
            for (int i = tradeDetail.Count - 1; i >= 0; i--)
            {
                if (tradeDetail[i].Tradeid == guid)
                {
                    return tradeDetail[i].TradeOHQty;
                }
            }
            return 0;
        }

        public bool getTradeDetail(Guid guid, ref TradeDetail td)
        {
            for (int i = tradeDetail.Count - 1; i >= 0 ; i--)
            {
                if (tradeDetail[i].Tradeid == guid)
                {
                    td = tradeDetail[i];
                    return true;
                }
            }
            return false;
        }

        public void AddOpenWeiTuo(TradeWeiTuo tradeWeiTuo, Guid guid)
        {
            for (int i = tradeDetail.Count - 1; i >= 0; i--)
            {
                if (tradeDetail[i].Tradeid == guid)
                {
                    tradeDetail[i].OpenWeiTuo.Add(tradeWeiTuo);
                    //RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(tradeDetail[i]));
                    return;
                }
            }
        }



        public void AddCloseWeiTuo(TradeWeiTuo tradeWeiTuo, Guid guid)
        {
            for (int i = tradeDetail.Count - 1; i >= 0 ; i--)
            {
                if (tradeDetail[i].Tradeid == guid)
                {
                    tradeDetail[i].CloseWeiTuo.Add(tradeWeiTuo);
                    return;
                }
            }
        }

        public bool UpdateChenJiao(TradeChenJiao tcj)
        {
            for (int i = tradeDetail.Count - 1; i >= 0 ; i--)
            {
                for (int m = 0; m < tradeDetail[i].OpenWeiTuo.Count; m++)
                {
                    if (tradeDetail[i].OpenWeiTuo[m].ClientBianHao == tcj.ClientBianhao)
                    {
                        tradeDetail[i].OpenWeiTuo[m].ChenJiao.Add(tcj);
                        RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(tradeDetail[i]));
                        return true;
                    }
                }
                for (int m = 0; m < tradeDetail[i].CloseWeiTuo.Count; m++)
                {
                    if (tradeDetail[i].CloseWeiTuo[m].ClientBianHao == tcj.ClientBianhao)
                    {
                        tradeDetail[i].CloseWeiTuo[m].ChenJiao.Add(tcj);
                        RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(tradeDetail[i]));
                        return true;
                    }
                }
            }
            return false;
        }




        //args.Wt.Time,args.Wt.WTnbr,args.Wt.Price_deal,args.Wt.Qty_deal,args.Wt.Name
        public void UpdateMonitorWeiTuo(string weituotime, string clientbianhao, double dealPrice, double dealQty,double fee, string codename,bool done)
        {
            for (int i = tradeDetail.Count - 1; i >= 0 ; i--)
            {
                for (int m = 0; m < tradeDetail[i].OpenWeiTuo.Count; m++)
                {
                    if (tradeDetail[i].OpenWeiTuo[m].ClientBianHao == clientbianhao)
                    {
                        tradeDetail[i].OpenWeiTuo[m].Time = weituotime;
                        tradeDetail[i].TradeSi.Name = codename;
                        tradeDetail[i].OpenWeiTuo[m].WeiTuoDealPrice = dealPrice;
                        tradeDetail[i].OpenWeiTuo[m].WeiTuoDealFee = fee;

                        tradeDetail[i].OpenWeiTuo[m].WeiTuoDealQty = dealQty;
                        tradeDetail[i].OpenWeiTuo[m].Done = done;
                        RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(tradeDetail[i]));
                        return;

                    }
                }
                for (int m = 0; m < tradeDetail[i].CloseWeiTuo.Count; m++)
                {
                    if (tradeDetail[i].CloseWeiTuo[m].ClientBianHao == clientbianhao)
                    {
                        tradeDetail[i].CloseWeiTuo[m].Time = weituotime;
                        tradeDetail[i].CloseWeiTuo[m].WeiTuoDealPrice = dealPrice;
                        tradeDetail[i].CloseWeiTuo[m].WeiTuoDealFee = fee;
                        tradeDetail[i].CloseWeiTuo[m].WeiTuoDealQty = dealQty;
                        tradeDetail[i].CloseWeiTuo[m].Done = done;
                        RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(tradeDetail[i]));
                        return;
                    }
                }

            }
            return;
        }
        //args.Wt.Time,args.Wt.WTnbr,args.Wt.Price_deal,args.Wt.Qty_deal,args.Wt.Name

    }
}
