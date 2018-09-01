using RestSharp;
using System;
using WebApplication1.Models;

namespace WebApplication1
{
    public static class Utility
    {
        public static void LogReport(PredictionResult result, LocationDetail loc)
        {
            RestClient _client = new RestClient(System.Configuration.ConfigurationManager.AppSettings["ElasticUrl"]);
            foreach (var prediction in result.Predictions)
            {
                var logEntry = new LogDetails
                {
                    Time = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffZ"),
                    Location = loc,
                    ReportedDis = prediction.TagName.Split('_')[1],
                    Crop = prediction.TagName.Split('_')[0],
                    Probability = prediction.Probability * 100
                };

                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(logEntry);
                var response = _client.Execute(request);
            }

        }
    }

    public class LogDetails
    {
        public string Time { get; set; }
        public LocationDetail Location { get; set; }
        public string ReportedDis { get; set; }
        public string Crop { get; set; }
        public float Probability { get; set; }
    }

    public class LocationDetail
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }
}