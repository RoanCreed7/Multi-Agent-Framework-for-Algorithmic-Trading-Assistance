using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MAS_Algo_Trader
{
	public class APIReply
	{

        public APIReply()
		{
            
		}


        public JsonConvert[] CallApiSync(string stock)
        {
        
            string apiStart = "https://financialmodelingprep.com/api/v3/historical-rating/";
            string apiEnd = "?limit=1&apikey=94e07214964175f36c3ca22f1f9a5a18";

            string apiUrl = apiStart + stock + apiEnd;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync().Result;
                    JsonConvert[] result = JsonSerializer.Deserialize<JsonConvert[]>(content);

                    /*
                    foreach (JsonConvert rating in result)
                    {
                        Console.WriteLine(rating.symbol);
                        Console.WriteLine(rating.date);
                        Console.WriteLine(rating.rating);
                        Console.WriteLine(rating.ratingScore);
                        Console.WriteLine(rating.ratingRecommendation);
                        Console.WriteLine(rating.ratingDetailsDCFScore);
                        Console.WriteLine(rating.ratingDetailsDCFRecommendation);
                        Console.WriteLine(rating.ratingDetailsROEScore);
                        Console.WriteLine(rating.ratingDetailsROERecommendation);
                        Console.WriteLine(rating.ratingDetailsROAScore);
                        Console.WriteLine(rating.ratingDetailsROARecommendation);
                        Console.WriteLine(rating.ratingDetailsDEScore);
                        Console.WriteLine(rating.ratingDetailsDERecommendation);
                        Console.WriteLine(rating.ratingDetailsPEScore);
                        Console.WriteLine(rating.ratingDetailsPERecommendation);
                        Console.WriteLine(rating.ratingDetailsPBScore);
                        Console.WriteLine(rating.ratingDetailsPBRecommendation);
                    }
                    */

                    return result;
                }
                else
                {
                    throw new Exception("API call failed with status code: " + response.StatusCode);
                }
            }
        }


    }
}

