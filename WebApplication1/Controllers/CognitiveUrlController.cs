using RestSharp;
using System.Linq;
using System.Web.Http;
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

            PredictionResult result;
            var model = new CustomVisionRequest { url = searchRequest.ImageUrl };

            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model);
            _client.AddDefaultHeader("Prediction-Key", System.Configuration.ConfigurationManager.AppSettings["PredictionKey"]);

            var response = _client.Execute<PredictionResult>(request).Data;
            if (response!=null && response.Predictions != null)
            {
                response.Predictions = response.Predictions.Where(p => p.Probability > 0.5).ToList();
                Utility.LogReport(response, searchRequest.Location);
            }

            return response;
        }
                       
    }
}
