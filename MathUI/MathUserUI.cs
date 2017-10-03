using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwim;

namespace MathUI
{

    public class MathUserUI
    {

        public class MyEventParam : EventArgs{
            public Dictionary<String, RTDQuote> MQ { get; set; }
            public MyEventParam(Dictionary<String, RTDQuote> mq){
                MQ = new Dictionary<String, RTDQuote>(mq);
            }
        }
        public event EventHandler<MyEventParam> NewQuoteAdded;
        public void OnNewQuoteAdded(Dictionary<string, RTDQuote> mQuote){
            var QuoteAdded = NewQuoteAdded;
            if (QuoteAdded != null) QuoteAdded(this, new MyEventParam(mQuote));
        }


        private int IncD = 0, iNumofD=0;
        private Object tsLock = new Object();
        public Boolean LoopEnd, debug;
        public Client CLt;

        public struct RTDQuote
        {
            public string symbol;
            public double last;
            public double last_size;
            public double bid;
            public double ask;
            public double volume;
            public double volumeDelta;
            public double open;
            public double high;
            public double low;
            public double bid_size;
            public double ask_size;
            public double close;
            public double delta;
            public double extrinsic;
            public double intrinsic;
            public double impl_vol;
            public double net_change;
            public double percent_change;
            public double open_int;
            public double mark;
            public double shares;
            public DateTime TimeRT;
            public int SeqNo;
        }

        private Dictionary<String,List<RTDQuote>> UserQuoteDList = new Dictionary<String,List<RTDQuote>>();
        public  Dictionary<String,List<RTDQuote>> MQuoteDList    = new Dictionary<String,List<RTDQuote>>();
        public List<string> SList;
        
        public MathUserUI(Byte [] SinB,Boolean debug = false)
        {
            this.debug = debug;

            String SSt = Encoding.ASCII.GetString(SinB);
            SList = new List<string>(SSt.Split(',',';',' ','+'));
            //SList.Add("TSLA");SList.Add(".TSLA180119C320");

            CLt = new Client();
            foreach (var Sb in SList)
            {
                Add(Sb, "Last");
                Add(Sb, "Mark");
                Add(Sb, "Volume");
                Add(Sb, "Last_Size");
                Add(Sb, "Bid");
                Add(Sb, "Ask");
                Add(Sb, "Bid_Size");
                Add(Sb, "Ask_Size");
                MQuoteDList.Add(Sb, new List<RTDQuote>());
                UserQuoteDList.Add(Sb, new List<RTDQuote>());
                UserQuote = new RTDQuote();
                UserQuote.symbol = Sb; UserQuote.SeqNo = 0; UserQuote.volume = 0;
                UserQuoteD.Add(Sb, UserQuote);
                ovolume.Add(Sb, 0);
            }
        }

        public async Task<int> StartQuoteLoop()
        {
            LoopEnd = false;
            bool restart = true;

            while (restart) {

                var TaskRes = await Task.Run(() => QuoteLoop());

                if(TaskRes==1){
                    restart = false;
                    LoopEnd = true;
                }else{
                    restart = true;
                    //CLt.feed.Disconnect();
                    CLt.Dispose();
                    CLt = new Client();
                    foreach (var Sb in SList)
                    {
                        Add(Sb, "Last");
                        Add(Sb, "Mark");
                        Add(Sb, "Volume");
                        Add(Sb, "Last_Size");
                        Add(Sb, "Bid");
                        Add(Sb, "Ask");
                        Add(Sb, "Bid_Size");
                        Add(Sb, "Ask_Size");
                    }
                }
            }
            return 1;
        }

        Dictionary<String, RTDQuote> UserQuoteD = new Dictionary<String, RTDQuote>();
        Dictionary<String, double> ovolume =new Dictionary<String,double>();

        RTDQuote UserQuote;
        private int QuoteLoop()
        {
            try
            {
                foreach (var quote in CLt.Quotes())
                {
                    String idxstr = quote.Symbol;
                    UserQuote = UserQuoteD[idxstr];

                    ovolume[idxstr] = UserQuote.volume;
                    switch (quote.Type)
                    {
                        case "Last":
                            UserQuote.last = Convert.ToDouble(quote.Value);
                            break;
                        case "Last_Size":
                            UserQuote.last_size = Convert.ToDouble(quote.Value);
                            break;
                        case "Bid":
                            UserQuote.bid = Convert.ToDouble(quote.Value);
                            break;
                        case "Ask":
                            UserQuote.ask = Convert.ToDouble(quote.Value);
                            break;
                        case "Volume":
                            UserQuote.volume = Convert.ToDouble(quote.Value);
                            break;
                        case "Open":
                            UserQuote.open = Convert.ToDouble(quote.Value);
                            break;
                        case "High":
                            UserQuote.high = Convert.ToDouble(quote.Value);
                            break;
                        case "Low":
                            UserQuote.low = Convert.ToDouble(quote.Value);
                            break;
                        case "Bid_Size":
                            UserQuote.bid_size = Convert.ToDouble(quote.Value);
                            break;
                        case "Ask_Size":
                            UserQuote.ask_size = Convert.ToDouble(quote.Value);
                            break;
                        case "Close":
                            UserQuote.close = Convert.ToDouble(quote.Value);
                            break;
                        case "Delta":
                            UserQuote.delta = Convert.ToDouble(quote.Value);
                            break;
                        case "Extrinsic":
                            UserQuote.extrinsic = Convert.ToDouble(quote.Value);
                            break;
                        case "Intrinsic":
                            UserQuote.intrinsic = Convert.ToDouble(quote.Value);
                            break;
                        case "Impl_Vol":
                            UserQuote.impl_vol = Convert.ToDouble(quote.Value);
                            break;
                        case "Net_Change":
                            UserQuote.net_change = Convert.ToDouble(quote.Value);
                            break;
                        case "Percent_Change":
                            UserQuote.percent_change = Convert.ToDouble(quote.Value);
                            break;
                        case "Open_Int":
                            UserQuote.open_int = Convert.ToDouble(quote.Value);
                            break;
                        case "Mark":
                            UserQuote.mark = Convert.ToDouble(quote.Value);
                            break;
                        case "Shares":
                            UserQuote.shares = Convert.ToDouble(quote.Value);
                            break;
                        default:
                            break;
                    }
                    UserQuoteD[idxstr] = UserQuote;

                    iNumofD++;
                    //if (debug) Console.WriteLine("Inside QuoteLoop:iNumofD={0}", iNumofD);
                    if (debug) Debug.Print("Inside QuoteLoop:iNumofD={0}", iNumofD);
                    if (UserQuote.volume>ovolume[idxstr]){
                        lock (tsLock)
                        {
                            IncD++;
                            UserQuote.TimeRT = DateTime.Now;
                            if (UserQuote.ask == 0) UserQuote.ask = UserQuote.mark;
                            if (UserQuote.bid == 0) UserQuote.bid = UserQuote.mark;
                            UserQuote.volumeDelta = UserQuote.volume - ovolume[idxstr];
                            var LinU = UserQuoteDList[idxstr];

                            UserQuote.SeqNo = LinU.Count + 1;
                            LinU.Add(UserQuote);

                            var QuoteAdded = NewQuoteAdded;
                            if (QuoteAdded != null) QuoteAdded(this, new MyEventParam(UserQuoteD));
                        }
                    }
                }
                return 1;
            }
            catch
            {
                //if (debug) Console.WriteLine("TimeOut Exception: Exiting QuoteLoop");
                if (debug) Debug.Print("TimeOut Exception: Exiting QuoteLoop");
                return 0;
            }
        }

        public void GetQuote()
        {
            lock (tsLock){
                if (IncD > 0){
                    foreach (var Sm in SList){
                        var UQ = UserQuoteDList[Sm];
                        var MQ = MQuoteDList[Sm];
                        var Cu = UQ.Count;
                        var Cm = MQ.Count;
                        if (Cu > Cm)
                            try{
                                for (int i = Cm + 1; i <= Cu; i++)MQ.Add(UQ[i - 1]);
                            }catch { };
                    }
                }
                IncD = 0;
            }
        }

        public void Add(String Sm="TSLA",String St = "Mark")
        {
            switch (St)
            {
                case "Last":
                    CLt.Add(Sm,QuoteType.Last);
                    break;
                case "Last_Size":
                    CLt.Add(Sm, QuoteType.Last_Size);
                    break;
                case "Bid":
                    CLt.Add(Sm, QuoteType.Bid);
                    break;
                case "Ask":
                    CLt.Add(Sm, QuoteType.Ask);
                    break;
                case "Volume":
                    CLt.Add(Sm, QuoteType.Volume);
                    break;
                case "Open":
                    CLt.Add(Sm, QuoteType.Open);
                    break;
                case "High":
                    CLt.Add(Sm, QuoteType.High);
                    break;
                case "Low":
                    CLt.Add(Sm, QuoteType.Low);
                    break;
                case "Bid_Size":
                    CLt.Add(Sm, QuoteType.Bid_Size);
                    break;
                case "Ask_Size":
                    CLt.Add(Sm, QuoteType.Ask_Size);
                    break;
                case "Close":
                    CLt.Add(Sm, QuoteType.Close);
                    break;
                case "Delta":
                    CLt.Add(Sm, QuoteType.Delta);
                    break;
                case "Extrinsic":
                    CLt.Add(Sm, QuoteType.Extrinsic);
                    break;
                case "Intrinsic":
                    CLt.Add(Sm, QuoteType.Intrinsic);
                    break;
                case "Impl_Vol":
                    CLt.Add(Sm, QuoteType.Impl_vol);
                    break;
                case "Net_Change":
                    CLt.Add(Sm, QuoteType.Net_Change);
                    break;
                case "Percent_Change":
                    CLt.Add(Sm, QuoteType.Percent_Change);
                    break;
                case "Open_Int":
                    CLt.Add(Sm, QuoteType.Open_Int);
                    break;
                case "Mark":
                    CLt.Add(Sm, QuoteType.Mark);
                    break;
                case "Shares":
                    CLt.Add(Sm, QuoteType.Mark);
                    break;
                default:
                    break;
            }
        }
    }
}
