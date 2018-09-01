using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
        
    public class CognitiveImageController : ApiController
    {

        public PredictionResult Post([FromBody]CognitiveImageRequest searchRequest)
        {
            //searchRequest.Image = GetImageAsByteArray(@"D:\Kisan\b.jpg");
            if(searchRequest==null)
                return new PredictionResult { Errors = "Empty Search Request."};

            if (searchRequest.Image == null || searchRequest.Image.Length == 0)
                return new PredictionResult { Errors = "Empty Image data."};
            byte[] image = Convert.FromBase64String(searchRequest.Image);
            string url = System.Configuration.ConfigurationManager.AppSettings["PredictionUrlForImage"];
            var _client = new RestClient(url);
            _client.AddHandler("application/octet-stream", new RestSharp.Deserializers.JsonDeserializer());

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddParameter("application/octet-stream", image, ParameterType.RequestBody);
            _client.AddDefaultHeader("Prediction-Key", System.Configuration.ConfigurationManager.AppSettings["PredictionKey"]);
            var res = _client.Execute<PredictionResult>(request);
            if(res.StatusCode!=HttpStatusCode.OK)
                return new PredictionResult { Errors = res.Content };
            var response = res.Data;
            if (response != null && response.Predictions != null)
            {
                response.Predictions = response.Predictions.Where(p => p.Probability > 0.5).ToList();
                Utility.LogReport(response, searchRequest.Location);
                response.Predictions.ForEach(p => p.TagName = p.TagName.Replace('_', ' '));
            }

            return response;
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
            
        }
       
    }

    class fileDetail
    {
        public byte[] content { get; set; }
        public string mimetype { get; set; }
        public string ext { get; set; }
    }
}
