using Azure.Storage.Blobs;
using System.IO;

namespace Assessment.Util
{
    //DOWNLOAD MANAGER CLASS
    public class DownloadManager
    {
        //GET DOWNLOAD URL METHOD
        public static string GetDownloadUrl(string path, string fileName)
        {            
            //PREPARE CONNECTION STRING
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=byronblobstorage;AccountKey=lG+aG4Kx9KrrJpSvs+r6XK2roSLqUilwyt55LtViDFFTd3DChK3gWQ9q3hSCEbTnN930Dd49MfIBlAI/ORUMoA==;EndpointSuffix=core.windows.net";

            //CREATE A CLIENT
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //GRAB THE DATA CONTAINER
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("data");

            //PREPARE TO CREATE A BLOB
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            //READ FILE AND UPLOAD AS BLOB
            var uploadFileStream = File.OpenRead(path);
            blobClient.Upload(uploadFileStream);
            uploadFileStream.Close();
            
            //RETURN THE UPLOADED BLOB'S URL
            return blobClient.Uri.AbsoluteUri.ToString();
        }
    }
}
