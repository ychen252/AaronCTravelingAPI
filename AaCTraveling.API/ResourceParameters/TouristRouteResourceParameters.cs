using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AaCTraveling.API.ResourceParameters
{
    public class TouristRouteResourceParameters
    {
        public string Keyword { get; set; }
        public string Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperatorType = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }
                }
            }
        }
        public string RatingOperatorType { set; get; }
        public int? RatingValue { get; set; }
        private string _rating;
    }
}
