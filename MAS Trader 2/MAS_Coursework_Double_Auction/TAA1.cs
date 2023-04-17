using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using ActressMas;
using MAS_Algo_Trader;

namespace MAS_Coursework_Double_Auction
{
    public class TAA1 : Agent
    {
        private static readonly APIReply apiReply = new APIReply();

        public TAA1()
        {

        }

        public override void Setup()
        {
            Send("coordinatorAgent", $"registerTAA");
        }

        public override void Act(Message message)   //Program enters here after it receives a message
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);  //parsing the data coming from other agents into the action and a list of parameters
                switch (action)
                {
                    case "start":
                        Console.WriteLine("Initialising SMA calcuation for: " + parameters[0]);

                        HandleAnalysis(parameters[0], Convert.ToDateTime(parameters[1]));
                        break;

                    case "stop":
                        Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleAnalysis(string stock, DateTime startDate)
        {

            var resultFA = HandleFA(stock);
            double FAProbabilty = resultFA.Item1;
            int FASignal = resultFA.Item2;

            var resultTA = HandleSMA(stock,startDate);
            double TAProbabilty = resultTA.Item1;
            int TASignal = resultTA.Item2;

            //Weight 7/3 for TA
            double FAWeight = 0.3;
            double TAWeight = 0.7;

            double combinedTAFASignal = (TASignal * TAWeight) + (FASignal * FAWeight);
            double combinedTAFAProbabilty = 0;

            if (combinedTAFASignal == 1 || combinedTAFASignal > 0.5)
            {
                combinedTAFAProbabilty = (TAProbabilty * TAWeight);
                string content = $"taaResult {combinedTAFAProbabilty} {stock} 1";  //Probability, stock name, signal
                Send("coordinatorAgent", content);
            }
            else if(combinedTAFASignal <= 0.5)
            {
                combinedTAFAProbabilty = (FAProbabilty * FAWeight);
                string content = $"taaResult {combinedTAFAProbabilty} {stock} 0";  //Probability, stock name, signal
                Send("coordinatorAgent", content);
            }

            

        }

        private (double, int) HandleFA(string stock)
        {
            double probability = 0;
            Console.WriteLine("Fetching API data");
            JsonConvert[] apiResponse = apiReply.CallApiSync(stock);

            Console.WriteLine("<><><><><><><><><><><><><><> API response recieved: " + apiResponse[0].ratingScore);
            
            int signal = 0;

            if (apiResponse[0].ratingScore > 3)
            {
                //Buy
                signal = 1;
                probability = 1;
            }
            else
            {
                //Sell
                signal = 0;
                probability = 1;
            }

            return (probability, signal);
        }



        private (double, int) HandleSMA(string stock, DateTime startDate)
        {
            InputData dat = new InputData();
            List<InputData> d = new List<InputData>();
            d = dat.LoadData(stock);
            Console.WriteLine($"{this.Name} calculating the 50 day moving average from {startDate}...");

            int period = 50;
            
            int index = d.FindIndex(d => d.date == startDate);
            int p = index;

            double sum = 0;
            double SMA = 0;

            for (int i = period; i > 0; i--)
            {
                p--;
                sum += ((d[p].open + d[p].close) / 2);
            }

            SMA = sum / period;
            p = period;

            int signal = 0;
            double probability = 0;

            if (SMA < d[index].close)    //Current day stock price
            {
                //Buy
                signal = 1;
                probability = 1;
                Console.WriteLine($"Stock: {stock} is in an uptrend - Current price: {d[index].close} - SMA: {SMA}");

            }
            else if (SMA > d[index].close)
            {
                //Sell
                signal = 0;
                probability = 1;
                Console.WriteLine($"Stock: {stock} is in a downtrend - Current price: {d[index].close} - SMA: {SMA}");

            }
            else
            {
                //No change
                signal = 0;
                probability = 0;
                Console.WriteLine($"Stock: {stock} has no change - Current price: {d[index].close} - SMA: {SMA}");

            }

            return (probability, signal);
        }
    }
}

