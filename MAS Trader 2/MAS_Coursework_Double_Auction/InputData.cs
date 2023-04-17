using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


//This class acts like the database where all agents can read file data from

namespace MAS_Algo_Trader
{
	public class InputData
	{
		
		public DateTime date { get; set; }
        public Double open { get; set; }
		public Double high { get; set; }
		public Double low { get; set; }
        public Double close { get; set; }
        public Double volume { get; set; }
        public Dictionary<string, List<InputData>> dataLists { get; set; }

        public InputData()
		{
            dataLists = new Dictionary<string, List<InputData>>();
        }

        public List<InputData> LoadData(string stock)
        {
            Console.WriteLine($"Loading data for {stock}...");
            
            string mainPath = "/Users/roancreed/Desktop/University/FourthYear/Dissertation/MAS Trader 2/MAS_Coursework_Double_Auction/Data/";
            string csv = ".csv";
            string path = mainPath + stock + csv;

            string listName = stock;
            List<InputData> dataList = new List<InputData>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                String[] lineSplit;

                while ((line = reader.ReadLine()) != null)
                {
                    InputData temp = new InputData();
                    
                    lineSplit = line.Split(',');
                    temp.date = DateTime.ParseExact(lineSplit[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    temp.open = Double.Parse(lineSplit[1]);
                    temp.high = Double.Parse(lineSplit[2]);
                    temp.low = Double.Parse(lineSplit[3]);
                    temp.close = Double.Parse(lineSplit[4]);
                    temp.volume = Double.Parse(lineSplit[6]);

                    dataList.Add(temp);
                }
                dataLists.Add(listName, dataList);
                //Console.WriteLine($"Adding data list for {listName}");
                reader.Close();
            }
            //Console.WriteLine($"Sample data for {stock}: " + dataList[0].date.ToString("yyyy-MM-dd") + " " + dataList[0].open + " " + dataList[0].high + " " + dataList[0].low + " " + dataList[0].close + " " + dataList[0].volume);
            //Console.WriteLine($"Data collection for {stock} complete");
            return dataList;
        }
    }
}

