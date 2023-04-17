using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using ActressMas;
using MAS_Algo_Trader;

namespace MAS_Coursework_Double_Auction
{
    public class TAA3 : Agent
    {
        public TAA3()
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
                        Console.WriteLine("Initialising RSI calcuation for: " + parameters[0]);
                        HandleRSI(parameters[0], Convert.ToDateTime(parameters[1]));
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

        private void HandleRSI(string stock, DateTime startDate)
        {
            InputData dat = new InputData();
            List<InputData> d = new List<InputData>();
            d = dat.LoadData(stock);
            Console.WriteLine($"{this.Name} calculating the relative strength index from {startDate}...");

            int period = 50;

            int index = d.FindIndex(d => d.date == startDate);
            int p = index;

            double sumGain = 0;
            double sumLoss = 0;

            for (int i = period; i > 0; i--)
            {
                double priceChange = d[p].close - d[p - 1].close;

                if (priceChange >= 0)
                {
                    sumGain += priceChange;
                }
                else
                {
                    sumLoss += Math.Abs(priceChange);
                }
                p--;
            }

            double averageGain = sumGain / period;
            double averageLoss = sumLoss / period;
            p = index;

            for (int i = period; i > 0; i--)
            {
                double priceChange = d[p].close - d[p-1].close;

                if (priceChange >= 0)
                {
                    averageGain = (averageGain * (period - 1) + priceChange) / period;
                    averageLoss = (averageLoss * (period - 1)) / period;
                }
                else
                {
                    averageGain = (averageGain * (period - 1)) / period;
                    averageLoss = (averageLoss * (period - 1) + Math.Abs(priceChange)) / period;
                }
                p--;
            }

            double rs = averageGain / averageLoss;
            double rsi = 100 - (100 / (1 + rs));

            if (rsi > 70)   //If high then stock is valued as overbought so should sell
            {
                Console.WriteLine($"Stock: {stock} has high RSI strength - Current RSI: {rsi} - Overbought");
                string content = $"taaResult 1 {stock} 0";  //Probability, stock name, sell
                Send("coordinatorAgent", content);
            }
            else if (rsi < 30)  //If high then stock is valued as oversold so should buy
            {
                Console.WriteLine($"Stock: {stock} has low RSI strength - Current RSI: {rsi} - Oversold");
                string content = $"taaResult 1 {stock} 1";  //Probability, stock name, buy
                Send("coordinatorAgent", content);
            }
            else
            {
                Console.WriteLine($"Stock: {stock} RSI has no inbalance - Current RSI: {rsi}");
                string content = $"taaResult 0 {stock} 0";  //Probability, stock name, nothing
                Send("coordinatorAgent", content);
            }
        }
    }
}

