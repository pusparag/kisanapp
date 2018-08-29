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
        private static List<string> hemlockImages;
        private static List<string> japaneseCherryImages;
        private static MemoryStream testImage;

        
        private static void LoadImagesFromDisk()
        {
            // this loads the images to be uploaded from disk into memory
            hemlockImages = Directory.GetFiles(Path.Combine("Images", "Hemlock")).ToList();
            japaneseCherryImages = Directory.GetFiles(Path.Combine("Images", "Japanese Cherry")).ToList();
            testImage = new MemoryStream(File.ReadAllBytes(Path.Combine("Images", "Test\\test_image.jpg")));
        }
        static void Main(string[] args)
        {
            // Add your training & prediction key from the settings page of the portal
            string trainingKey = "d7ba782c8051443c8557ff464418949f";
            //string predictionKey = "<your prediction key here>";

            // Create the Api, passing in the training key
            TrainingApi trainingApi = new TrainingApi() { ApiKey = trainingKey };

            // Create a new project
            Console.WriteLine("Creating new project:");
            var project = trainingApi.GetProject(new Guid("f3e17865-ad01-45b1-8974-730649c79634"));


            var tags = trainingApi.GetTags(project.Id);
            var sourceDirectory= System.Configuration.ConfigurationSettings.AppSettings["ImageSourece"];
            var imageDirectories=Directory.GetDirectories(sourceDirectory).ToList();
            var count = 0;
            var countcategories = 0;
            foreach (var directory in imageDirectories)
            {
                
                var diseaseTags = directory.Split(new string[] {"___"}, StringSplitOptions.None);
                if (diseaseTags.Count() < 2)
                    continue;
                var diseaseTag = diseaseTags[1];
                if (diseaseTag == "healthy")
                    continue;

                var imagefiles = Directory.GetFiles(Path.Combine(sourceDirectory, directory)).ToList();
                var randomFiles = GetRandomFiles(imagefiles, 190);
                count += randomFiles.Count();
                countcategories++;
            }

            



            // Make two tags in the new project
            var hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock");
            var japaneseCherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry");

            // Add some images to the tags
            Console.WriteLine("\tUploading images");
            LoadImagesFromDisk();

            // Images can be uploaded one at a time
            //foreach (var image in hemlockImages)
            //{
            //    using (var stream = new MemoryStream(File.ReadAllBytes(image)))
            //    {
            //        trainingApi.CreateImagesFromData(project.Id, stream, new List<string>() { hemlockTag.Id.ToString() });
            //    }
            //}

            // Or uploaded in a single batch 
            var imageFiles = japaneseCherryImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
            trainingApi.CreateImagesFromFiles(project.Id, new ImageFileCreateBatch(imageFiles, new List<Guid>() { japaneseCherryTag.Id }));

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

            // Now there is a trained endpoint, it can be used to make a prediction

            // Create a prediction endpoint, passing in obtained prediction key
            //PredictionEndpoint endpoint = new PredictionEndpoint() { ApiKey = predictionKey };

            //// Make a prediction against the new project
            //Console.WriteLine("Making a prediction:");
            //var result = endpoint.PredictImage(project.Id, testImage);

            //// Loop over each prediction and write out the results
            //foreach (var c in result.Predictions)
            //{
            //    Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
            //}
            Console.ReadKey();
        }

        private static List<string> GetRandomFiles(List<string> imagefiles, int v)
        {
            var count = 0;
            var maxfiles = imagefiles.Count()-1;
            List<string> selectedFiles = new List<string>();
            while (count <= v)
            {
                Random random = new Random();
                var ran = random.Next(0, maxfiles);
                if (!selectedFiles.Contains(imagefiles[ran]))
                {
                    selectedFiles.Add(imagefiles[ran]);
                    count++;
                }
            }
            return selectedFiles;
            
        }
    }
}
