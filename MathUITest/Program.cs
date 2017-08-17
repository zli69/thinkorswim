using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathUI;
using ThinkOrSwim;
using System.Threading;

namespace MathUITest
{
    class Program
    {
        static void Main(string[] args)
        {
            String SSt = "TSLA,.TSLA180119C320,NVDA,.NVDA180119C130";
            Byte[] SSb = Encoding.ASCII.GetBytes(SSt);
            var MU = new MathUserUI(SSb,true);


            //MU.Add("TSLA", "Last");
            //MU.Add("TSLA", "Mark");
            //MU.Add("TSLA", "Volume");
            //MU.Add("TSLA", "Last_Size");
            //MU.Add("TSLA", "Bid"); MU.Add("TSLA", "Ask");
            //MU.Add("TSLA", "Bid_Size"); MU.Add("TSLA", "Ask_Size");


            //MU.Add(".TSLA180119C320", "Last");

            //******************************************************************

            Task<int> TaskRes = MU.StartQuoteLoop();
            int i = 0;
            while (!MU.LoopEnd)
            {
                i++;
                int TotNum = MU.GetQuote();
                int IncNum = MU.MathNumofD;
                if (IncNum == 0) Thread.Sleep(10000);
                foreach (var sbQ in MU.MathUserQuoteList )
                {
                    string Dsym  = sbQ.symbol;
                    double Dmark = sbQ.mark;
                    double Dvol  = sbQ.volume;
                    double Dbid  = sbQ.bid;
                    double Dask  = sbQ.ask;
                    DateTime TimeN = DateTime.Now; String TimeS = TimeN.ToString();
                    Console.WriteLine("{0}: $m{1},v{2},$b{3},$a{4} ({5}/{6})@Time:{7}", Dsym, Dmark, Dvol, Dbid, Dask, i, TotNum, TimeS);
                }
            }

            TaskRes.Wait();
            int SynRes = TaskRes.Result;
            Console.WriteLine("Task StartQuoteLoop Ends with Result:{0}", SynRes);

            //*******************************************************************
        }
    }
}
