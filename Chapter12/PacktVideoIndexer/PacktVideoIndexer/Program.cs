using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PacktVideoIndexer
{
    class Program
    {
        private static CloudMediaContext _context = null;

        static void Main(string[] args)
        {
            var tokenCredentials = new AzureAdTokenCredentials("sjoukjelive.onmicrosoft.com", new AzureAdClientSymmetricKey("7715809d-8c64-4cd6-9265-d5ac64675f7e", "AKv5r7h+ptucVd71Fa2SaEyjyKDv+eNW6yuP1XN+4ic="), AzureEnvironments.AzureCloudEnvironment);
            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            _context = new CloudMediaContext(new Uri("https://packtmediaservices.restv2.westeurope.media.azure.net/api/"), tokenProvider);

            var video = @"C:\PacktIndexer\InputFiles\BigBuckBunny.mp4";
            var config = @"C:\PacktIndexer\InputFiles\config.json";
            var asset = RunIndexingJob(video, config);

            DownloadAsset(asset, @"C:\PacktIndexer\OutputFiles");

        }
        static IAsset RunIndexingJob(string inputMediaFilePath, string configurationFile)
        {
            IAsset asset = CreateAssetAndUploadSingleFile(inputMediaFilePath,
                "Packt Indexing Input Asset",
                AssetCreationOptions.None);

            IJob job = _context.Jobs.Create("Packt Indexing Job");

            string MediaProcessorName = "Azure Media Indexer 2 Preview";

            var processor = GetLatestMediaProcessorByName(MediaProcessorName);
            string configuration = File.ReadAllText(configurationFile);

            ITask task = job.Tasks.AddNew("Packt Indexing Task",
                processor,
                configuration,
                TaskOptions.None);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew("Packt Indexing Output Asset", AssetCreationOptions.None);

            job.StateChanged += new EventHandler<JobStateChangedEventArgs>(StateChanged);
            job.Submit();

            Task progressJobTask = job.GetExecutionProgressTask(CancellationToken.None);

            progressJobTask.Wait();

            if (job.State == JobState.Error)
            {
                ErrorDetail error = job.Tasks.First().ErrorDetails.First();
                Console.WriteLine(string.Format("Error: {0}. {1}",
                                                error.Code,
                                                error.Message));
                return null;
            }

            return job.OutputMediaAssets[0];
        }

        static IAsset CreateAssetAndUploadSingleFile(string filePath, string assetName, AssetCreationOptions options)
        {
            IAsset asset = _context.Assets.Create(assetName, options);

            var assetFile = asset.AssetFiles.Create(Path.GetFileName(filePath));
            assetFile.Upload(filePath);

            return asset;
        }

        static void DownloadAsset(IAsset asset, string outputDirectory)
        {
            foreach (IAssetFile file in asset.AssetFiles)
            {
                file.Download(Path.Combine(outputDirectory, file.Name));
            }
        }

        static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _context.MediaProcessors
                .Where(p => p.Name == mediaProcessorName)
                .ToList()
                .OrderBy(p => new Version(p.Version))
                .LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor",
                                                           mediaProcessorName));

            return processor;
        }

        static private void StateChanged(object sender, JobStateChangedEventArgs e)
        {
            Console.WriteLine("Job state changed event:");
            Console.WriteLine("  Previous state: " + e.PreviousState);
            Console.WriteLine("  Current state: " + e.CurrentState);

            switch (e.CurrentState)
            {
                case JobState.Finished:
                    Console.WriteLine();
                    Console.WriteLine("Job is finished.");
                    Console.WriteLine();
                    break;
                case JobState.Canceling:
                case JobState.Queued:
                case JobState.Scheduled:
                case JobState.Processing:
                    Console.WriteLine("Please wait...\n");
                    break;
                case JobState.Canceled:
                case JobState.Error:
                    IJob job = (IJob)sender;
                    break;
                default:
                    break;
            }
        }
    }
}