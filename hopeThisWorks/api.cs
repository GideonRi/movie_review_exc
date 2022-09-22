using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using MongoDB.Driver;
using MongoDB.Bson;

namespace hopeThisWorks
{
    public class api
    {
        public string getConnectionString(string name)
        {
            //make the connectionstring to the api
            string connectionString = @"https://www.omdbapi.com/?t=" + name + "&apikey=";
            return connectionString;
        }

        public string findOwnScore(string name)
        {
            string score = "";
            //prepare the client and request
            var client = new MongoClient();
            var database = client.GetDatabase("Ratings");
            var movie = database.GetCollection<BsonDocument>("RatingsCollection");
            var filter = Builders<BsonDocument>.Filter.Eq("title", name);
            //request data with appropiate filter
            var firstDocument = movie.Find(filter).FirstOrDefault();
            //turn the response into a string
            score = firstDocument.ToString();
            //obtain the value of the resonse
            score = Convert.ToString(convertToOwnScore(score));
            return score;
        }

        public int convertToOwnScore(string result)
        {
            //set neccisairy variables
            int score = 0;
            string[] subs = result.Split(':');
            foreach (var sub in subs)
            {
                //filter the string
                if (sub.StartsWith(" \""))
                {
                    try
                    {
                        //remove everything from the string except numbers
                        score = Convert.ToInt32(new string(sub.Where(Char.IsDigit).ToArray()));
                    }
                    catch
                    {
                    }
                }
            }
            return score;
        }



        public void sendRating(string name, string value)
        {
            //prepare connection
            var Client = new MongoClient();
            var MongoDB = Client.GetDatabase("Ratings");
            var Collec = MongoDB.GetCollection<BsonDocument>("RatingsCollection");
            var documnt = new BsonDocument
                //set variables
                {
                    {"title", name},
                    {"source", "User"},
                    {"value", value}
                };
            //check if there already exists a rating for this movie
            bool exists = doesItExist(name);
            //send
            Collec.InsertOne(documnt);
            // if there existed a rating before this one, remove it
            if (exists)
            {
                deletePreviosRating(name);
            }
            
        }

        public bool doesItExist(string name)
        {
            try
            {
                var client = new MongoClient();
                var database = client.GetDatabase("Ratings");
                var movie = database.GetCollection<BsonDocument>("RatingsCollection");
                var filter = Builders<BsonDocument>.Filter.Eq("title", name);
                //request data with appropiate filter
                var firstDocument = movie.Find(filter).FirstOrDefault();
                //turn the response into a string
                string score = firstDocument.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void deletePreviosRating(string name)
        {
            try
            {
                //prepare connection
                var client = new MongoClient();
                var database = client.GetDatabase("Ratings");
                var movie = database.GetCollection<BsonDocument>("RatingsCollection");
                var filter = Builders<BsonDocument>.Filter.Eq("title", name);
                //send request to delete entry
                movie.DeleteOne(filter);
            }
            catch
            {
            }
            
        }

        public Movie getMovieByName(string name)
        {
            Movie data = new Movie();
            string connectionString = getConnectionString(name);
            try
            {
                //create a request for the data
                WebRequest request = HttpWebRequest.Create(connectionString);
                //send the request
                WebResponse response = request.GetResponse();
                //recieve the anwser
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //put the anwser in a string
                string json = reader.ReadToEnd();
                //convert the string into an obect
                data = JsonConvert.DeserializeObject<Movie>(json);
            }
            catch (WebException e)
            {
            }
            return data;
        }
    }
}
