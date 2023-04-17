using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using ActressMas;
using MAS_Algo_Trader;

namespace MAS_Coursework_Double_Auction
{
    public class TAA2 : Agent
    {

        public TAA2()
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
                        Console.WriteLine("Initialising OBV calcuation for: " + parameters[0]);
                        HandleOBV(parameters[0], Convert.ToDateTime(parameters[1]));
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

        private void HandleOBV(string stock, DateTime startDate)
        {
            InputData dat = new InputData();
            List<InputData> d = new List<InputData>();
            d = dat.LoadData(stock);
            Console.WriteLine($"{this.Name} calculating the on balence volume...");

            int period = 50;
            int index = d.FindIndex(d => d.date == startDate);
            int p = index;

            double currentOBV = 0;
            double previousOBV = 0;
            double previousClosePrice = 0;
            double dayBeforeOBV = 0;

            for (int i = period; i > 0; i--)
            {
                dayBeforeOBV = previousOBV;
                p--;
                if (d[p].close > previousClosePrice)
                {
                    currentOBV += d[p].volume;
                }
                else if (d[p].close < previousClosePrice)
                {
                    currentOBV -= d[p].volume;
                }
                previousClosePrice = d[p].close;
                previousOBV = currentOBV;
            }

            if(currentOBV > dayBeforeOBV)
            {
                Console.WriteLine($"Stock: {stock} is on an uptrend - Current OBV: {currentOBV} - Previous OBV: {dayBeforeOBV}");
                string content = $"taaResult 1 {stock} 1";  //Probability, stock name, buy
                Send("coordinatorAgent", content);
            }
            else if(currentOBV < dayBeforeOBV)
            {
                Console.WriteLine($"Stock: {stock} is on a downtrend - Current OBV: {currentOBV} - Previous OBV: {dayBeforeOBV}");
                string content = $"taaResult 1 {stock} 0";  //Probability, stock name, sell
                Send("coordinatorAgent", content);
            }
            else
            {
                Console.WriteLine($"Stock: {stock} has no change - Current OBV: {currentOBV} - Previous OBV: {dayBeforeOBV}");
                string content = $"taaResult 0 {stock} 0";  //Probability, stock name, no change
                Send("coordinatorAgent", content);
            }
        }
    }
}

