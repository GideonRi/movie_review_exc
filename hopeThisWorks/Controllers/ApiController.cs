using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hopeThisWorks.Controllers
{
    public class ApiController : Controller
    {
        api connection = new api();

        public Rating makeRating(string Source, string Value)
        {
            Rating own = new Rating();
            own.Source = Source;
            own.value = Value;
            return own;
        }

        [HttpPost("postReviewRoute")]
        public bool publishRating([FromQuery]string value, [FromQuery]string name)
        {
            try
            {
                connection.sendRating(name, value);
                return true;
            }
            catch
            {
                return false;
            }
        }


        [HttpGet("indexRoute")]
        public Movie getMovie([FromQuery] string name)
        {
            Movie movie = connection.getMovieByName(name);

            return movie;
        }

        [HttpGet("avarageRoute")]
        public string getAvarage([FromQuery] string name)
        {

            Movie request = connection.getMovieByName(name);

            int value1 = 0;
            int value2 = 0;
            int value3 = 0;
            int amount = 0;
            //check if a reviewer has left a review on the movie
            foreach (Rating rating in request.Ratings)
            {
                if (rating.Source == "Metacritic")
                {
                    //set the value of the review 
                    value1 = value(rating, '/');
                    //higher the amount of reviews found
                    amount++;
                }
                else if (rating.Source == "Internet Movie Database")
                {
                    value2 = value(rating, '/');
                    amount++;
                }
                else if (rating.Source == "Rotten Tomatoes")
                {
                    value3 = value(rating, '%');
                    amount++;
                }
            }
            try
            {
                int value4 = Convert.ToInt32(connection.findOwnScore(name));
                amount++;
                //get the average score
                decimal avarage = (value1 + value2 + value3 + value4) / amount;
                //show the avarage score
                return Convert.ToString(avarage);
            }
            catch
            {
                //get the average score
                decimal avarage = (value1 + value2 + value3) / amount;
                //show the avarage score
                return Convert.ToString(avarage);
            }            
        }

        private int value(Rating rating, char cutOffSign)
        {
            //cut the string corectly
            string input = rating.value;
            int index = input.IndexOf(cutOffSign);
            if (index >= 0)
                input = input.Substring(0, index);
            //if needed, remove .'s from strings
            input = input.Replace(".", "");
            //convert oucome to int
            return Convert.ToInt32(input);
        }
    }
}
