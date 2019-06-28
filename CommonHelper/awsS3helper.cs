using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amazon;
using Amazon.S3;
using Amazon.S3;
using Amazon.S3.Transfer;
namespace CommonHelper
{
        public class AmazonUploader
        {
            public bool sendMyFileToS3(System.IO.Stream localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
            {
                IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USWest1);
                TransferUtility utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
                {
                    request.BucketName = bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    request.BucketName = bucketName + @"/" + subDirectoryInBucket;
                }
                request.Key = fileNameInS3; //file name up in S3  
                request.InputStream = localFilePath;
                utility.Upload(request); //commensing the transfer  

                return true; //indicate that the file was sent  
            }
        }
    public class awsS3helper
    {
        public Boolean UploadToS3(string filename,Stream st)
        {
            Boolean resoult = false;
          

            //string name = Path.GetFileName(FileUpload1.FileName);
            string myBucketName = "fashionpass"; //your s3 bucket name goes here  
            string s3DirectoryName = "badger_images";
            string s3FileName = filename;
            bool a;
            AmazonUploader myUploader = new AmazonUploader();
            a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
            if (a == true)
            {
                resoult = true;

            }
            else
                resoult = false;


            return resoult;
        }
    }
}
