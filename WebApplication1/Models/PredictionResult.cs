using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PredictionResult
    {
        public Guid Id { get; set; }
        public Guid Project { get; set; }
        public Guid Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction> Predictions { get; set; }
    }
    public class Prediction
    {
        public float Probability { get; set; }
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public string Remedy
        {
            get
            {
                if (Remedies.RemediesList.ContainsKey(TagName))
                    return Remedies.RemediesList[TagName];
                return string.Empty;
            }
        }
    }

    public static class Remedies
    {
        public static Dictionary<string, string> RemediesList { get; set; } = new Dictionary<string, string>();

        public static void ReadAllRemedies()
        {
            NameValueCollection settingCollection = (NameValueCollection)ConfigurationManager.GetSection("Remedies");

            string[] allKeys = settingCollection.AllKeys;

            foreach (string key in allKeys)
            {
                RemediesList.Add(key, settingCollection[key]);
            }
        }
    }
    
}