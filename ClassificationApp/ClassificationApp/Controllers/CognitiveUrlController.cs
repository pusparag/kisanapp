using RestSharp;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.WebPages;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CognitiveUrlController : ApiController
    {
        
        private RestClient _client;
        public PredictionResult Post([FromBody]CognitiveUrlRequest searchRequest)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["PredictionUrlForWebPath"];
            _client = new RestClient(url);
            if (searchRequest == null)
                return new PredictionResult { Errors = "Empty Search Request." };
            if (searchRequest.ImageUrl.IsEmpty())
                return new PredictionResult { Errors = "Requested Url to be analysed is empty" };

            var model = new CustomVisionRequest { url = searchRequest.ImageUrl };

            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model);
            _client.AddDefaultHeader("Prediction-Key", System.Configuration.ConfigurationManager.AppSettings["PredictionKey"]);
            var res = _client.Execute<PredictionResult>(request);
            if (res.StatusCode != HttpStatusCode.OK)
                return new PredictionResult { Errors = res.Content };
            var response = res.Data;
            if (response!=null && response.Predictions != null)
            {
                response.Predictions = response.Predictions.Where(p => p.Probability > 0.5).ToList();
                Utility.LogReport(response, searchRequest.Location);
                response.Predictions.ForEach(p => p.TagName = p.TagName.Replace('_', ' '));
            }

            return response;
        }
                       
    }
}
