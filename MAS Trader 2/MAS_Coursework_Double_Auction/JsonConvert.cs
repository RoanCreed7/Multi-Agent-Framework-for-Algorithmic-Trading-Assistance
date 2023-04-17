using System;
namespace MAS_Algo_Trader
{
	public class JsonConvert
	{
        public string symbol { get; set; }
        public DateTime date { get; set; }
        public string rating { get; set; }
        public int ratingScore { get; set; }
        public string ratingRecommendation { get; set; }
        public int ratingDetailsDCFScore { get; set; }
        public string ratingDetailsDCFRecommendation { get; set; }
        public int ratingDetailsROEScore { get; set; }
        public string ratingDetailsROERecommendation { get; set; }
        public int ratingDetailsROAScore { get; set; }
        public string ratingDetailsROARecommendation { get; set; }
        public int ratingDetailsDEScore { get; set; }
        public string ratingDetailsDERecommendation { get; set; }
        public int ratingDetailsPEScore { get; set; }
        public string ratingDetailsPERecommendation { get; set; }
        public int ratingDetailsPBScore { get; set; }
        public string ratingDetailsPBRecommendation { get; set; }

        public JsonConvert()
		{
		}
	}
}

