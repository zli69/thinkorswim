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
            var client = new Client();
            //client.Add("SPX", QuoteType.Last);
            client.Add("TSLA", QuoteType.Last);
            //client.Add("TSLA", QuoteType.Bid); client.Add("TSLA", QuoteType.Ask);
            //client.Add("BABA", QuoteType.Last);
            //client.Add("AAPL", QuoteType.MT_News);
            client.Add("TSLA", QuoteType.Mark);
            client.Add("TSLA", QuoteType.Volume);
            client.Add("TSLA", QuoteType.Last_Size);
            client.Add("TSLA", QuoteType.Bid); client.Add("TSLA", QuoteType.Ask);
            client.Add("TSLA", QuoteType.Bid_Size); client.Add("TSLA", QuoteType.Ask_Size);
            //client.Add(".TSLA180119C320", QuoteType.Last);


            client.StartQuoteLoop();
            while (true)
            {
                client.GetQuote();
                int ToTNum = client.MathNumofD;
                int Start = ToTNum - client.IncD;
                for (int i = 0; i < client.IncD; i++)
                {
                    string Dsym = client.MathUserQuoteList[Start + i].symbol;
                    double Dmark = client.MathUserQuoteList[Start + i].mark;
                    double Dvol = client.MathUserQuoteList[Start + i].volume;
                    double Dbid = client.MathUserQuoteList[Start + i].bid;
                    double Dask = client.MathUserQuoteList[Start + i].ask;
                    Console.WriteLine("{0}: $m{1},v{2},$b{3},$a{4}", Dsym, Dmark, Dvol, Dbid, Dask);
                }
            }



            //foreach (var quote in client.Quotes())
            //{
            //    Console.WriteLine("{0} {1}: ${2}", quote.Symbol, quote.Type, quote.Value);
            //}

        }
    }
}
