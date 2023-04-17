using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ActressMas;
using MAS_Algo_Trader;




namespace MAS_Coursework_Double_Auction
{
    public class DataAgent : Agent
    {
        //Lists of data

        //public List<InputData> dataList = new List<InputData>();



        public DataAgent()
        {
            Console.WriteLine("DataAgent: Idling");

            

            InputData ddd = new InputData();
            //ddd.LoadData();
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

        



    }
}

