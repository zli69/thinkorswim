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
            var CLt = new Client();

            List<string> SList = new List<string>();

            SList.Add("TSLA"); SList.Add(".TSLA180119C320");

            foreach (var Sb in SList)
            {
                CLt.Add(Sb, QuoteType.Last);
                CLt.Add(Sb, QuoteType.Mark);
                CLt.Add(Sb, QuoteType.Volume);
                CLt.Add(Sb, QuoteType.Last_Size);
                CLt.Add(Sb, QuoteType.Bid);
                CLt.Add(Sb, QuoteType.Ask);
                CLt.Add(Sb, QuoteType.Bid_Size);
                CLt.Add(Sb, QuoteType.Ask_Size);
            }

            foreach (var quote in CLt.Quotes())
            {
                Console.WriteLine("{0} {1}: ${2}", quote.Symbol, quote.Type, quote.Value);
            }

        }
    }
}
