using StockData;
using System;
using System.Data;

namespace StockTrade
{
    public enum ResultFromat
    {
        /// <summary>
        /// 账号
        /// </summary>
        ZhangHao,
        /// <summary>
        /// 资金
        /// </summary>
        ZiJing,
        /// <summary>
        /// 当日委托
        /// </summary>
        DangRiWeiTuo,
        /// <summary>
        /// 当日成交
        /// </summary>
        DangRiChengJiao,
        /// <summary>
        /// 持仓
        /// </summary>
        ChiCang,
        /// <summary>
        /// 可撤清单
        /// </summary>
        KeChe,
        /// <summary>
        /// 下单
        /// </summary>
        XiaDan,
    }

    public class QueryResultEventArgs
    {
        DataTable _result;

        public DataTable ResultTable
        {
            get { return _result; }
            set { _result = value; }
        }

        ResultFromat _format;

        public ResultFromat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public QueryResultEventArgs(DataTable table, ResultFromat format)
        {
            this._format = format;
            this._result = table;
        }
    }

    public class MessageEventArgs
    {
        string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public MessageEventArgs(string message)
        {
            this._message = message;
        }
    }

    public class PolicyNotifyEventArgs
    {
        object policy;

        public object Policy
        {
            get { return policy; }
            set { policy = value; }
        }
        Guid tradeGuid;

        public Guid TradeGuid
        {
            get { return tradeGuid; }
            set { tradeGuid = value; }
        }
        OpenStatus openStatus;

        public OpenStatus OpenStatus
        {
            get { return openStatus; }
            set { openStatus = value; }
        }
        public PolicyNotifyEventArgs(object policy, Guid guid, OpenStatus openstatus)
        {
            this.policy = policy;
            this.tradeGuid = guid;
            this.openStatus = openstatus;
        }
    }

    public class TradeDetailUpdateEventArgs
    {
        TradeDetail tDetail;

        public TradeDetail TDetail
        {
            get { return tDetail; }
            set { tDetail = value; }
        }
        public TradeDetailUpdateEventArgs(TradeDetail td)
        {
            this.tDetail = td;
        }
    }

    public class Quote5EventArgs
    {
        TickData tick;

        public TickData Tick
        {
            get { return tick; }
            set { tick = value; }
        }
        public Quote5EventArgs(TickData t)
        {
            this.tick = t;
        }
    }
}
