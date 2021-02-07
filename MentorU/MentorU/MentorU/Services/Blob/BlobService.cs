using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
