using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;


namespace StockTrade
{
    public class Trader
    {

        /// <summary>
        /// 强制停止
        /// </summary>
        /// 
        bool _fullStop;
        bool EnableCID = false;

        TraderAccount _acct;

        Thread _thread_Connect;
        Thread _thread_MonitorCanceledWeiTuoList;

        Thread _thread_MonitorKeCheListThread;
        public string AccountID
        {
            get
            {
                return _acct.AccountID;
            }
        }

        public TraderAccount TA
        {
            get
            {
                return _acct;
            }
        }

        private OnHandDetail _onHandDetail;
        private TradeHistory _tradeHistory;
        public TradeHistory TradeHistory
        {
            get
            {
                return _tradeHistory;
            }
        }

        List<StockChengJiao> _rcvd_chenjiaoList;
        ConcurrentDictionary<string, StockWeiTuo> _rcvd_kecheList;
        Dictionary<string, StockWeiTuo> _rcvd_weituoList;
        List<StockZhanghao> _rcvd_zhanghaoList;
        List<StockZiJing> _rcvd_zijingList;
        List<StockChiCang> _rcvd_chicangList;
        List<string> CancelingList;
        List<string> RemovedCid = new List<string>();

        ConcurrentQueue<WeiTuoEventArgs> _monitoredWeiTuoEvents;
        ConcurrentDictionary<string, KeCheDetail> _monitoredKeCheList;
        ConcurrentDictionary<string, MarketDetail> _monitoredMarketList;
        List<KeCheArgs> KeCheArgsList;
        private ConcurrentDictionary<string, CanceledWeiTuo> _monitoredCanceledWeiTuoList;
        private Dictionary<string, StockWeiTuo> _history_weituoList;
        private Dictionary<string, string> _CID_WID;

        public delegate void PolicyNotify(object sender, PolicyNotifyEventArgs args);
        public event PolicyNotify Policy_Notify;

        private void RaisePolicyNotify(PolicyNotifyEventArgs args)
        {
            if (Policy_Notify != null)
            {
                Policy_Notify(this, args);
            }
        }

        public delegate void StockChenJiaoArrival(object sender, ChengJiaoEventArgs args);
        public event StockChenJiaoArrival ChenJiao_Arrival;

        private void RaiseChenJiao(ChengJiaoEventArgs args)
        {
            TradeChenJiao tcj = new TradeChenJiao();
            tcj.WeituoBianhao = args.Cj.WTnbr;
            tcj.ClientBianhao = args.Cj.CWTnbr;
            tcj.Bianhao = args.Cj.CJnbr;
            tcj.Price = args.Cj.Price;
            tcj.Qty = args.Cj.Qty;
            tcj.Fee = args.Cj.Fee;
            _tradeHistory.UpdateChenJiao(tcj);
            if (ChenJiao_Arrival != null)
            {
                ChenJiao_Arrival(this, args);
            }
        }

        public delegate void StockKeCheChange(object sender, KeCheEventArgs args);
        public event StockKeCheChange KeChe_Change;

        private void RaiseKeChe()
        {
            while (!_fullStop)
            {
                Dictionary<string, StockWeiTuo> kechelist = new Dictionary<string, StockWeiTuo>();
                foreach (var item in _rcvd_kecheList)
                {
                    kechelist.Add(item.Key, item.Value);
                    TA.Bta.GetOrder(GlobalValue.GetFutureByCodeAndMarket(item.Value.Code, TA.Bta.market), item.Value.WTnbr);
                }
                KeCheEventArgs args = new KeCheEventArgs(kechelist);
                if (KeChe_Change != null)
                {
                    KeChe_Change(this, args);
                }
                Thread.Sleep(2000);
            }
        }

        public delegate void StockChiCangChange(object sender, ChiCangEventArgs args);
        public event StockChiCangChange ChiCang_Change;

        private void RaiseChiCang(ChiCangEventArgs args)
        {
            if (ChiCang_Change != null)
            {
                ChiCang_Change(this, args);
            }
        }

        public delegate void StockWeiTuoArrival(object sender, WeiTuoEventArgs args);
        public event StockWeiTuoArrival WeiTuo_Change;

        private void RaiseWeiTuo(WeiTuoEventArgs args)
        {

            bool dealed = false;
            bool canceled = false;
            bool done = false;

            if (args.Wt.Status == TradeOrderStatus.Filled)
            {
                dealed = true;
            }
            if (args.Wt.Status == TradeOrderStatus.Cancelled)
            {
                canceled = true;
            }
            if (dealed || canceled)
            {
                done = true;
            }
            args.Done = done;
            if (done)
            {
                if (_rcvd_weituoList.ContainsKey(args.Wt.CWTnbr))
                {
                    _rcvd_weituoList.Remove(args.Wt.CWTnbr);
                }
            }
            if (_monitoredKeCheList.ContainsKey(args.Wt.CWTnbr))
            {
                _monitoredKeCheList[args.Wt.CWTnbr].DealedQty = args.Wt.Qty_deal;
                _monitoredKeCheList[args.Wt.CWTnbr].DealedPrice = args.Wt.Price_deal;
                if (args.Wt.Qty_deal == args.Wt.Qty && args.Wt.Qty_deal != 0)
                {
                    _monitoredKeCheList[args.Wt.CWTnbr].Dealed = true;
                }
                if (done)
                {
                    _monitoredKeCheList[args.Wt.CWTnbr].Finished = true;
                    _monitoredKeCheList[args.Wt.CWTnbr].Dealed = true;
                    _monitoredKeCheList[args.Wt.CWTnbr].Canceled = true;
                }
            }

            if (dealed || canceled)
            {
                foreach (var item in _monitoredCanceledWeiTuoList)
                {
                    CanceledWeiTuo cwt = item.Value;
                    if (cwt.Status == CanceledWeiTuoStatus.WaitForConfirm)
                    {
                        if (cwt.Clientbianhao == args.Wt.CWTnbr)
                        {
                            if (dealed)
                            {
                                if (cwt.TradeType == PolicyTradeType.Open)
                                {
                                    ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.Opened, args.Wt.Qty_deal, args.Wt.Price_deal);
                                }
                                else
                                {
                                    ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.Closed, args.Wt.Qty_deal, args.Wt.Price_deal, cwt.Clientbianhao);
                                }
                                ExportMessage(new MessageEventArgs(string.Format("{1}-委托单{0}已全部成交,无需再出场", cwt.Clientbianhao, cwt.Si.Code)));
                                cwt.UnDealedQty = 0;
                                cwt.Status = CanceledWeiTuoStatus.ReEntered;
                            }
                            else if (canceled)
                            {
                                cwt.UnDealedQty = (double)((decimal)args.Wt.Qty - (decimal)args.Wt.Qty_deal);
                                cwt.Status = CanceledWeiTuoStatus.CancelConfirmed;
                                if (args.Wt.Qty_deal > 0)
                                {
                                    if (cwt.TradeType == PolicyTradeType.Open)
                                    {
                                        ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.PartOpend, args.Wt.Qty_deal, args.Wt.Price_deal);
                                    }
                                    else
                                    {
                                        ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.PartClosed, args.Wt.Qty_deal, 0, cwt.Clientbianhao);
                                    }
                                }
                                if (cwt.Res == false)
                                {
                                    if (cwt.TradeType == PolicyTradeType.Open)
                                    {
                                        ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.OpenCanceled, cwt.UnDealedQty);
                                    }
                                    else
                                    {
                                        ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.CloseCanceled, cwt.UnDealedQty);
                                    }
                                }
                                ExportMessage(new MessageEventArgs(string.Format("{0}-委托单{1}撤单成功，剩余{2}", cwt.Si.Code, cwt.Clientbianhao, cwt.UnDealedQty)));
                            }
                            else
                            {
                                ExportMessage(new MessageEventArgs(string.Format("意外情况{0}-{1}", args.Wt.Status, args.Wt.CWTnbr)));
                            }
                            break;
                        }
                    }
                }
            }
            if (args.Wt.Qty_deal > 0)
            {
                if (_monitoredMarketList.ContainsKey(args.Wt.CWTnbr))
                {
                    MarketDetail md = _monitoredMarketList[args.Wt.CWTnbr];
                    if (md.OpenQty == args.Wt.Qty_deal)
                    {
                        //完全成交
                        if (md.TradeType == PolicyTradeType.Open)
                        {
                            //////插入提示开仓成功代码
                            ((RunningPolicy)md.PolicyObject).Notify(md.TradeGuid, OpenStatus.Opened, args.Wt.Qty_deal, args.Wt.Price_deal);
                        }
                        if (md.TradeType == PolicyTradeType.Close)
                        {
                            //////插入提示平仓成功代码
                            ((RunningPolicy)md.PolicyObject).Notify(md.TradeGuid, OpenStatus.Closed, args.Wt.Qty_deal, args.Wt.Price_deal, md.ClientBianHao);
                        }
                        MarketDetail mdtemp;
                        _monitoredMarketList.TryRemove(args.Wt.CWTnbr, out mdtemp);
                    }
                    else
                    {
                        //部分成交
                        if (md.TradeType == PolicyTradeType.Open)
                        {
                            ((RunningPolicy)md.PolicyObject).Notify(md.TradeGuid, OpenStatus.PartOpend, args.Wt.Qty_deal, args.Wt.Price_deal);
                        }
                        else
                        {
                            ((RunningPolicy)md.PolicyObject).Notify(md.TradeGuid, OpenStatus.PartClosed, args.Wt.Qty_deal, args.Wt.Price_deal, md.ClientBianHao);
                        }
                    }
                }
            }
            _monitoredWeiTuoEvents.Enqueue(args);
        }

        public delegate void StockZiJingUpdate(object sender, ZiJingEventArgs args);
        public event StockZiJingUpdate ZiJing_Update;

        private void RaiseZiJing(ZiJingEventArgs args)
        {
            if (ZiJing_Update != null)
            {
                ZiJing_Update(this, args);
            }
        }

        public delegate void MessageArrival(object sender, MessageEventArgs args);
        public event MessageArrival Message_Arrival;
        private void ExportMessage(MessageEventArgs args)
        {
            if (Message_Arrival != null)
            {
                Message_Arrival(this, args);
            }
        }

        public delegate void ConnectSuccessfully(object sender, EventArgs args);
        public event ConnectSuccessfully Account_Connected;

        private void RaiseConnectedEvent()
        {
            if (Account_Connected != null)
            {
                Account_Connected(this, new EventArgs());
            }
        }

        public delegate void TradeDetailUpdate(object sender, TradeDetailUpdateEventArgs args);
        public event TradeDetailUpdate TradeDetail_Update;
        public void RaiseTradeDetailUpdate(TradeDetailUpdateEventArgs args)
        {
            if (TradeDetail_Update != null)
            {
                TradeDetail_Update(this, args);
            }
        }

        private Trader(TraderAccount account)
        {
            this._acct = account;
            this.EnableCID = account.Bta.EnableCID;
            this._rcvd_chenjiaoList = new List<StockChengJiao>();
            this._rcvd_kecheList = new ConcurrentDictionary<string, StockWeiTuo>();
            this._rcvd_weituoList = new Dictionary<string, StockWeiTuo>();
            this._rcvd_zhanghaoList = new List<StockZhanghao>();
            this._rcvd_zijingList = new List<StockZiJing>();
            this._rcvd_chicangList = new List<StockChiCang>();
            _onHandDetail = new OnHandDetail();
            _onHandDetail.Change += _onHandDetail_Change;
            _tradeHistory = new TradeHistory();
            _tradeHistory.TradeDetail_Update += _tradeHistory_TradeDetail_Update;
            KeCheArgsList = new List<KeCheArgs>();
            _monitoredWeiTuoEvents = new ConcurrentQueue<WeiTuoEventArgs>();
            _monitoredKeCheList = new ConcurrentDictionary<string, KeCheDetail>();
            _monitoredMarketList = new ConcurrentDictionary<string, MarketDetail>();
            _monitoredCanceledWeiTuoList = new ConcurrentDictionary<string, CanceledWeiTuo>();
            CancelingList = new List<string>();
            _acct.Bta.ChiCang_Arrival += QueryChiCang;
            _acct.Bta.WeiTuo_Arrival += QueryWeiTuo;
            _acct.Bta.WeiTuo_Arrival += QueryKeChe;
            _acct.Bta.ZiJin_Arrival += QueryZiJing;
            _history_weituoList = new Dictionary<string, StockWeiTuo>();
            _CID_WID = new Dictionary<string, string>();
        }

        public event OnHandDetailChangeDelegate onHandDetailChange;
        void _onHandDetail_Change(object sender, OnHandDetailChangeEventArgs args)
        {
            if (onHandDetailChange != null)
            {
                onHandDetailChange(sender, args);
            }
        }
        void _tradeHistory_TradeDetail_Update(object sender, TradeDetailUpdateEventArgs args)
        {
            RaiseTradeDetailUpdate(args);
        }

        public void TryConnect()
        {
            this._fullStop = false;
            ThreadStart ts = new ThreadStart(startConnect);
            _thread_Connect = new Thread(ts);
            _thread_Connect.Start();
        }

        private void startConnect()
        {
            RaiseConnectedEvent();

            StartKeCheUpdate();

            StartWeiTuoUpdate();

            //启动可撤挂单监控线程
            StartMonitorKeCheList();
            //启动已撤单委托监控
            StartMonitorCanceledWeiTuoList();
        }

        private void StartKeCheUpdate()
        {
            Thread thread_KeCheUpdate = new Thread(new ThreadStart(RaiseKeChe));
            thread_KeCheUpdate.Start();
        }

        private void StartWeiTuoUpdate()
        {
            Thread thread_WeiTuoUpdate = new Thread(new ThreadStart(UpdateWeiTuo));
            thread_WeiTuoUpdate.Start();
        }

        private void StartMonitorCanceledWeiTuoList()
        {
            _thread_MonitorCanceledWeiTuoList = new Thread(new ThreadStart(MonitorCancelWeituoList));
            _thread_MonitorCanceledWeiTuoList.Start();
        }

        private void UpdateWeiTuo()
        {
            WeiTuoEventArgs args;
            while (!_fullStop)
            {
                while (!_monitoredWeiTuoEvents.IsEmpty)
                {
                    if (_monitoredWeiTuoEvents.TryDequeue(out args))
                    {
                        if (args.Wt.Qty_deal > 0)
                        {
                            _tradeHistory.UpdateMonitorWeiTuo(args.Wt.Time, args.Wt.CWTnbr, args.Wt.Price_deal, args.Wt.Qty_deal, args.Wt.Fee, args.Wt.Name, args.Done);
                        }
                        if (WeiTuo_Change != null)
                        {
                            WeiTuo_Change(this, args);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void MonitorCancelWeituoList()
        {
            while (!_fullStop)
            {
                //if (this._acct.ClientID != -1)
                {
                    try
                    {
                        CanceledWeiTuoMonitor();
                    }
                    catch { }
                }
                Thread.Sleep(100);
            }
        }

        private void CanceledWeiTuoMonitor()
        {
            foreach (var item in _monitoredCanceledWeiTuoList)
            {
                CanceledWeiTuo cwt = item.Value;
                if (cwt.Status == CanceledWeiTuoStatus.CancelConfirmed && cwt.Res)
                {
                    double qty = cwt.UnDealedQty;
                    if (qty < cwt.Si.MinQty)
                    {
                        if (cwt.TradeType == PolicyTradeType.Open)
                        {
                            ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.OpenRest, qty);
                        }
                        else
                        {
                            ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.CloseRest, qty);
                        }
                        cwt.Status = CanceledWeiTuoStatus.ReEntered;
                    }
                    else
                    {
                        decimal realqty = Math.Floor((decimal)qty * cwt.Si.JingDu) / cwt.Si.JingDu;
                        decimal restqty = (decimal)qty - realqty;
                        if (restqty != 0)
                        {
                            if (cwt.TradeType == PolicyTradeType.Open)
                            {
                                ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.OpenRest, (double)restqty);
                            }
                            else
                            {
                                ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.CloseRest, (double)restqty);
                            }
                            cwt.UnDealedQty = (double)realqty;
                        }
                        cwt.Status = CanceledWeiTuoStatus.Pending;
                        autoXiaDanDelegate axdd = new autoXiaDanDelegate(autoXiaDan);
                        axdd.BeginInvoke(cwt, (double)realqty, null, null);
                    }
                }
                else if (cwt.Status == CanceledWeiTuoStatus.ReEntered)
                {
                    CanceledWeiTuo cwttemp;
                    _monitoredCanceledWeiTuoList.TryRemove(cwt.Clientbianhao, out cwttemp);
                }
            }
        }

        public int Rand6Int()
        {
            int rtn = 0;
            Random r = new Random();
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            r = new Random(iSeed);
            rtn = r.Next(100000, 999999);
            return rtn;
        }

        delegate void autoXiaDanDelegate(CanceledWeiTuo cwt, double qty);
        private void autoXiaDan(CanceledWeiTuo cwt, double qty)
        {
            string weituobianhao = string.Empty;
            string errorinfo = string.Empty;
            string clientweituobianhao = "EntropyRise" + DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
            TradeDetail td = new TradeDetail();
            _tradeHistory.getTradeDetail(cwt.TradeGuid, ref td);

            StockWeiTuo swt = new StockWeiTuo();
            swt.WTnbr = weituobianhao;
            swt.CWTnbr = clientweituobianhao;
            swt.Qty_deal = 0;
            swt.Status = TradeOrderStatus.New;
            _rcvd_weituoList.Add(swt.CWTnbr, swt);
            RaiseWeiTuo(new WeiTuoEventArgs(swt, DataChangeType.Add));

            ExportMessage(new MessageEventArgs(string.Format("{0}-再出场价格:{1},再出场数量:{2}", cwt.Si.Code, cwt.OrderPrice, qty)));

            if (cwt.TradeType == PolicyTradeType.Open)
            {
                td.OpenWeiTuo.Add(new TradeWeiTuo(clientweituobianhao, cwt.OrderPrice, qty, cwt.OpenType));
            }
            else
            {
                td.CloseWeiTuo.Add(new TradeWeiTuo(clientweituobianhao, cwt.OrderPrice, qty, cwt.OpenType));
            }
            if (cwt.ReTradePriceType == TradeSendOrderPriceType.Market)
            {
                MarketDetail md = new MarketDetail();
                md.ClientBianHao = clientweituobianhao;
                md.TradeGuid = cwt.TradeGuid;
                md.PolicyObject = cwt.PolicyObject;
                md.TradeType = cwt.TradeType;
                md.OpenQty = qty;
                _monitoredMarketList.TryAdd(clientweituobianhao, md);
            }
            if (cwt.ReTradePriceType == TradeSendOrderPriceType.Limit)
            {
                AddNewKeCheMonitor(weituobianhao, clientweituobianhao, cwt.CancelLimitTime, cwt.MarkPrice, cwt.ReTradePriceType, cwt.TradeType, cwt.Si, cwt.OrderPrice, qty, cwt.OpenType, false, cwt.TradeGuid, cwt.ReEnterPercentage, cwt.CancelTimes, cwt.PolicyObject, cwt.Maker, cwt.Leverage);
            }
            if (_acct.AutoXiaDan(cwt.OpenType, cwt.Si.Code, cwt.ReTradePriceType.ToString(), (float)cwt.OrderPrice, qty, cwt.Si.Type, ref weituobianhao, clientweituobianhao, ref errorinfo, cwt.Maker, cwt.Leverage))
            {
                if (cwt.ReTradePriceType == TradeSendOrderPriceType.Limit && _monitoredKeCheList.ContainsKey(clientweituobianhao))
                {
                    try
                    {
                        _monitoredKeCheList[clientweituobianhao].BianHao = weituobianhao;
                    }
                    catch { }
                }
                cwt.CancelTimes += 1;
                if (cwt.TradeType == PolicyTradeType.Open)
                {
                    ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.OpenPending);
                }
                else
                {
                    ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.ClosePending, cwt.OrderQty, 0, cwt.Clientbianhao, clientweituobianhao);
                }
                cwt.Status = CanceledWeiTuoStatus.ReEntered;
                if (!EnableCID)
                {
                    _CID_WID.Add(weituobianhao, clientweituobianhao);
                }
                if (_history_weituoList.ContainsKey(weituobianhao))
                {
                    if (_rcvd_weituoList.ContainsKey(clientweituobianhao))
                    {
                        _rcvd_weituoList[clientweituobianhao].Update(_history_weituoList[weituobianhao]);
                        RaiseWeiTuo(new WeiTuoEventArgs(_rcvd_weituoList[clientweituobianhao], DataChangeType.Change));
                    }
                    QueryKeChe(this, new List<StockWeiTuo>() { _history_weituoList[weituobianhao] });
                    _history_weituoList.Remove(weituobianhao);
                }
                ExportMessage(new MessageEventArgs(string.Format("{1} 追加下单成功，委托编号{0},数量{2}", clientweituobianhao, cwt.Si.Code, qty)));
            }
            else
            {
                Thread.Sleep(500);
                _monitoredCanceledWeiTuoList[cwt.Clientbianhao].Status = CanceledWeiTuoStatus.CancelConfirmed;
                _rcvd_weituoList.Remove(clientweituobianhao);
                if (cwt.ReTradePriceType == TradeSendOrderPriceType.Market)
                {
                    MarketDetail md;
                    _monitoredMarketList.TryRemove(clientweituobianhao, out md);
                }
                if (cwt.ReTradePriceType == TradeSendOrderPriceType.Limit)
                {
                    KeCheDetail kcd;
                    _monitoredKeCheList.TryRemove(clientweituobianhao, out kcd);
                }
                ExportMessage(new MessageEventArgs(string.Format("{0} 追加失败，等待继续下单,原因：{1}", cwt.Si.Code, errorinfo)));
            }

        }

        private void StartMonitorKeCheList()
        {
            _thread_MonitorKeCheListThread = new Thread(new ThreadStart(MonitorKeCheList));
            _thread_MonitorKeCheListThread.Start();
        }

        private void MonitorKeCheList()
        {
            while (!_fullStop)
            {
                //if (this._acct.ClientID != -1)
                {
                    try
                    {
                        KeCheListMonitor();
                    }
                    catch { }

                }
                Thread.Sleep(100);
            }
        }

        private void KeCheListMonitor()
        {
            foreach (var item in _monitoredKeCheList)
            {
                KeCheDetail kcd = item.Value;
                if (!kcd.Dealed || !kcd.Notified) //如没有完成 或者没有通知
                {
                    double dealQty = kcd.DealedQty;
                    if (kcd.OrderQty == dealQty)  //如果成交数量=挂单数量
                    {
                        kcd.Dealed = true;
                        if (kcd.TradeType == PolicyTradeType.Open)
                        {
                            //////插入提示开仓成功代码
                            ((RunningPolicy)kcd.PolicyObject).Notify(kcd.TradeGuid, OpenStatus.Opened, kcd.DealedQty, kcd.DealedPrice);
                            kcd.Notified = true;
                        }
                        if (kcd.TradeType == PolicyTradeType.Close)
                        {
                            //////插入提示平仓成功代码
                            ((RunningPolicy)kcd.PolicyObject).Notify(kcd.TradeGuid, OpenStatus.Closed, kcd.DealedQty, kcd.DealedPrice, kcd.ClientBianHao);
                            kcd.Notified = true;
                        }
                        ExportMessage(new MessageEventArgs(string.Format("{0}-委托单{1}已完成orderqty{2}dealqty{3}", kcd.Si.Code, kcd.ClientBianHao, kcd.OrderQty, kcd.DealedQty)));
                    }
                    else if (dealQty > 0)
                    {
                        //通知部分成交数量
                        if (kcd.TradeType == PolicyTradeType.Open)
                        {
                            ((RunningPolicy)kcd.PolicyObject).Notify(kcd.TradeGuid, OpenStatus.PartOpend, kcd.DealedQty, kcd.DealedPrice);
                        }
                        else
                        {
                            ((RunningPolicy)kcd.PolicyObject).Notify(kcd.TradeGuid, OpenStatus.PartClosed, kcd.DealedQty, 0, kcd.ClientBianHao);
                        }
                    }
                }
                if (!kcd.Dealed && kcd.BianHao != string.Empty) //没有全部成交
                {
                    CheDanArgs cda = ((RunningPolicy)kcd.PolicyObject).PolicyCheDan(kcd);
                    if (cda.Cancel)
                    {
                        if (!CancelingList.Contains(kcd.ClientBianHao))
                        {
                            CanceledWeiTuo cwt = new CanceledWeiTuo();
                            cwt.Status = CanceledWeiTuoStatus.WaitForConfirm;
                            cwt.Bianhao = kcd.BianHao;
                            cwt.Clientbianhao = kcd.ClientBianHao;
                            cwt.MarkPrice = cda.MarkPrice;
                            cwt.OpenType = kcd.OpenType;
                            cwt.OrderPrice = Math.Floor(cda.NewOrderPrice / kcd.Si.PriceJingDu) * kcd.Si.PriceJingDu;
                            cwt.OrderQty = kcd.OrderQty;
                            cwt.PolicyObject = kcd.PolicyObject;
                            cwt.ReTradePriceType = cda.Tsopt;
                            cwt.ReEnterPercentage = kcd.ReEnterPercentage;
                            cwt.Si = kcd.Si;
                            cwt.TradeGuid = kcd.TradeGuid;
                            cwt.TradeType = kcd.TradeType;
                            cwt.UnDealedQty = kcd.UnDealQty;
                            cwt.CancelLimitTime = kcd.WaitSecond;
                            cwt.CancelTimes = kcd.CancelTimes;
                            cwt.Res = cda.ZhuiDan;
                            cwt.Leverage = kcd.Leverage;
                            this._monitoredCanceledWeiTuoList.TryAdd(cwt.Clientbianhao, cwt);
                            CancelingList.Add(kcd.ClientBianHao);
                            kcd.Notified = true;
                            kcd.Dealed = true;
                            ExportMessage(new MessageEventArgs(string.Format("{0}-{1}-等待撤单成功", kcd.Si.Code, EnableCID ? cwt.Clientbianhao : cwt.Bianhao)));
                            CancelOrderDelegate cod = new CancelOrderDelegate(CancelOrder);
                            cod.BeginInvoke(cwt.Si, EnableCID ? cwt.Clientbianhao : cwt.Bianhao, cwt.Clientbianhao, null, null);
                        }
                    }
                }
                if (kcd.Finished && kcd.Notified)
                {
                    KeCheDetail kcdtemp;
                    _monitoredKeCheList.TryRemove(kcd.ClientBianHao, out kcdtemp);
                }
            }
        }


        delegate void CancelOrderDelegate(SecurityInfo si, string Hth, string cid);
        private void CancelOrder(SecurityInfo si, string Hth, string cid)
        {
            string errmsg = string.Empty;
            if (!_acct.CancelOrder(si, Hth, ref errmsg))
            {
                CanceledWeiTuo cwt2;
                if (_monitoredCanceledWeiTuoList.TryGetValue(cid, out cwt2))
                {
                    if (cwt2.Status == CanceledWeiTuoStatus.WaitForConfirm)
                    {
                        ExportMessage(new MessageEventArgs(string.Format("{0}-可撤单监控委托单撤单失败,原因：{1}", cid, errmsg)));
                        var OrderMsg = _acct.Bta.GetOrder(si, Hth);
                        if (OrderMsg != null && OrderMsg.Property("status") == null)
                        {
                            if (_rcvd_weituoList.ContainsKey(cid))
                            {
                                _rcvd_weituoList.Remove(cid);
                            }
                            _monitoredKeCheList[cid].Dealed = true;
                            foreach (var item in _monitoredCanceledWeiTuoList)
                            {
                                CanceledWeiTuo cwt = item.Value;
                                if (cwt.Status == CanceledWeiTuoStatus.WaitForConfirm)
                                {
                                    if (cwt.Clientbianhao == cid)
                                    {
                                        cwt.Status = CanceledWeiTuoStatus.CancelConfirmed;
                                        if (cwt.Res == false)
                                        {
                                            if (cwt.TradeType == PolicyTradeType.Open)
                                            {
                                                ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.OpenCanceled, cwt.UnDealedQty, 0, cwt.Clientbianhao);
                                            }
                                            else
                                            {
                                                ((RunningPolicy)cwt.PolicyObject).Notify(cwt.TradeGuid, OpenStatus.CloseCanceled, cwt.UnDealedQty, 0, cwt.Clientbianhao);
                                            }
                                        }
                                        ExportMessage(new MessageEventArgs(string.Format("{0}-委托单{1}撤单成功，剩余{2}", cwt.Si.Code, cwt.Clientbianhao, cwt.UnDealedQty)));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ExportMessage(new MessageEventArgs(string.Format("委托单{0}自动撤单成功,等待撤单成功", Hth)));
            }
            CancelingList.Remove(Hth);
            Thread.Sleep(1000);
            CanceledWeiTuo cwt3;
            if (_monitoredCanceledWeiTuoList.TryGetValue(cid, out cwt3))
            {
                if (cwt3.Status == CanceledWeiTuoStatus.WaitForConfirm)
                {
                    KeCheDetail kcd;
                    if (_monitoredKeCheList.TryGetValue(cid, out kcd))
                    {
                        if (!kcd.Canceled)
                        {
                            CancelOrder(si, Hth, cid);
                        }
                    }
                }
            }

        }


        private void QueryChiCang(object sender, List<StockChiCang> args)
        {
            _rcvd_chicangList = args;
            RaiseChiCang(new ChiCangEventArgs(_rcvd_chicangList));
        }

        private void QueryZiJing(object sender, List<StockZiJing> args)
        {
            for (int m = 0; m < args.Count; m++)
            {
                StockZiJing zj = args[m];
                if (zj.Instrument_id != null)
                {
                    bool find = false;
                    for (int i = 0; i < _rcvd_zijingList.Count; i++)
                    {
                        if (_rcvd_zijingList[i].Instrument_id == zj.Instrument_id)
                        {
                            find = true;
                            _rcvd_zijingList[i] = zj;
                            RaiseZiJing(new ZiJingEventArgs(zj, DataChangeType.Change));
                            break;
                        }
                    }
                    if (!find)
                    {
                        _rcvd_zijingList.Add(zj);
                        RaiseZiJing(new ZiJingEventArgs(zj, DataChangeType.Add));
                    }
                }
            }
        }

        private void QueryWeiTuo(object sender, List<StockWeiTuo> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                StockWeiTuo wt = args[i];
                if (!EnableCID)
                {
                    if (_CID_WID.ContainsKey(wt.WTnbr))
                    {
                        wt.CWTnbr = _CID_WID[wt.WTnbr];
                    }
                    else
                    {
                        wt.CWTnbr = "0";
                    }
                }
                if (_rcvd_weituoList.ContainsKey(wt.CWTnbr))
                {
                    DataChangeType t_changeType = DataChangeType.None;
                    try
                    {
                        StockWeiTuo st = _rcvd_weituoList[wt.CWTnbr];
                        if (st.Qty_deal != wt.Qty_deal || st.Status != wt.Status || st.WTnbr != wt.WTnbr)
                        {
                            t_changeType = DataChangeType.Change;
                            st.Update(wt);
                        }
                    }
                    catch (Exception e)
                    {
                        ExportMessage(new MessageEventArgs(string.Format("委托单信息报错：{0}", e.Message)));
                    }
                    if (t_changeType != DataChangeType.None)
                    {
                        RaiseWeiTuo(new WeiTuoEventArgs(wt, t_changeType));
                    }
                }
                else
                {
                    if (_history_weituoList.ContainsKey(wt.WTnbr))
                    {
                        _history_weituoList[wt.WTnbr].Update(wt);
                    }
                    else
                    {
                        _history_weituoList.Add(wt.WTnbr, wt);
                    }
                }
            }
        }

        object lockKeChe = new object();
        private void QueryKeChe(object sender, List<StockWeiTuo> args)
        {
            lock (lockKeChe)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    StockWeiTuo wt = args[i];
                    if (!EnableCID)
                    {
                        if (_CID_WID.ContainsKey(wt.WTnbr))
                        {
                            wt.CWTnbr = _CID_WID[wt.WTnbr];
                        }
                        else
                        {
                            wt.CWTnbr = "0";
                        }
                    }
                    if (wt.Status == TradeOrderStatus.Open || wt.Status == TradeOrderStatus.Part_Filled)
                    {
                        if (!_rcvd_kecheList.ContainsKey(wt.CWTnbr) && wt.CWTnbr != "0" && !RemovedCid.Contains(wt.CWTnbr))
                        {
                            _rcvd_kecheList.TryAdd(wt.CWTnbr, wt);
                        }
                    }
                    else
                    {
                        if (_rcvd_kecheList.ContainsKey(wt.CWTnbr))
                        {
                            StockWeiTuo a;
                            _rcvd_kecheList.TryRemove(wt.CWTnbr, out a);
                            RemovedCid.Add(wt.CWTnbr);
                        }
                    }
                }
            }
        }

        private void QueryChenJiao(object sender, List<StockChengJiao> args)
        {
            //if (true || _needMonitorChengJiaoList.Count != 0)
            {
                foreach (StockChengJiao cj in args)
                {
                    //if (true || _needMonitorChengJiaoList.Contains(cj.WTnbr))
                    {
                        bool find = false;
                        for (int i = 0; i < this._rcvd_chenjiaoList.Count; i++)
                        {
                            if (_rcvd_chenjiaoList[i].CJnbr == cj.CJnbr)
                            {
                                find = true;
                                break;
                            }
                        }
                        if (!find)
                        {
                            _rcvd_chenjiaoList.Add(cj);
                            RaiseChenJiao(new ChengJiaoEventArgs(cj, DataChangeType.Add));
                        }
                    }
                }
            }
        }



        public static Trader CreateTrader(TraderAccount sa)
        {
            Trader trader = new Trader(sa);
            return trader;
        }

        public void policyParamChange(RunningPolicy policy, string ParamName, string ParamValue)
        {
            //if (policy.PolicyName.Contains("PolicyBtcFuture0828"))
            //{
            //    switch (ParamName)
            //    {
            //        case "ZhiSunJinE":
            //        {
            //            for (int i = 0; i < _monitoredKeCheList.Count; i++)
            //            {
            //                if (!_monitoredKeCheList[i].Dealed)
            //                {
            //                    if (((RunningPolicy)_monitoredKeCheList[i].PolicyObject).PolicyName.Contains("PolicyBtcFuture0828"))
            //                    {
            //                        _monitoredKeCheList[i].ReEnterPercentage = System.Convert.ToDouble(ParamValue);
            //                    }
            //                }
            //            }
            //            break;
            //        }
            //        case "OpenDuoIndex":
            //        {
            //            for (int i = 0; i < KeCheArgsList.Count; i++)
            //            {
            //                if (KeCheArgsList[i].PolicyName.Contains("PolicyBtcFuture0828"))
            //                {
            //                    KeCheArgsList[i].OpenDuoIndex = System.Convert.ToInt16(ParamValue);
            //                }
            //            }
            //            break;
            //        }
            //        case "OpenKongIndex":
            //        {
            //            for (int i = 0; i < KeCheArgsList.Count; i++)
            //            {
            //                if (KeCheArgsList[i].PolicyName.Contains("PolicyBtcFuture0828"))
            //                {
            //                    KeCheArgsList[i].OpenKongIndex = System.Convert.ToInt16(ParamValue);
            //                }
            //            }
            //            break;
            //        }
            //        default: break;
            //    }
            //}
        }

        public void AddKeCheArgs(KeCheArgs args)
        {
            KeCheArgsList.Add(args);
        }


        public void policyTrade(SecurityInfo securityInfo, string policyName, TradePoints tradePoints, RunningPolicy policy, PolicyTradeType policyTradeType)
        {
            if (policyTradeType == PolicyTradeType.Open)
            {
                double orderQty = tradePoints.EnterPoint.OpenQty;
                string weituobianhao = string.Empty;
                string errorinfo = string.Empty;
                string clientweituobianhao = "EntropyRise" + DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
                StockWeiTuo swt = new StockWeiTuo();
                swt.CWTnbr = clientweituobianhao;
                swt.Qty_deal = 0;
                swt.Status = TradeOrderStatus.New;
                _rcvd_weituoList.Add(swt.CWTnbr, swt);
                RaiseWeiTuo(new WeiTuoEventArgs(swt, DataChangeType.Add));

                tradePoints.EnterPoint.OpenPrice = Math.Floor(tradePoints.EnterPoint.OpenPrice / securityInfo.PriceJingDu) * securityInfo.PriceJingDu;
                //if (TradeHistory.XiaDan(args.PairePoint.EnterPoint.OpenType, args.PairePoint.EnterPoint.SecInfo, args.PairePoint.EnterPoint.OpenPriceType, (float)args.PairePoint.EnterPoint.OpenPrice, orderQty, ref weituobianhao, ref errorinfo))
                TradeDetail td = new TradeDetail();
                td.PolicyName = policyName;
                td.Tradeid = tradePoints.TradeGuid;
                td.TradeOpenOrderQty = orderQty;
                td.TradeSi = tradePoints.EnterPoint.SecInfo;
                td.PolicyOpenPoint = tradePoints.EnterPoint;
                td.TradeOpenOrderPrice = tradePoints.EnterPoint.OpenPrice;
                td.TradeOpenOrderType = tradePoints.EnterPoint.OpenType;

                td.PoicyFee = tradePoints.Fee;
                td.Remark = tradePoints.TpRemark;
                if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Limit) //限价委托
                {
                    this.AddNewKeCheMonitor(weituobianhao, clientweituobianhao, tradePoints.EnterPoint.CancelLimitTime, tradePoints.EnterPoint.OpenPrice, tradePoints.EnterPoint.ReTradePriceType, PolicyTradeType.Open, securityInfo, tradePoints.EnterPoint.OpenPrice, tradePoints.EnterPoint.OpenQty, tradePoints.EnterPoint.OpenType, false, tradePoints.TradeGuid, tradePoints.EnterPoint.ReEnterPecentage, 0, policy, tradePoints.EnterPoint.Maker, tradePoints.EnterPoint.Leverage);
                }
                if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Market) //市价委托
                {
                    //即时成交
                    MarketDetail md = new MarketDetail();
                    md.ClientBianHao = clientweituobianhao;
                    md.TradeGuid = td.Tradeid;
                    md.PolicyObject = policy;
                    md.TradeType = policyTradeType;
                    md.OpenQty = orderQty;
                    _monitoredMarketList.TryAdd(clientweituobianhao, md);
                }
                td.OpenWeiTuo.Add(new TradeWeiTuo(clientweituobianhao, tradePoints.EnterPoint.OpenPrice, orderQty, tradePoints.EnterPoint.OpenType));
                this._tradeHistory.Add(td);
                if (this._acct.AutoXiaDan(tradePoints.EnterPoint.OpenType, tradePoints.EnterPoint.SecInfo.Code, tradePoints.EnterPoint.FirstTradePriceType.ToString(), (float)tradePoints.EnterPoint.OpenPrice, orderQty, tradePoints.EnterPoint.SecInfo.Type, ref weituobianhao, clientweituobianhao, ref errorinfo, tradePoints.EnterPoint.Maker, tradePoints.EnterPoint.Leverage))
                {
                    if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Limit && _monitoredKeCheList.ContainsKey(clientweituobianhao)) //限价委托
                    {
                        try
                        {
                            _monitoredKeCheList[clientweituobianhao].BianHao = weituobianhao;
                        }
                        catch { }
                    }
                    policy.Notify(tradePoints.TradeGuid, OpenStatus.Open);
                    if (!EnableCID)
                    {
                        _CID_WID.Add(weituobianhao, clientweituobianhao);
                    }
                    if (_history_weituoList.ContainsKey(weituobianhao))
                    {
                        if (_rcvd_weituoList.ContainsKey(clientweituobianhao))
                        {
                            _rcvd_weituoList[clientweituobianhao].Update(_history_weituoList[weituobianhao]);
                            RaiseWeiTuo(new WeiTuoEventArgs(_rcvd_weituoList[clientweituobianhao], DataChangeType.Change));
                        }
                        QueryKeChe(this, new List<StockWeiTuo>() { _history_weituoList[weituobianhao] });
                        _history_weituoList.Remove(weituobianhao);
                    }
                    ExportMessage(new MessageEventArgs(string.Format("{0}-{1}-开仓下单成功，委托编号{2}", securityInfo.Code, policyName, weituobianhao)));
                }
                else
                {
                    _rcvd_weituoList.Remove(clientweituobianhao);
                    if (tradePoints.EnterPoint.ReTradePriceType == TradeSendOrderPriceType.Market)
                    {
                        MarketDetail md;
                        _monitoredMarketList.TryRemove(clientweituobianhao, out md);
                    }
                    if (tradePoints.EnterPoint.ReTradePriceType == TradeSendOrderPriceType.Limit)
                    {
                        KeCheDetail kcd;
                        _monitoredKeCheList.TryRemove(clientweituobianhao, out kcd);
                    }
                    policy.Notify(tradePoints.TradeGuid, OpenStatus.OpenCanceled, tradePoints.EnterPoint.OpenQty);
                    _onHandDetail.UpdateAvailableQty(securityInfo, -orderQty);
                    ExportMessage(new MessageEventArgs(string.Format("{0}-{2}-开仓失败:{1},价格:{3}", tradePoints.EnterPoint.SecInfo.Code, errorinfo, policyName, tradePoints.EnterPoint.OpenPrice)));
                }

            }
            else
            {
                double qty = tradePoints.OutPoint.OpenQty;

                if (qty > 0)
                {
                    decimal realqty = Math.Floor((decimal)qty * securityInfo.JingDu) / securityInfo.JingDu;
                    if (realqty != 0)
                    {
                        string weituobianhao = string.Empty;
                        string errorinfo = string.Empty;
                        string clientweituobianhao = "EntropyRise" + DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
                        tradePoints.OutPoint.OpenPrice = Math.Floor(tradePoints.OutPoint.OpenPrice / securityInfo.PriceJingDu) * securityInfo.PriceJingDu;
                        StockWeiTuo swt = new StockWeiTuo();
                        swt.CWTnbr = clientweituobianhao;
                        swt.Qty_deal = 0;
                        swt.Status = TradeOrderStatus.New;
                        _rcvd_weituoList.Add(swt.CWTnbr, swt);
                        RaiseWeiTuo(new WeiTuoEventArgs(swt, DataChangeType.Add));
                        TradeDetail td = new TradeDetail();
                        if (this._tradeHistory.getTradeDetail(tradePoints.TradeGuid, ref td))
                        {
                            td.PolicyClosePoint = tradePoints.OutPoint;
                            if (td.TradeCloseOrderQty == 0)
                            {
                                td.TradeCloseOrderQty += (double)realqty;
                            }
                            td.TradeCloseOrderPrice = tradePoints.OutPoint.OpenPrice;
                            td.TradeCloseOrderType = tradePoints.OutPoint.OpenType;
                        }
                        if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Limit) //限价委托
                        {
                            this.AddNewKeCheMonitor(weituobianhao, clientweituobianhao, tradePoints.OutPoint.CancelLimitTime, tradePoints.OutPoint.OpenPrice, tradePoints.OutPoint.ReTradePriceType, PolicyTradeType.Close, securityInfo, tradePoints.OutPoint.OpenPrice, tradePoints.OutPoint.OpenQty, tradePoints.OutPoint.OpenType, false, tradePoints.TradeGuid, tradePoints.OutPoint.ReEnterPecentage, 0, policy, tradePoints.OutPoint.Maker, tradePoints.OutPoint.Leverage);
                        }
                        if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Market) //市价委托
                        {
                            //即时成交
                            MarketDetail md = new MarketDetail();
                            md.ClientBianHao = clientweituobianhao;
                            md.TradeGuid = td.Tradeid;
                            md.PolicyObject = policy;
                            md.TradeType = policyTradeType;
                            md.OpenQty = (double)realqty;
                            _monitoredMarketList.TryAdd(clientweituobianhao, md);
                        }
                        td.CloseWeiTuo.Add(new TradeWeiTuo(clientweituobianhao, tradePoints.OutPoint.OpenPrice, (double)realqty, tradePoints.OutPoint.OpenType));
                        if (this._acct.AutoXiaDan(tradePoints.OutPoint.OpenType, tradePoints.OutPoint.SecInfo.Code, tradePoints.OutPoint.FirstTradePriceType.ToString(), (float)tradePoints.OutPoint.OpenPrice, (double)realqty, tradePoints.OutPoint.SecInfo.Type, ref weituobianhao, clientweituobianhao, ref errorinfo, tradePoints.OutPoint.Maker, tradePoints.OutPoint.Leverage))
                        {
                            if (tradePoints.EnterPoint.FirstTradePriceType == TradeSendOrderPriceType.Limit && _monitoredKeCheList.ContainsKey(clientweituobianhao)) //限价委托
                            {
                                try
                                {
                                    _monitoredKeCheList[clientweituobianhao].BianHao = weituobianhao;
                                }
                                catch { }
                            }
                            policy.Notify(tradePoints.TradeGuid, OpenStatus.Close, (double)realqty, 0, clientweituobianhao);
                            if (!EnableCID)
                            {
                                _CID_WID.Add(weituobianhao, clientweituobianhao);
                            }
                            if (_history_weituoList.ContainsKey(weituobianhao))
                            {
                                if (_rcvd_weituoList.ContainsKey(clientweituobianhao))
                                {
                                    _rcvd_weituoList[clientweituobianhao].Update(_history_weituoList[weituobianhao]);
                                    RaiseWeiTuo(new WeiTuoEventArgs(_rcvd_weituoList[clientweituobianhao], DataChangeType.Change));
                                }
                                QueryKeChe(this, new List<StockWeiTuo>() { _history_weituoList[weituobianhao] });
                                _history_weituoList.Remove(weituobianhao);
                            }
                            ExportMessage(new MessageEventArgs(string.Format("{0}-{1}-平仓下单成功，委托编号{2}", securityInfo.Code, policyName, weituobianhao)));
                        }
                        else
                        {
                            td.Remark = errorinfo;
                            _rcvd_weituoList.Remove(clientweituobianhao);
                            if (tradePoints.OutPoint.ReTradePriceType == TradeSendOrderPriceType.Market)
                            {
                                MarketDetail md;
                                _monitoredMarketList.TryRemove(clientweituobianhao, out md);
                            }
                            if (tradePoints.OutPoint.ReTradePriceType == TradeSendOrderPriceType.Limit)
                            {
                                KeCheDetail kcd;
                                _monitoredKeCheList.TryRemove(clientweituobianhao, out kcd);
                            }
                            ExportMessage(new MessageEventArgs(string.Format("{0}-{2}平仓失败:{1},继续追单", tradePoints.EnterPoint.SecInfo.Code, errorinfo, policyName)));
                            StratReClose(securityInfo, policyName, tradePoints, policy, (double)realqty);
                        }
                    }
                    else
                    {
                        TradeDetail td = new TradeDetail();
                        if (this._tradeHistory.getTradeDetail(tradePoints.TradeGuid, ref td))
                        {
                            td.PolicyClosePoint = tradePoints.OutPoint;
                            td.TradeCloseOrderQty += (double)realqty;
                            td.TradeCloseOrderPrice = tradePoints.OutPoint.OpenPrice;
                            td.TradeCloseOrderType = tradePoints.OutPoint.OpenType;
                        }

                    }
                }
            }

        }


        private void StratReClose(SecurityInfo securityInfo, string policyName, TradePoints tradePoints, RunningPolicy policy, double qty)
        {

            ReoutObject ro = new ReoutObject();
            ro.securityInfo = securityInfo;
            ro.policyName = policyName;
            ro.tradePoints = tradePoints;
            ro.policy = policy;
            ro.qty = qty;
            ParameterizedThreadStart pstart = new ParameterizedThreadStart(FailReOut);
            Thread th = new Thread(pstart);
            th.Start(ro);

        }

        private void FailReOut(object obj)
        {
            ReoutObject ro = (ReoutObject)obj;
            TradeDetail td = new TradeDetail();
            while (true)
            {
                Thread.Sleep(500);
                if (this._tradeHistory.getTradeDetail(ro.tradePoints.TradeGuid, ref td))
                {
                    TickData tickdata = CurrentStockData.GetTick(ro.tradePoints.OutPoint.SecInfo);
                    OpenType ot = ro.tradePoints.OutPoint.OpenType;
                    double openPrice = 0;
                    if (ot == OpenType.Buy)
                    {
                        openPrice = tickdata.Bid;
                    }
                    else if (ot == OpenType.Sell)
                    {
                        openPrice = tickdata.Ask;
                    }
                    else if (ot == OpenType.PingDuo)
                    {
                        openPrice = tickdata.Ask;
                    }
                    else if (ot == OpenType.PingKong)
                    {
                        openPrice = tickdata.Bid;
                    }
                    string weituobianhao = string.Empty;
                    string errorinfo = string.Empty;
                    string clientweituobianhao = "EntropyRise" + DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
                    StockWeiTuo swt = new StockWeiTuo();
                    swt.CWTnbr = clientweituobianhao;
                    swt.Qty_deal = 0;
                    swt.Status = TradeOrderStatus.New;
                    _rcvd_weituoList.Add(swt.CWTnbr, swt);
                    RaiseWeiTuo(new WeiTuoEventArgs(swt, DataChangeType.Add));
                    td.CloseWeiTuo.Add(new TradeWeiTuo(clientweituobianhao, ro.tradePoints.OutPoint.OpenPrice, ro.qty, ro.tradePoints.OutPoint.OpenType));
                    if (ro.tradePoints.OutPoint.FirstTradePriceType == TradeSendOrderPriceType.Market) //五档剩撤单
                    {
                        MarketDetail md = new MarketDetail();
                        md.ClientBianHao = clientweituobianhao;
                        md.TradeGuid = td.Tradeid;
                        md.PolicyObject = ro.policy;
                        md.TradeType = PolicyTradeType.Close;
                        md.OpenQty = ro.qty;
                        _monitoredMarketList.TryAdd(clientweituobianhao, md);
                    }
                    else
                    {
                        this.AddNewKeCheMonitor(weituobianhao, clientweituobianhao, ro.tradePoints.OutPoint.CancelLimitTime, ro.tradePoints.EnterPoint.OpenPrice, ro.tradePoints.OutPoint.ReTradePriceType, PolicyTradeType.Close, ro.securityInfo, openPrice, ro.qty, ro.tradePoints.OutPoint.OpenType, false, ro.tradePoints.TradeGuid, ro.tradePoints.OutPoint.ReEnterPecentage, 0, ro.policy, ro.tradePoints.OutPoint.Maker, ro.tradePoints.OutPoint.Leverage);
                    }
                    if (this._acct.AutoXiaDan(ro.tradePoints.OutPoint.OpenType, ro.tradePoints.OutPoint.SecInfo.Code, ro.tradePoints.OutPoint.FirstTradePriceType.ToString(), (float)openPrice, ro.qty, ro.tradePoints.OutPoint.SecInfo.Type, ref weituobianhao, clientweituobianhao, ref errorinfo, ro.tradePoints.OutPoint.Maker, ro.tradePoints.OutPoint.Leverage))
                    {
                        if (ro.tradePoints.OutPoint.FirstTradePriceType == TradeSendOrderPriceType.Limit && _monitoredKeCheList.ContainsKey(clientweituobianhao)) //限价委托
                        {
                            try
                            {
                                _monitoredKeCheList[clientweituobianhao].BianHao = weituobianhao;
                            }
                            catch { }
                        }
                        ro.policy.Notify(ro.tradePoints.TradeGuid, OpenStatus.Close, ro.qty, 0, clientweituobianhao);

                        if (!EnableCID)
                        {
                            _CID_WID.Add(weituobianhao, clientweituobianhao);
                        }
                        if (_history_weituoList.ContainsKey(weituobianhao))
                        {
                            if (_rcvd_weituoList.ContainsKey(clientweituobianhao))
                            {
                                _rcvd_weituoList[clientweituobianhao].Update(_history_weituoList[weituobianhao]);
                                RaiseWeiTuo(new WeiTuoEventArgs(_rcvd_weituoList[clientweituobianhao], DataChangeType.Change));
                            }
                            QueryKeChe(this, new List<StockWeiTuo>() { _history_weituoList[weituobianhao] });
                            _history_weituoList.Remove(weituobianhao);
                        }
                        return;
                    }
                    else
                    {
                        _rcvd_weituoList.Remove(clientweituobianhao);
                        if (ro.tradePoints.OutPoint.ReTradePriceType == TradeSendOrderPriceType.Market)
                        {
                            MarketDetail md;
                            _monitoredMarketList.TryRemove(clientweituobianhao, out md);
                        }
                        if (ro.tradePoints.OutPoint.ReTradePriceType == TradeSendOrderPriceType.Limit)
                        {
                            KeCheDetail kcd;
                            _monitoredKeCheList.TryRemove(clientweituobianhao, out kcd);
                        }
                        td.Remark = errorinfo;
                        ExportMessage(new MessageEventArgs(string.Format("{0}-{2}第{3}次平仓失败:{1},继续追单", ro.tradePoints.EnterPoint.SecInfo.Code, errorinfo, ro.policyName, ro.count)));
                        ro.count++;
                    }
                }
            }
        }


        private void AddNewKeCheMonitor(string weituobianhao, string clientweituobianhao, int cancelLimitTime, double markPrice, TradeSendOrderPriceType reEnterPriceType, PolicyTradeType policyTradeType, SecurityInfo securityInfo, double orderprice, double orderQty, OpenType opentype, bool notified, Guid tradeguid, double reEnterPercentage, int cancelTimes, Object runningPolicy, bool maker, string leverage)
        {
            KeCheDetail kcd = new KeCheDetail();
            kcd.BianHao = weituobianhao;
            kcd.ClientBianHao = clientweituobianhao;
            kcd.CreateTime = System.DateTime.Now.TimeOfDay;
            kcd.WaitSecond = cancelLimitTime;
            kcd.MarkPrice = markPrice;
            kcd.TradeType = policyTradeType;
            kcd.Si = securityInfo;
            //kcd.UnDealedQty = orderQty;
            kcd.Dealed = false;
            kcd.ReEnterPriceType = reEnterPriceType;
            //kcd.Monitored = false;
            kcd.OrderPrice = orderprice;
            kcd.OpenType = opentype;
            kcd.TradeGuid = tradeguid;
            kcd.PolicyObject = runningPolicy;
            kcd.Notified = false;
            kcd.OrderQty = orderQty;
            kcd.ForceToCancel = false;
            kcd.Maker = maker;
            kcd.Leverage = leverage;
            ////////////////////////////////////////////////////////////>交易所ID， 上海1，深圳0(招商证券普通账户深圳是2)</param>
            kcd.ExchangeID = securityInfo.Market.ToString();
            kcd.Notified = notified;
            kcd.ReEnterPercentage = reEnterPercentage;
            kcd.CancelTimes = cancelTimes;
            /////////////////////////////////////////////////////////////
            this._monitoredKeCheList.TryAdd(clientweituobianhao, kcd);// allWeiTuo.Add(wtm);
        }

        public void UpdateDisplay(Guid guid)
        {
            TradeDetail td = new TradeDetail();
            if (_tradeHistory.getTradeDetail(guid, ref td))
            {
                RaiseTradeDetailUpdate(new TradeDetailUpdateEventArgs(td));
            }
        }

        public bool ManualXiaDan(string tradeCategory, string priceType, string code, float price, double qty, ref string weituobianhao, ref string errmsg, string symbolType, bool maker, string leverage = "10")
        {
            string clientweituobianhao = "EntropyRise" + DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
            StockWeiTuo swt = new StockWeiTuo();
            swt.CWTnbr = clientweituobianhao;
            swt.Qty_deal = 0;
            swt.Status = TradeOrderStatus.New;
            _rcvd_weituoList.Add(swt.CWTnbr, swt);
            RaiseWeiTuo(new WeiTuoEventArgs(swt, DataChangeType.Add));
            if (_acct.ManualXiaDan(tradeCategory, priceType, code, price, qty, ref weituobianhao, symbolType, clientweituobianhao, ref errmsg, maker, leverage))
            {
                if (!EnableCID)
                {
                    _CID_WID.Add(weituobianhao, clientweituobianhao);
                }
                if (_history_weituoList.ContainsKey(weituobianhao))
                {
                    if (_rcvd_weituoList.ContainsKey(clientweituobianhao))
                    {
                        _rcvd_weituoList[clientweituobianhao].Update(_history_weituoList[weituobianhao]);
                        RaiseWeiTuo(new WeiTuoEventArgs(_rcvd_weituoList[clientweituobianhao], DataChangeType.Change));
                    }
                    QueryKeChe(this, new List<StockWeiTuo>() { _history_weituoList[weituobianhao] });
                    _history_weituoList.Remove(weituobianhao);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setQueryStock(StockData.SecurityInfo securityInfo)
        {
            this._acct.SetQuery5(securityInfo);
        }

        public void clearQueryStock()
        {
            this._acct.ClearQuery5();
        }

        public bool TryDisConnect()
        {
            this._fullStop = true;

            try
            {
                this._thread_Connect.Abort();// ConnectThread.Abort();
            }
            catch { }
            ExportMessage(new MessageEventArgs(string.Format("账号:{0} 已手工断开连接", this._acct.AccountID)));
            this._acct.ClientID = -1;
            return true;
        }

        public void updateOnHandStock(SecurityInfo securityInfo, int qty)
        {
            _onHandDetail.ResetQty(securityInfo, qty);
        }
    }
}
public enum ReEnterStatus
{
    Drop,
    Need,
    Keep,
    Abadon
}

public class ReoutObject
{
    //SecurityInfo securityInfo, string policyName, TradePoints tradePoints, RunningPolicy policy,int qty
    public SecurityInfo securityInfo;
    public string policyName;
    public TradePoints tradePoints;
    public RunningPolicy policy;
    public double qty;
    public int count;
    public ReoutObject()
    {
        count = 1;
    }
}