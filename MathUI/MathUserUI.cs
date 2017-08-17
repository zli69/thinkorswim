using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwim;

namespace MathUI
{
    public class MathUserUI
    {
        private int iNumofD = 0, IncD = 0, TotNum = 0;
        public int MathNumofD = 0,MathNumofS = 0;
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
        }

        private List<Dictionary<String,RTDQuote>> UserQuoteDList = new List<Dictionary<String,RTDQuote>>();
        //public List<Dictionary<String, RTDQuote>> MathUserQuoteDList = new List<Dictionary<String,RTDQuote>>();
        public List<RTDQuote> MathUserQuoteList = new List<RTDQuote>();
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
                    return TaskRes;

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
            return 0;
        }

        //A dictionary containing the latest quote data for all requested symbols.
        Dictionary<String, RTDQuote> UserQuoteD = new Dictionary<String, RTDQuote>(); 
        private int QuoteLoop()
        {
            RTDQuote UserQuote;

            try
            {
                foreach (var quote in CLt.Quotes())
                {
                    String idxstr = quote.Symbol;
                    if (!UserQuoteD.ContainsKey(idxstr))
                    {
                        UserQuoteD.Add(idxstr, new RTDQuote());
                        UserQuote = new RTDQuote();
                        UserQuote.symbol = idxstr;
                    } else
                    {
                        UserQuote = UserQuoteD[idxstr];
                    }

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
                    if (debug) Console.WriteLine("Inside QuoteLoop:iNumofD={0}", iNumofD);
                    if (iNumofD > 2 * sizeof(QuoteType)*UserQuoteD.Count)
                    {
                        lock (tsLock)
                        {
                            IncD++; TotNum = iNumofD;
                            UserQuoteDList.Add(UserQuoteD);
                        }
                    }
                }
                return 1;
            }
            catch
            {
                if (debug) Console.WriteLine("TimeOut Exception: Exiting QuoteLoop");
                return 0;
            }
        }

        public int GetQuote()
        {
            //if (debug)
            //{
            //    DateTime TimeN = DateTime.Now; String TimeS = TimeN.ToString();
            //    Console.WriteLine("Inside GetQuote@{0}",TimeS);
            //}
            int StotN = 0;
            lock (tsLock)
            {
                //MathUserQuoteDList = new List<Dictionary<String, RTDQuote>>();
                MathUserQuoteList = new List<RTDQuote>();

                //for (int i = 0; i < IncD; i++)
                //{
                //    MathUserQuoteDList.Add(UserQuoteDList[i]);
                //}
                MathNumofD = IncD;
                if (MathNumofD > 0)
                {
                    var DLast = new Dictionary<String, RTDQuote>(UserQuoteDList[MathNumofD - 1]);
                    MathNumofS = DLast.Count;
                    foreach (var Sb in SList)
                    {
                        foreach (KeyValuePair<String, RTDQuote> kvp in DLast)
                        {
                            if(kvp.Key==Sb)MathUserQuoteList.Add(kvp.Value);
                        }
                    }
                }
                IncD = 0;
                UserQuoteDList = new List<Dictionary<String, RTDQuote>>();
                StotN = TotNum;
            }
            return StotN;
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
