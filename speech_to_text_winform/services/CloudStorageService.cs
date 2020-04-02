using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;

namespace speech_to_text_winform.services
{
    internal class CloudStorageService
    {
        string projectId = "selma-227201";

        // [START storage_create_bucket]
        public void CreateBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            
            storage.CreateBucket(projectId, bucketName);

            Console.WriteLine($"Created {bucketName}.");
        }
        // [END storage_create_bucket]
        //asia-southeast1
        public void CreateRegionalBucket(string location, string bucketName)
        {
            var storage = StorageClient.Create();
            Bucket bucket = new Bucket { Location = location, Name = bucketName };
            storage.CreateBucket(projectId, bucket);
            Console.WriteLine($"Created {bucketName}.");
        }

        // [START storage_delete_file]
        public async Task<string> AsyncDeleteObject(string bucketName, IEnumerable<string> objectNames)
        {
            try
            {
                var storage = await StorageClient.CreateAsync();
                foreach (var objectName in objectNames)
                {
                    await storage.DeleteObjectAsync(bucketName, objectName);
                    Console.WriteLine($"Deleted {objectName}.");
                }

                return "ok";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        // [END storage_delete_file]

        // [START storage_download_file]
        public async Task<string> AsyncDownloadObject(string bucketName, string objectName,
            string localPath = null)
        {
            try
            {
                var storage = await StorageClient.CreateAsync();
                localPath = localPath ?? Path.GetFileName(objectName);
                using (var outputFile = File.OpenWrite(localPath))
                {
                    await storage.DownloadObjectAsync(bucketName, objectName, outputFile);
                }

                Console.WriteLine($"downloaded {objectName} to {localPath}.");

                return "ok";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        // [END storage_download_file]


        // [START storage_upload_file]
        public async Task<string> AsyncUploadFile(string bucketName, string localPath,
            string objectName = null)
        {
            try
            {
                var storage = await StorageClient.CreateAsync();
                using (var f = File.OpenRead(localPath))
                {
                    objectName = objectName ?? Path.GetFileName(localPath);
                    await storage.UploadObjectAsync(bucketName, objectName, null, f);
                    Console.WriteLine($"Uploaded {objectName}.");
                }

                return objectName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        // [END storage_upload_file]
    }
}