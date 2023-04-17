/*
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using ActressMas;
using MAS_Algo_Trader;

class EnvironmentAgent : Agent
{
    

    private const int Min = 0; //stock price change
    private const int Max = 20; //


    


    public override void Act(Message message)
    {

        switch (message.Content)
        {
            case "start": //this agent only responds to "start" messages
                string senderID = message.Sender;


                //string content = $"inform {price1} {price2} {price3} {price4} {price5}";
                //Send(senderID, content); //send the message with this information back to the household agent that requested it
                break;

            default:
                break;
        }
    }
}