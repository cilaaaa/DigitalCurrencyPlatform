using DataBase;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PolicyBtcFuture0315
{
    public class Parameter : PolicyParameter
    {
        private bool debugModel;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("回测模式")]
        [Description("回测模式")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool DebugModel
        {
            get { return debugModel; }
            set { debugModel = value; }
        }

        private int barCountIn;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("入场BAR数量")]
        [Description("入场BAR数量")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountIn
        {
            get { return barCountIn; }
            set { barCountIn = value; }
        }

        private double zhiYingBeiShu;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开启止盈的倍数")]
        [Description("开启止盈的倍数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double ZhiYingBeiShu
        {
            get { return zhiYingBeiShu; }
            set { zhiYingBeiShu = value; }
        }

        private bool limitYinYang;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("限制阴阳")]
        [Description("阴线做空，阳线做多")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool LimitYinYang
        {
            get { return limitYinYang; }
            set { limitYinYang = value; }
        }

        private bool limitZhenFu;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("限制日振幅开仓")]
        [Description("限制日振幅开仓")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool LimitZhenFu
        {
            get { return limitZhenFu; }
            set { limitZhenFu = value; }
        }

        private int zhenFuDaXiao;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("日振幅大小")]
        [Description("日振幅大小")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public int ZhenFuDaXiao
        {
            get { return zhenFuDaXiao; }
            set { zhenFuDaXiao = value; }
        }

        private bool zuoDuo;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("是否做多")]
        [Description("")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool ZuoDuo
        {
            get { return zuoDuo; }
            set { zuoDuo = value; }
        }

        private bool zuoKong;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("是否做空")]
        [Description("")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool ZuoKong
        {
            get { return zuoKong; }
            set { zuoKong = value; }
        }

        private int barInteval;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("bar间隔，秒")]
        [Description("bar间隔，秒")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarInteval
        {
            get { return barInteval; }
            set { barInteval = value; }
        }

        private double tongDao;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("通道大小")]
        [Description("振幅大于通道才开仓")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double TongDao
        {
            get { return tongDao; }
            set { tongDao = value; }
        }


        private double zhiYingZhenFu;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("止盈振幅")]
        [Description("振幅大于该值才止盈")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiYingZhenFu
        {
            get { return zhiYingZhenFu; }
            set { zhiYingZhenFu = value; }
        }

        private double huiCeBiLi;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("回测比例")]
        [Description("回测比例")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double HuiCeBiLi
        {
            get { return huiCeBiLi; }
            set { huiCeBiLi = value; }
        }

        private int barCountOut;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("出场BAR数量")]
        [Description("出场BAR数量")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountOut
        {
            get { return barCountOut; }
            set { barCountOut = value; }
        }

        private double zhiYingDianShu;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("止盈点数")]
        [Description("止盈点数")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiYingDianShu
        {
            get { return zhiYingDianShu; }
            set { zhiYingDianShu = value; }
        }

        private double beiDongZhiSunPercent;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("被动止损百分比")]
        [Description("被动止损百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double BeiDongZhiSunPercent
        {
            get { return beiDongZhiSunPercent; }
            set { beiDongZhiSunPercent = value; }
        }

        private JiaCangLeiXing jiaCangLeiXing;

        [Browsable(true)]
        [Category("4-海龟参数")]
        [DisplayName("加仓类型")]
        [Description("加仓类型")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public JiaCangLeiXing JiaCangLeiXing
        {
            get { return jiaCangLeiXing; }
            set { jiaCangLeiXing = value; }
        }

        private double jiaCangPriceBeiShu;

        [Browsable(true)]
        [Category("4-海龟参数")]
        [DisplayName("加仓价格倍数")]
        [Description("加仓价格倍数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double JiaCangPriceBeiShu
        {
            get { return jiaCangPriceBeiShu; }
            set { jiaCangPriceBeiShu = value; }
        }

        private double jiaCangQtyBeiShu;

        [Browsable(true)]
        [Category("4-海龟参数")]
        [DisplayName("加仓数量倍数")]
        [Description("加仓数量倍数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double JiaCangQtyBeiShu
        {
            get { return jiaCangQtyBeiShu; }
            set { jiaCangQtyBeiShu = value; }
        }

        private int jiaCangCiShu;

        [Browsable(true)]
        [Category("4-海龟参数")]
        [DisplayName("加仓次数")]
        [Description("加仓次数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public int JiaCangCiShu
        {
            get { return jiaCangCiShu; }
            set { jiaCangCiShu = value; }
        }

        private bool funJiaCang;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("加仓")]
        [Description("加仓")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunJiaCang
        {
            get { return funJiaCang; }
            set { funJiaCang = value; }
        }

        private bool funFanXiangTR;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("反向TR")]
        [Description("反向TR")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunFanXiangTR
        {
            get { return funFanXiangTR; }
            set { funFanXiangTR = value; }
        }

        private bool funZhengXiangTR;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("正向TR")]
        [Description("正向TR")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunZhengXiangTR
        {
            get { return funZhengXiangTR; }
            set { funZhengXiangTR = value; }
        }

        private bool funBeiDongZhiSun;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("被动止损")]
        [Description("被动止损")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunBeiDongZhiSun
        {
            get { return funBeiDongZhiSun; }
            set { funBeiDongZhiSun = value; }
        }

        private bool funZhiYing;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("止盈")]
        [Description("多重止损")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunZhiYing
        {
            get { return funZhiYing; }
            set { funZhiYing = value; }
        }

        private bool funTongDao;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("通道")]
        [Description("通道")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunTongDao
        {
            get { return funTongDao; }
            set { funTongDao = value; }
        }

        private bool funZongYingLiZhiYing;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("使用总盈利止盈")]
        [Description("使用总盈利止盈")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunZongYingLiZhiYing
        {
            get { return funZongYingLiZhiYing; }
            set { funZongYingLiZhiYing = value; }
        }

        private bool funJinChangJiSuanChuChangBar;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("进场开始计算出场bar")]
        [Description("进场开始计算出场bar")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunJinChangJiSuanChuChangBar
        {
            get { return funJinChangJiSuanChuChangBar; }
            set { funJinChangJiSuanChuChangBar = value; }
        }

        private bool funTongXiangZhiSun;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("同向止损")]
        [Description("同向止损")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunTongXiangZhiSun
        {
            get { return funTongXiangZhiSun; }
            set { funTongXiangZhiSun = value; }
        }

        private bool funXianZhiKaiCang;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("限制开仓")]
        [Description("限制开仓")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunXianZhiKaiCang
        {
            get { return funXianZhiKaiCang; }
            set { funXianZhiKaiCang = value; }
        }

        private bool funXianZhiJinChangBar;

        [Browsable(true)]
        [Category("5-功能开关")]
        [DisplayName("限制出场时延用进场的bar")]
        [Description("限制出场时延用进场的bar")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public bool FunXianZhiJinChangBar
        {
            get { return funXianZhiJinChangBar; }
            set { funXianZhiJinChangBar = value; }
        }


        private double stoploss;
        [Browsable(false)]
        //[Browsable(true)]
        //[Category("3-出场参数")]
        //[DisplayName("止损点")]
        //[Description("止损百分比")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        public double Stoploss
        {
            get { return stoploss; }
            set { stoploss = value; }
        }

        private int stopLossPercent;
        [Browsable(false)]
        //[Browsable(true)]
        //[Category("3-出场参数")]
        //[DisplayName("止损大单减少到原来的百分比")]
        //[Description("值为20表示止损大单的数量小于原先的20%止损")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        public int StopLossPercent
        {
            get { return stopLossPercent; }
            set { stopLossPercent = value; }
        }


        public  bool save;


        public Parameter()
        {
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            

            //this.bid1amount = 5;
            //this.checkBid1 = true;
            //this.checkBid123 = false;
            //this.Bid123amount = 10;
            //this.isDropValue = true;
            //this.isTimelyDrop = false;
            //this.timelyDropSecond = 120;
            //this.timelyDropValue = 1;
            //this.stoploss = 0.6;
            //this.largeCount = 3;
            //this.largePercent = 80;
            //this.stopLossPercent = 40;
            this.save = false;
        }

        
        public Parameter(SecurityInfo si, PolicyParameter pp,string account)
            : this(si,account,"Parameter.xml")
        {
            this.startDate = pp.StartDate;
            this.endDate = pp.EndDate;
            this.inteval = 0;
            this.isReal = false;

        }
        public Parameter(SecurityInfo si,string account,string filename)
        {
            GetGeneralValue();
            this.debugModel = true;
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = true;
            this.SI = si;
            this.Account = account;
            this.Filename = filename;
            this.ProgramName = "PolicyBtcFuture0315.DLL";
            this.barCountIn = 120;
            this.barCountOut = 30;
            this.zhiYingBeiShu = 20;
            this.huiCeBiLi = 50;
            this.zuoDuo = true;
            this.zuoKong = true;
            this.barInteval = 60;
            this.limitYinYang = false;
            this.limitZhenFu = false;
            this.zhenFuDaXiao = 1;
            this.qty = 1000;
            this.fee = 0;
            this.jiaCangPriceBeiShu = 2;
            this.jiaCangQtyBeiShu = 1;
            this.jiaCangCiShu = 4;
            this.zhiYingZhenFu = 50;
            
            this.zhiYingDianShu = 100;
            this.tongDao = 0.1;

            //功能块
            this.funTongDao = false;
            this.funFanXiangTR = false;
            this.funBeiDongZhiSun = false;
            this.funZhengXiangTR = false;
            this.funZhiYing = false;
            this.funZongYingLiZhiYing = false;
            this.funJinChangJiSuanChuChangBar = false;
            this.funTongXiangZhiSun = false;
            this.funXianZhiKaiCang = false;
            this.FunXianZhiJinChangBar = false;
        }

        internal void Save()
        {
        }
    }
}

