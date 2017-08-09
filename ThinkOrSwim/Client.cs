using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwim
{
    public enum QuoteType
    {
        Last,
        Last_Size,
        Bid,
        Ask,
        Volume,
        Open,
        High,
        Low,
        Bid_Size,
        Ask_Size,
        Close,
        Delta,
        Extrinsic,
        Intrinsic,
        Impl_vol,
        Net_Change,
        Percent_Change,
        Open_Int,
        Mark
    }

    public class Client : IDisposable
    {
        Feed feed;

        private int iNumofD = 0, IncD = 0,TotNum=0;
        public int MathNumofD = 0;
        private Object tsLock = new Object();
        public Boolean LoopEnd,debug;

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
        }

        private List<RTDQuote> UserQuoteList = new List<RTDQuote>();
        public List<RTDQuote> MathUserQuoteList = new List<RTDQuote>();
        private RTDQuote UserQuote;

        public Client(Boolean debug=false) : this(10)
        {
            this.debug = debug;
        }

        public Client(int heartbeat)
        {
            this.feed = new Feed(heartbeat);
        }

        public void Add(string symbol, QuoteType quoteType)
        {
            this.feed.Add(symbol, quoteType.ToString());
        }

        public void Remove(string symbol, QuoteType quoteType)
        {
            this.feed.Remove(symbol, quoteType.ToString());
        }

        public IEnumerable<Quote> Quotes()
        {
            return this.feed;
        }

        public void Dispose()
        {
            this.feed.Stop();
        }

        public async Task<int> StartQuoteLoop()
        {
            LoopEnd = false;
            int TaskRes=await Task.Run(() => QuoteLoop());
            LoopEnd = true;
            return TaskRes;
        }

        private int QuoteLoop()
        {
            try
            {
                foreach (var quote in this.Quotes())
                { 
                    UserQuote.symbol = quote.Symbol;
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
                        default:
                            break;
                    }
                    iNumofD++;
                    if (debug) Console.WriteLine("Inside QuoteLoop:iNumofD={0}", iNumofD);
                    if (iNumofD > 2 * sizeof(QuoteType))
                    {
                        lock (tsLock) 
                        {
                            IncD++; TotNum = iNumofD;
                            UserQuoteList.Add(UserQuote);
                        }
                    }
                }
                return 1;
            } catch
            {
                if (debug) Console.WriteLine("Exiting QuoteLoop");
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
            MathUserQuoteList = new List<RTDQuote>();
            int StotN=0;
            lock (tsLock)
            {
                for (int i = 0; i< IncD; i++) {
                    MathUserQuoteList.Add(UserQuoteList[i]);
                }
                MathNumofD = IncD;
                IncD = 0;
                UserQuoteList = new List<RTDQuote>();
                StotN = TotNum;
            }
            return StotN;
        }
    }
}
