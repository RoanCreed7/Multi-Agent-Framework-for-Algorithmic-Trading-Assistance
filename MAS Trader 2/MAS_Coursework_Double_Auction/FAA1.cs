using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ActressMas;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using MAS_Algo_Trader;

namespace MAS_Coursework_Double_Auction
{



    public class FAA1 : Agent
    {

        public override void Setup()
        {
            Send("coordinatorAgent", $"registerFAA");

        }

        public override void Act(Message message)   //Program enters here after it receives a message
        {
            try
            {
                Console.WriteLine($"\t{message.Format()}");
                message.Parse(out string action, out List<string> parameters);  //parsing the data coming from the environment agent into the action and a list of parameters
                switch (action)
                {
                   case "stop":
                        Stop();
                        break;

                    case "FUN":
                        Console.WriteLine(parameters[0]);
                        break;

                    case "here":
                        Console.WriteLine("we got here");
                        break;



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public override void ActDefault()
        {

        }

        private void HandleFUN(string stock)    //Should use API to get data
        {
            Random random = new Random();
            int i = random.Next(-1, 2);

            string content = $"faaResult {i} {stock}";
            Send("coordinatorAgent", content);
        }

        



    }
}

