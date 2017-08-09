using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwim;

namespace ConsoleQuotes
{
    class Program
    {
        static void Main(string[] args)
        {
            var CLt = new Client(true);
            //client.Add("SPX", QuoteType.Last);
            CLt.Add("TSLA", QuoteType.Last);
            //client.Add("TSLA", QuoteType.Bid); client.Add("TSLA", QuoteType.Ask);
            //client.Add("BABA", QuoteType.Last);
            //client.Add("AAPL", QuoteType.MT_News);
            CLt.Add("TSLA", QuoteType.Mark);
            CLt.Add("TSLA", QuoteType.Volume);
            CLt.Add("TSLA", QuoteType.Last_Size);
            CLt.Add("TSLA", QuoteType.Bid); CLt.Add("TSLA", QuoteType.Ask);
            CLt.Add("TSLA", QuoteType.Bid_Size); CLt.Add("TSLA", QuoteType.Ask_Size);
            //client.Add(".TSLA180119C320", QuoteType.Last);

            //List<Client.RTDQuote> QL = new List<Client.RTDQuote>();





            //******************************************************************

            //Task<int> TaskRes=CLt.StartQuoteLoop();

            //while (!CLt.LoopEnd)
            //{
            //    int TotNum=CLt.GetQuote();
            //    int IncNum = CLt.MathNumofD;
            //    for (int i = 0; i < IncNum; i++)
            //    {
            //        string Dsym = CLt.MathUserQuoteList[i].symbol;
            //        double Dmark = CLt.MathUserQuoteList[i].mark;
            //        double Dvol = CLt.MathUserQuoteList[i].volume;
            //        double Dbid = CLt.MathUserQuoteList[i].bid;
            //        double Dask = CLt.MathUserQuoteList[i].ask;
            //        DateTime TimeN=DateTime.Now;String TimeS = TimeN.ToString();
            //        Console.WriteLine("{0}: $m{1},v{2},$b{3},$a{4} ({5}/{6})@Time:{7}", Dsym, Dmark, Dvol, Dbid, Dask,i,TotNum,TimeS);
            //    }
            //}

            //TaskRes.Wait();
            //int SynRes = TaskRes.Result;
            //Console.WriteLine("Task StartQuoteLoop Ends with Result:{0}",SynRes);

            //*******************************************************************





            foreach (var quote in CLt.Quotes())
            {
                Console.WriteLine("{0} {1}: ${2}", quote.Symbol, quote.Type, quote.Value);
            }

        }
    }
}
