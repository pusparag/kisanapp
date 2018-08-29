using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;

namespace kisan
{
    class Program
    {
        static void Main(string[] args)
        {
            // Add your training & prediction key from the settings page of the portal
            string trainingKey = "d7ba782c8051443c8557ff464418949f";
            
            // Create the Api, passing in the training key
            TrainingApi trainingApi = new TrainingApi() { ApiKey = trainingKey };

            // Create a new project
            Console.WriteLine("Creating new project:");
            var project = trainingApi.GetProject(new Guid("65d4860d-036d-4e2a-97af-50c3afaec972"));


            var sourceDirectory= System.Configuration.ConfigurationSettings.AppSettings["ImageSourece"];
            var imageDirectories=Directory.GetDirectories(sourceDirectory).ToList();
            foreach (var directory in imageDirectories)
            {
                
                var diseaseTags = directory.Split(new string[] {"___"}, StringSplitOptions.None);
                if (diseaseTags.Count() < 2)
                    continue;
                var diseaseTag = diseaseTags[1].Replace('_',' ');
                if (diseaseTag == "healthy")
                    continue;
                var plantName = diseaseTags[0].Substring(diseaseTags[0].LastIndexOf('\\')+1).Replace('_', ' ');

                var imagefiles = Directory.GetFiles(Path.Combine(sourceDirectory, directory)).ToList();
                var imagePerTag = 190;
                
                var tagName = string.Format("{0} {1}", plantName, diseaseTag);
                if (trainingApi.GetTags(project.Id).Any(t => t.Name == tagName))
                    continue;
                var tag = trainingApi.CreateTag(project.Id, tagName);

                var randomFiles = GetRandomFiles(imagefiles, imagePerTag);
                //max upload size 64 in a batch
                var batchStartIndex = 0;
                var batchsize = 64;
                while (batchStartIndex < imagePerTag)
                {
                    var itemsCountToTake = (imagePerTag - batchStartIndex >= batchsize) ? batchsize : imagePerTag - batchStartIndex;
                    var batchImages = randomFiles.Skip(batchStartIndex).Take(itemsCountToTake);
                    var imageFilesToUpload = batchImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
                    trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFilesToUpload, new List<Guid>() { tag.Id }));
                    batchStartIndex += itemsCountToTake;
                }
            }
                 
           
            // Now there are images with tags start training the project
            Console.WriteLine("\tTraining");
            var iteration = trainingApi.TrainProject(project.Id);

            // The returned iteration will be in progress, and can be queried periodically to see when it has completed
            while (iteration.Status == "Training")
            {
                System.Threading.Thread.Sleep(1000);

                // Re-query the iteration to get it's updated status
                iteration = trainingApi.GetIteration(project.Id, iteration.Id);
            }

            // The iteration is now trained. Make it the default project endpoint
            iteration.IsDefault = true;
            trainingApi.UpdateIteration(project.Id, iteration.Id, iteration);
            Console.WriteLine("Done!\n");

            Console.ReadKey();
        }

        private static List<string> GetRandomFiles(List<string> imagefiles, int v)
        {
            var count = 0;
            var maxfilesIndex = imagefiles.Count()-1;
            List<string> selectedFiles = new List<string>();
            int itera = 0;
            while (count < v)
            {
                Random random = new Random();
                var ran = random.Next(0, maxfilesIndex);
                if (!selectedFiles.Contains(imagefiles[ran]))
                {
                    selectedFiles.Add(imagefiles[ran]);
                    count++;
                }
                itera++;
            }
            return selectedFiles;
            
        }
    }
}
