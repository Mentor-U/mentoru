using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.Services.Blob
{


    public class BlobService
    {

        private static readonly Lazy<BlobService> lazy = new Lazy<BlobService>
           (() => new BlobService());

        public static BlobService Instance { get { return lazy.Value; } }

        // Blob Storage
        private string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=mentorustorage;AccountKey=gbaR9b3iwGtWfSjbWKW5mgC1mtEpU2UijjOrwrniFaAS8Kb0KLr/g4inZX6+aoNB07FoUUSR4hxYYP7ZTNbbfw==;EndpointSuffix=core.windows.net";
        public BlobServiceClient BlobServiceClient { get; internal set; }

        private BlobService()
        {
            BlobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        public async Task<ImageSource> TryDownloadImage(string ContainerName, string ImageName)
        {

            BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient(ContainerName);
            BlobClient blob = containerClient.GetBlobClient(ImageName);
          
            ImageSource ItemImageSource;

            if (blob.Exists())
            {
                BlobDownloadInfo info = await blob.DownloadAsync();

                ItemImageSource = ImageSource.FromStream(() => info.Content);
            }
            else
            {
                ItemImageSource = "placeholder.jpg";
            }

            return ItemImageSource;

        }

        /// <summary>
        /// Uploads an image to blob storage. Checks to see if image exists, and then replaces it if so. 
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="ImageName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<bool> TryUploadImage(BlobContainerClient containerClient, string ImageName, string filePath)
        {


            BlobClient blob = containerClient.GetBlobClient(ImageName);

            try
            {
                //deletes the file if it exists
                await containerClient.DeleteBlobIfExistsAsync(ImageName);

                //uploads the file
                await containerClient.UploadBlobAsync(ImageName, File.OpenRead(filePath));


            }
            catch (Exception e)
            {
                return false;
            }

            return true;

        }
    }
}
