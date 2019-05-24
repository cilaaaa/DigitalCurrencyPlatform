using StockData;
using System.Collections.Generic;
using System.Linq;

namespace StockPolicies
{
    public class RiscPoints
    {
        List<RiscPoint> riscPointList;

        public List<RiscPoint> RiscPointList
        {
            get { return riscPointList; }
            set { riscPointList = value; }
        }

        AbsoluteBarType preBarType;

        public AbsoluteBarType PreBarType
        {
            get { return preBarType; }
            set { preBarType = value; }
        }

        double preOpen;

        public double PreOpen
        {
            get { return preOpen; }
            set { preOpen = value; }
        }
        double preClose;

        public double PreClose
        {
            get { return preClose; }
            set { preClose = value; }
        }


        public RiscPoints()
        {
            riscPointList = new List<RiscPoint>();
            preOpen = 0;
            preClose = 0;
        }



        public void InsertBar(LiveBar liveBar)
        {
            if (preOpen == 0)
            {
                preOpen = liveBar.Open;
                preClose = liveBar.Close;
                this.preBarType = liveBar.AbsoluteRaiseType;
                if (this.preBarType == AbsoluteBarType.Even)
                {
                    this.preBarType = AbsoluteBarType.Raise;
                }
                riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Open));
                riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Close));

            }
            else
            {
                if (this.preBarType == AbsoluteBarType.Raise)
                {
                    if (liveBar.RelativeRaiseType == BarType.RealRaise || liveBar.RelativeRaiseType == BarType.FakeDecent)
                    {
                        riscPointList.Last().Time = liveBar.BarOpenTime;
                        riscPointList.Last().Price = liveBar.Close;
                        if (liveBar.RelativeRaiseType == BarType.RealRaise)
                        {
                            this.preOpen = liveBar.Open;
                            this.preClose = liveBar.Close;
                        }
                    }
                    else if (liveBar.RelativeRaiseType == BarType.RealDecent)
                    {
                        if (liveBar.Close >= preOpen)
                        {
                            riscPointList.Last().Time = liveBar.BarOpenTime;
                            riscPointList.Last().Price = liveBar.Open > preClose ? liveBar.Open : preClose;
                        }
                        else
                        {
                            riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Close));
                            this.preOpen = liveBar.Open;
                            this.PreClose = liveBar.Close;
                            this.preBarType = AbsoluteBarType.Decent;
                        }
                    }
                    else if (liveBar.RelativeRaiseType == BarType.FakeRaise)
                    {
                        if (liveBar.Close < preOpen)
                        {
                            riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Close));
                            this.preOpen = liveBar.Open;
                            this.preClose = liveBar.Close;
                            this.preBarType = AbsoluteBarType.Decent;
                        }
                        else
                        {
                            riscPointList.Last().Time = liveBar.BarOpenTime;
                            riscPointList.Last().Price = liveBar.Close;
                        }
                    }
                    else
                    {
                        riscPointList.Last().Time = liveBar.BarOpenTime;
                        riscPointList.Last().Price = liveBar.Close;
                    }

                }
                else if(this.preBarType == AbsoluteBarType.Decent)
                {
                    if (liveBar.RelativeRaiseType == BarType.RealDecent || liveBar.RelativeRaiseType == BarType.FakeRaise)
                    {
                        riscPointList.Last().Time = liveBar.BarOpenTime;
                        riscPointList.Last().Price = liveBar.Close;
                        if (liveBar.RelativeRaiseType == BarType.RealDecent)
                        {
                            this.preOpen = liveBar.Open;
                            this.preClose = liveBar.Close;
                        }
                    }
                    else if (liveBar.RelativeRaiseType == BarType.FakeDecent)
                    {
                        if (liveBar.Close > preClose)
                        {
                            riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Close));
                            this.preOpen = liveBar.Open;
                            this.preClose = liveBar.Close;
                            this.preBarType = AbsoluteBarType.Raise;
                        }
                        else
                        {
                            riscPointList.Last().Time = liveBar.BarOpenTime;
                            riscPointList.Last().Price = liveBar.Close;
                        }
                    }
                    else if (liveBar.RelativeRaiseType == BarType.RealRaise)
                    {
                        if (liveBar.Close > this.preOpen)
                        {
                            riscPointList.Add(new RiscPoint(liveBar.BarOpenTime, liveBar.Close));
                            this.preOpen = liveBar.Open;
                            this.preClose = liveBar.Close;
                            this.preBarType = AbsoluteBarType.Raise;
                        }
                        else
                        {
                            riscPointList.Last().Time = liveBar.BarOpenTime;
                            riscPointList.Last().Price = liveBar.Open > preClose ? preClose : liveBar.Open;
                        }
                    }
                }
                else
                {
                    riscPointList.Last().Time = liveBar.BarOpenTime;
                }
            }
        }
    }
}
