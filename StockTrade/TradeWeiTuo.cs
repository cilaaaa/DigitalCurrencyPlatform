using StockData;
using System;
using System.Collections.Generic;

namespace StockTrade
{
    public class TradeWeiTuo
    {
        string clientBianHao;

        public string ClientBianHao
        {
            get { return clientBianHao; }
            set { clientBianHao = value; }
        }

        List<TradeChenJiao> chenJiao;

        public List<TradeChenJiao> ChenJiao
        {
            get { return chenJiao; }
            set { chenJiao = value; }
        }
        public TradeWeiTuo()
        {
            this.orderPrice = 0;
            this.orderQty = 0;
        }
        public TradeWeiTuo(string clientbianhao, double price, double qty, OpenType ot)
        {
            this.clientBianHao = clientbianhao;
            this.orderPrice = price;
            this.orderQty = qty;
            this.openType = ot;
            //this.dealQty = 0;
            //this.cancelQty = 0;
            this.weiTuoDealPrice = 0;
            this.weiTuoDealQty = 0;
            
            chenJiao = new List<TradeChenJiao>();
            done = false;
        }
        double orderQty;

        public double OrderQty
        {
            get { return orderQty; }
            set { orderQty = value; }
        }
        double orderPrice;

        public double OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value; }
        }
        OpenType openType;

        public OpenType OpenType
        {
            get { return openType; }
            set { openType = value; }
        }

        double weiTuoDealQty;
        /// <summary>
        /// 委托接口获得的成交数量/不做计算使用
        /// </summary>
        public double WeiTuoDealQty
        {
            get { return weiTuoDealQty; }
            set { weiTuoDealQty = value; }
        }


        double chengJiaoPrice;
        /// <summary>
        /// 该委托平均成交价格
        /// </summary>
        public double ChengJiaoPrice
        {
            get {
                //double price = 0;
                double totalamt = 0;
                double totalprice = 0;
                for(int i=0;i<this.chenJiao.Count;i++)
                {
                    totalamt += this.chenJiao[i].Qty;
                    totalprice += this.chenJiao[i].Qty * this.chenJiao[i].Price;
                }
                if (totalamt == 0)
                {
                    return 0;
                }
                else
                {
                    return Math.Round(totalprice / totalamt, 8, MidpointRounding.AwayFromZero);
                }
            } 
        }

        double chengJiaoFee;
        /// <summary>
        /// 该委托平均成交手续费
        /// </summary>
        public double ChengJiaoFee
        {
            get
            {
                double fee = 0;
                for (int i = 0; i < this.chenJiao.Count; i++)
                {
                    fee += this.chenJiao[i].Fee;
                }
                return fee;
            }
        }

        int chengJiaoQty;
        /// <summary>
        /// 该委托成交数量
        /// </summary>
        public double ChengJiaoQty
        {
            get
            {
                double totalQty = 0;
                for (int i = 0; i < this.chenJiao.Count; i++)
                {
                    totalQty += this.chenJiao[i].Qty;
                }
                return totalQty;
            }
        }

        double weiTuoDealPrice;
        /// <summary>
        /// 委托接口得到的成交价格，不做计算用
        /// </summary>
        public double WeiTuoDealPrice
        {
            get { return this.weiTuoDealPrice; }
            set { weiTuoDealPrice = value; }
        }

        double weiTuoDealFee;
        /// <summary>
        /// 委托接口得到的成交手续费价格，不做计算用
        /// </summary>
        public double WeiTuoDealFee
        {
            get { return this.weiTuoDealFee; }
            set { weiTuoDealFee = value; }
        }
        string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        bool done;

        public bool Done
        {
            get { return done; }
            set { done = value; }
        }

    }
}
