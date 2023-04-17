using System;
using System.Collections.Generic;
using ActressMas;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Linq;
using System.Reflection;
using System.Collections;
using MAS_Algo_Trader;
using System.Reflection.Metadata;

namespace MAS_Coursework_Double_Auction
{
    public class CoordinatorAgent : Agent
    {

        public CoordinatorAgent()
        {

        }
        private int calculations = 0;
        private int dateLoops = 0;

        bool flag = true;

        private int numResults = 0;

        private int _turnsToWait;
        private List<AgentRep> TAAList;
        private List<AgentRep> FAAList;
        private List<string> taaStockList = new List<string>(Program.stockList);
        private List<string> faaStockList = new List<string>(Program.stockList);
        private Dictionary<string, List<Result>> taaResultDict = new Dictionary<string, List<Result>>();
        //private Dictionary<string, List<Result>> faaResultDict = new Dictionary<string, List<Result>>();
        //private Dictionary<string, List<double>> combinedResultsDict = new Dictionary<string, List<double>>();

        private List<DateTime> dateTimesList = new List<DateTime>();

        //private List<string> faaStockList = new List<string>(Program.stockList);



        int iterations = 6;
        bool finish = false;

        public override void Setup()
        {

            
            _turnsToWait = Program.turns;
            
            TAAList = new List<AgentRep>();
            FAAList = new List<AgentRep>();

            DateTime date1 = new DateTime(2022, 4, 1);
            DateTime date2 = new DateTime(2022, 5, 3);
            DateTime date3 = new DateTime(2022, 6, 1);
            //DateTime date4 = new DateTime(2022, 10, 4);
            //DateTime date5 = new DateTime(2022, 11, 1);
            //DateTime date6 = new DateTime(2022, 12, 1);


            dateTimesList.Add(date1);
            dateTimesList.Add(date2);
            dateTimesList.Add(date3);
            //dateTimesList.Add(date4);
            //dateTimesList.Add(date5);
            //dateTimesList.Add(date6);


        }

        public override void Act(Message message)   //Program enters here after it receives a message
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);  //parsing the data into the action and a list of parameters

                switch (action)
                {
                    case "taaResult":
                        HandleTAAResult(Convert.ToDouble(parameters[0]), parameters[1], message.Sender, Convert.ToInt32(parameters[2]));
                        break;

                    case "faaResult":
                        HandleFAAResult(Convert.ToDouble(parameters[0]), parameters[1], message.Sender, Convert.ToInt32(parameters[2]));
                        break;

                    case "registerFAA":
                        Console.WriteLine($"Registering FAA {message.Sender} to list");
                        HandleRegisterFAA(message.Sender);
                        break;

                    case "unregisterFAA":
                        Console.WriteLine($"Unregistering FAA {message.Sender} from list");
                        HandleUnregisterFAA(message.Sender);
                        break;

                    case "registerTAA":
                        Console.WriteLine($"Registering TAA {message.Sender} to list");
                        HandleRegisterTAA(message.Sender);
                        break;

                    case "unregisterTAA":
                        Console.WriteLine($"Unregistering TAA {message.Sender} from list");
                        HandleUnregisterTAA(message.Sender);
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


        public override void ActDefault()   //Program enters here if no messages have been sent to it
        {
            if (_turnsToWait-- <= 0)
            {


                //When no messages are being recieved and agents have been given stocks
                if (finish)
                {
                    HandleFinish();
                }

                //Task.Delay(500).Wait();

                if (numResults == Program.numTAA && finish == false )
                {
                    
                    CalculateResult();
                    taaStockList.RemoveAt(0);  //Moves to the next date in the list after calculating the result
                    
                    calculations++;
                    //Task.Delay(2000).Wait();
                    flag = true;
                }

                if (taaStockList.Count == 0)
                {
                    //Replenishes stocks to be evaluated
                    taaStockList.AddRange(Program.stockList);
                    dateTimesList.RemoveAt(0);
                    dateLoops++;
                }



                //Send each TA agent each stock to analyse
                if (dateTimesList.Count != 0 && flag == true)
                {
                    numResults = 0;
                    for (int i = Program.numTAA; i > 0; i--)
                    {
                        string content = $"start {taaStockList[0]} {dateTimesList[0]}";
                        Send($"{TAAList[i - 1].agentName}", content);
                    }
                    flag = false;
                }

                _turnsToWait = Program.turns+100;

                if (dateTimesList.Count == 0)
                {
                    finish = true;
                }
            }
        }


        

        private void CalculateResult()
        {
            Console.WriteLine("Calculate combined reputation and result");

            double repWeight = 0.4;
            double resultWeight = 0.6;

            List<double> stockProbRepList = new List<double>();

            foreach (var agent in TAAList)
            {
                agent.stock.combinedResRep = (agent.stock.probability * resultWeight) + (agent.reputation * repWeight);
                stockProbRepList.Add(agent.stock.combinedResRep);   //Adding probability combined with the agents reputation to the list
            }

            double maxProb = stockProbRepList.Max();
            int maxIndex = stockProbRepList.IndexOf(stockProbRepList.Max());

            Console.WriteLine($"Chosen agent to make trade for {TAAList[maxIndex].stock.stockName} is: {TAAList[maxIndex].agentName}");
            Console.WriteLine($"-- Probability of trade: {TAAList[maxIndex].stock.probability}");
            Console.WriteLine($"-- Agent reputation: {TAAList[maxIndex].reputation}");
            Console.WriteLine($"-- Combined probability and reputation: {TAAList[maxIndex].stock.combinedResRep}");
            Console.WriteLine($"-- Signal generated: {TAAList[maxIndex].stock.signal}");

            InputData dat = new InputData();
            List<InputData> d = new List<InputData>();
            d = dat.LoadData(TAAList[maxIndex].stock.stockName);

            int dateIndex = d.FindIndex(d => d.date == dateTimesList[0]);

            if (TAAList[maxIndex].stock.signal == 1)
            {   //Buy signal
                Console.WriteLine($"Executing a buy trade for: {TAAList[maxIndex].stock.stockName}");
                if (d[dateIndex + 1].close > d[dateIndex].close)
                {
                    Console.WriteLine("Successful trade - Reward agent reputation");
                    TAAList[maxIndex].reputation = TAAList[maxIndex].reputation + 0.2;
                }
                else
                {
                    Console.WriteLine("Unsuccessful trade - Penalise agent reputation");
                    TAAList[maxIndex].reputation = TAAList[maxIndex].reputation - 0.2;
                }
            }
            else if(TAAList[maxIndex].stock.signal == 0 && TAAList[maxIndex].stock.probability > 0)
            {   //Sell signal
                Console.WriteLine($"Executing a sell trade for: {TAAList[maxIndex].stock.stockName}");
                if (d[dateIndex + 1].close < d[dateIndex].close)
                {
                    Console.WriteLine("Successful trade - Reward agent reputation");
                    TAAList[maxIndex].reputation = TAAList[maxIndex].reputation + 0.2;
                }
                else
                {
                    Console.WriteLine("Unsuccessful trade - Penalise agent reputation");
                    TAAList[maxIndex].reputation = TAAList[maxIndex].reputation - 0.2;
                }
            }
            else
            {
                Console.WriteLine("No change in stock price");
            }
        }

        public void HandleFinish()
        {
            Console.WriteLine("Program is finished");
            foreach(var agent in TAAList)
            {
                Console.WriteLine($"{agent.agentName} <><><> Reputation: {agent.reputation} <><><>");
            }

            Console.WriteLine("Number of calculations performed: " + calculations);
            Console.WriteLine("Number of time periods calculated: " + dateLoops);

            Broadcast("stop");
            Stop();
        }

        public void HandleTAAResult(double probability, string stock, string agentName, int signal)
        {
            Result agentResult = new Result();
            var agentToUpdate = TAAList.FirstOrDefault(a => a.agentName == agentName);

            agentResult.probability = probability;
            agentResult.signal = signal;
            agentResult.stockName = stock;
            agentResult.combinedResRep = 0.0;

            agentToUpdate.stock = agentResult;
            numResults++;
        }

        public void HandleFAAResult(double probability, string stock, string agentName, int signal)
        {
            var agentToUpdate = FAAList.FirstOrDefault(a => a.agentName == agentName);

            agentToUpdate.stock.probability = probability;
            agentToUpdate.stock.stockName = stock;
            agentToUpdate.stock.signal = signal;
        }


        private void HandleRegisterTAA(string sender)
        {
            AgentRep agentRep = new AgentRep();
            agentRep.reputation = 1;
            agentRep.agentName = sender;
            TAAList.Add(agentRep);
        }

        private void HandleUnregisterTAA(string sender)
        {
            
        }

        private void HandleRegisterFAA(string sender)
        {
            AgentRep agentRep = new AgentRep();
            agentRep.reputation = 1;
            agentRep.agentName = sender;
            FAAList.Add(agentRep);
        }

        private void HandleUnregisterFAA(string sender)
        {

        }

    }
}

