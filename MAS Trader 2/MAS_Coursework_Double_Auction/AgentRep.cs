using System;
using System.Collections.Generic;

namespace MAS_Algo_Trader
{
	public class AgentRep
	{

        public double reputation { get; set; }
        public string agentName { get; set; }
        public Result stock { get; set; }

        public AgentRep()
		{
		}
	}

    public class Result
    {
        public int signal { get; set; }  //buy or sell (1/0)
        public double probability { get; set; }  //Predcted probability of this happening
        public string stockName { get; set; }
        public double combinedResRep { get; set; }

        public Result()
        {
        }
    }
}

