using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
        
    public class CognitiveImageController : ApiController
    {
       
        public PredictionResult Post([FromBody]byte[] image1)
        {            
            var image = GetImageAsByteArray(@"D:\Kisan\b.jpg");
            _client.AddHandler("application/octet-stream", new RestSharp.Deserializers.JsonDeserializer());
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/octet-stream");
            
            request.AddParameter("application/octet-stream", image, ParameterType.RequestBody);
            _client.AddDefaultHeader("Prediction-Key", "9822f33b8f5941d9acfa3a842285e6f9");
            var response = _client.Execute<PredictionResult>(request).Data;
            
            return response;
        }
        const string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/f3e17865-ad01-45b1-8974-730649c79634/image?iterationId=751aee28-cb37-47d2-95a3-11d2a08dd8a9";

        private readonly RestClient _client = new RestClient(url);

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
