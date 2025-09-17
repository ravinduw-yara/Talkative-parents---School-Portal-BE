using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public class StorageService
    {
    }
    public static class Shared
    {
        public static string AdminChannel = "87859B1B-053D-4EED-8Ay21-B69A6F3A02CA"; //System channel
        public static string UploadContainer = "upload";
        public static string ProfileContainer = "profile";
        public static string SchoolContainer = "school";
        public static ICloudBlobManager CloudBlobManagerService = new CloudBlobManager();
        //internal static object telemetryClient;
        //public static TelemetryClient telemetryClient = new TelemetryClient();
    }

    public class BlobInfo
    {
        public string ParentContainerName { get; set; }
        public Uri Uri { get; set; }
        public string Filename { get; set; }
        public long FileLengthKilobytes { get; set; }
    }

    public interface ICloudBlobManager
    {
        void StoreFileInAzureStorage(string filename, Stream stream);
        public string StoreFileInAzureStorageV2(FilesStatus.FileUploadDetails details);
        void CopyAndRenameFileInAzureStorage(string filename, string guidPrefix);

        Task<string> StoreFileInAzureStorage(FilesStatus2.FileUploadDetails2 details);
        //string StoreFileInAzureStorage(FilesStatus2.FileUploadDetails2 details); // Octomber 11 2024
        string StoreFileInAzureStorage(FilesStatus2.FileUploadDetails3 details);
        //string StoreFileInAzureStorage(FilesStatus.FileUploadDetails details);
        void SaveParentDefaultProfilePic(string userId, int genderId);
        void SaveParentDefaultRelationPic(string userId, string channelId, int relationId);
        IEnumerable<BlobInfo> GetListOfBlobsFromContainer(string containerName);
        Stream DownloadBlob(BlobInfo blobInfo);
        bool DoesFileExist(string filename);
    }
    public class CloudBlobManager : ICloudBlobManager
    {


        private CloudStorageAccount _storageAccount = null;

        public CloudBlobManager()
        {
            // _storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tpupload;AccountKey=LNhNd8IW5Ld2dNgsrtd3YHE0M1zjye5mzq53e/1jDM24l4CG02xXfVIlYj2ZjPdbh8W+W25LIvfcfjKVQXCrtw==");
            _storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=yaraupload;AccountKey=10aLB8Gt/moVA553QF0z2TyjyS+x6GAE3LOHYwxm5AbSRs9FEZImLaEr5Fuhb1vB5gRxyRmDKzMfQ4p589K18g==;EndpointSuffix=core.windows.net");
            //_storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=yaraupload;AccountKey=10aLB8Gt/moVA553QF0z2TyjyS+x6GAE3LOHYwxm5AbSRs9FEZImLaEr5Fuhb1vB5gRxyRmDKzMfQ4p589K18g==");
        }



        public bool DoesFileExist(string filename)
        {
            try
            {
                CloudBlobContainer container = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
                return blockBlob.ExistsAsync().Result;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void StoreFileInAzureStorage(string filename, System.IO.Stream stream)
        {
            CloudBlobContainer container = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStreamAsync(stream);
        }


        public string StoreFileInAzureStorageV2(FilesStatus.FileUploadDetails details)
        {
            byte[] bytes = Convert.FromBase64String(details.base64);
            CloudBlobContainer blobContainer;// = GetBlobContainerCreateIfDoesntExists(Shared.profileContainer);
            string filename = "";
            if (details.isProfile)
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
                filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            }
            else if (details.isSchool)
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.SchoolContainer);
                filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            }
            else
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
                filename = Guid.NewGuid() + "_" + details.filename;
            }
            CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
            imgBlockBlob.DeleteIfExistsAsync();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                imgBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
                if (details.filename.ToLower().EndsWith(".png") || details.filename.ToLower().EndsWith(".jpg") || details.filename.ToLower().EndsWith(".jpeg")
                    || details.filename.ToLower().EndsWith(".bmp"))
                {
                    CloudBlockBlob imgThumbBlockBlob = blobContainer.GetBlockBlobReference("thumb_" + filename);
                    imgThumbBlockBlob.DeleteIfExistsAsync();
                    imgThumbBlockBlob.UploadFromStreamAsync(CreateThumbnail(ms));
                }
            }
            return filename;
        }


        public void CopyAndRenameFileInAzureStorage(string filename, string guidPrefix)
        {
            CloudBlobContainer blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filename);

            CloudBlockBlob newBlob = blobContainer.GetBlockBlobReference(guidPrefix + "_" + filename);
            newBlob.StartCopyAsync(blob);
            blob.DeleteAsync();
        }



        //public void StoreProfilePictureInAzureStorage(Talkative.Shared.FilesStatus.FileUploadDetails details)
        //{
        //    byte[] bytes = Convert.FromBase64String(details.base64)
        //    CloudBlobContainer blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.profileContainer);
        //    string filename = "";
        //    if (details.isProfile)
        //        filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
        //    else
        //        filename = Guid.NewGuid() + "_" + details.filename;
        //    CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
        //    imgBlockBlob.DeleteIfExists();
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        imgBlockBlob.UploadFromStream(ms);
        //    }
        //}


        //Octomber 11 2024 Newly Added Sanduni
        public async Task<string> StoreFileInAzureStorage(FilesStatus2.FileUploadDetails2 details)
        {
            byte[] bytes = null;
           // byte[] bytes = Convert.FromBase64String(details.base64);
            if (!string.IsNullOrEmpty(details.base64) && details.base64.Length > 0)
            {
                bytes = Convert.FromBase64String(details.base64);
                // Proceed with your logic here
            }
            else
            {
                return "Details.base64 is null or empty.";
            }
            CloudBlobContainer blobContainer;
            string filename = "";

            if (details.isProfile)
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
                filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            }
            //if (details.isSchool)
            //{
            //    blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
            //    filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            //}
            else
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
                filename = Guid.NewGuid() + "_" + details.filename;
            }

            CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);

            // Ensure the old blob is deleted if it exists
            await imgBlockBlob.DeleteIfExistsAsync();

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                // Upload the file as a byte array
                await imgBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

                // Check if the blob exists
                if (await imgBlockBlob.ExistsAsync())
                {
                    if (details.filename.ToLower().EndsWith(".png") ||
                        details.filename.ToLower().EndsWith(".jpg") ||
                        details.filename.ToLower().EndsWith(".jpeg") ||
                        details.filename.ToLower().EndsWith(".bmp"))
                    {
                        // Create a thumbnail for image files
                        CloudBlockBlob imgThumbBlockBlob = blobContainer.GetBlockBlobReference("thumb_" + filename);

                        // Delete the old thumbnail if it exists
                        await imgThumbBlockBlob.DeleteIfExistsAsync();

                        // Upload the thumbnail
                        await imgThumbBlockBlob.UploadFromStreamAsync(CreateThumbnail(ms));
                    }
                    else if (details.filename.ToLower().EndsWith(".pdf"))
                    {
                        // Skip thumbnail creation for PDFs
                        // Alternatively, handle PDFs differently, like creating an image of the first page
                    }
                }
                else
                {
                    return "File is not Uploaded";
                }
            }

            return filename;
        }

        //Octomber 11 2024 Sanduni
        //public string StoreFileInAzureStorage(FilesStatus2.FileUploadDetails2 details)
        //{
        //    byte[] bytes = Convert.FromBase64String(details.base64);
        //    CloudBlobContainer blobContainer;
        //    string filename = "";
        //    if (details.isProfile)
        //    {
        //        blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
        //        filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
        //    }
        //    //if (details.isSchool)
        //    //{
        //    //    blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
        //    //    filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
        //    //}
        //    else
        //    {
        //        blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
        //        filename = Guid.NewGuid() + "_" + details.filename;
        //    }
        //    CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
        //    imgBlockBlob.DeleteIfExistsAsync();
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //       imgBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        //       if (details.filename.ToLower().EndsWith(".png") || details.filename.ToLower().EndsWith(".jpg") || details.filename.ToLower().EndsWith(".jpeg")
        //               || details.filename.ToLower().EndsWith(".bmp"))
        //       {
        //            CloudBlockBlob imgThumbBlockBlob = blobContainer.GetBlockBlobReference("thumb_" + filename);
        //            imgThumbBlockBlob.DeleteIfExistsAsync();
        //            imgThumbBlockBlob.UploadFromStreamAsync(CreateThumbnail(ms));
        //       }
        //        else if (details.filename.ToLower().EndsWith(".pdf"))
        //        {

        //            // Skip thumbnail creation for PDFs or handle it separately if needed
        //            // Alternatively, use a library to convert the first page of the PDF to an image and create a thumbnail
        //        }
        //    }
        //    return filename;
        //}


        public string StoreFileInAzureStorage(FilesStatus2.FileUploadDetails3 details)
        {
            byte[] bytes = Convert.FromBase64String(details.base64);
            CloudBlobContainer blobContainer;
            string filename = "";
            if (details.isProfile)
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
                filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            }
            //if (details.isSchool)
            //{
            //    blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
            //    filename = details.profileId + details.filename.Substring(details.filename.LastIndexOf('.'));
            //}
            else
            {
                blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.UploadContainer);
                filename = Guid.NewGuid() + "_" + details.filename;
            }
            CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
            imgBlockBlob.DeleteIfExistsAsync();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                imgBlockBlob.UploadFromStreamAsync(ms);
                if (details.filename.ToLower().EndsWith(".png") || details.filename.ToLower().EndsWith(".jpg") || details.filename.ToLower().EndsWith(".jpeg")
                    || details.filename.ToLower().EndsWith(".bmp"))
                {
                    CloudBlockBlob imgThumbBlockBlob = blobContainer.GetBlockBlobReference("thumb_" + filename);
                    imgThumbBlockBlob.DeleteIfExistsAsync();
                    imgThumbBlockBlob.UploadFromStreamAsync(CreateThumbnail(ms));
                }
            }
            return filename;
        }

        private Stream CreateThumbnail(Stream input)
        {
            Bitmap orig = new Bitmap(input);

            int width;
            int height;
            if (orig.Width > orig.Height)
            {
                width = 128;
                height = 128 * orig.Height / orig.Width;
            }
            else
            {
                height = 128;
                width = 128 * orig.Width / orig.Height;
            }

            Bitmap thumb = new Bitmap(width, height);
            using (Graphics graphic = Graphics.FromImage(thumb))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphic.DrawImage(orig, 0, 0, width, height);
                MemoryStream ms = new MemoryStream();
                thumb.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }

        //Check auth
        public void SaveParentDefaultProfilePic(string userId, int gender)
        {
            string base64String = "";
            switch (gender)
            {
                case 2:
                    base64String = "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";
                    break;
                case 1:
                    base64String = "iVBORw0KGgoAAAANSUhEUgAAADMAAAAzCAYAAAA6oTAqAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAABYZJREFUeNrUml9oW1UYwE+ztmtsuy01W1nULabSyUQ3TIWJw0Zxb0argv8ebBBpXiMTRUHMXmQ+VPragtI+WJn/aBcFYSId60bBRVNhUotpU+Yyt3bpn7W0LCvx+67npjc3995zzz0na/3gkJDmnHt+5/vO9y+tKhQKRKb83vZeCF4Ow9gFw0+HKikYCzAy+P6Ri5+kZD67ShQGNo+b7aCjnXP6IowRGEM4AG5hU2CoBmIwnpd4uAMw4gCVuSMwVBP9DrTACxXj1ZSLEyQOL9MVBkHpxHsFz4tJ1wzVBtr1IXLnZRhGxI6WmDAAcphe0p1k82QcRogF5PofgBBqERm6H37NbCEQrcxgDDPTkCEMgOyigW0rgTBNzszMhrYoiGpyPbY0Q93vRyJP84TbSFM4SOofDZR8vnT2DzI3eI4sJ6dkQL0A2hkyhaEuOOVUK9sa3aSlN0rqWvdafg+hLse/Ius3V6XeH72ZxSsNgrKj/aDyXUHZT9Opcs1QrUw7XbmlL1pmViyZ+3KUZLsTIkCYqPpV7bh0WnEkDcEANwiK97WjpNbnEYFBK4qUmBl1xR0iF96p7Ag9JGpuMf2d6RBxxSKn2xBsEb47amaghXEsTkxsw3HUyXDTHVqYkMhKt67Ob3YgVfbvol5MKNrnBWBW/8zKgGlXNeMXXWlx5JLjuUsCc/U1l0vUxFDmE0myvrzGPW9t8qqs1AbF75KxCqYlmJ5wzQF43jlSewAsc7l84msukNXJrFQnUC1ztfnERZLP5khz9Jipu175dUoBuZWV7wGrZS+Id2C5q5e4W32kvi1AtjW4i+57JZmuCASVTHWlVkYTkmxGtmAyslbDMsANJUDdAV9RI0YBFk1RohcrMTNhGEw0d0MGbKeW0RdpObhnkmJNqmo8+C5mzI4MGTNe3/Ewqd0rlMYr8SbbfVpEWzNQ0/iV4gyiZ4pwdCvRnBDC82zQ8tSv9Z4p3ht0CL53wpZJ6bW+n2CccQIzADARNc6M8JbHViDZTxMkc3ygxAHg+zR4ufnvk6bzmrueIffFX3YCM6INmv22c4buNyzvBsaRucFR078rMcYiMcVDQq1zylARhv6CNcOa0dx1jFm75BJJW7kcq5zmqECHjXoAPaxqEs2AWQ6A22UJBk+WoLmhSduQHqPcrJ92O0y1ospUtBedBrn0VFzxRLxS42sq82a4Fq6JTkC5mw11ioYYcha0MlIGQ1UVN7v0Rhces+U0gGmB7DQ3sNupBcE1jBqCnnCQtVTcNGsGIFTZOKuDot2wHgih0Q1bxSb13hmBuA9szMX4ZbHWsFYrZiVAxKgvpvc4WheqBwr0RQ0vsPf1o8V5RiD4N+x2ljRL2gwdzqLRPs1+0kD1FZvnrYMxQ3eMMUNbYOlbtP9Vkmnl83o4EDVTMAMxMmUMvhizdFLWNDeFoUDoEDqp+Zm7WQMgjEVmLhy/jy1ZFYTVo8aYNBE+qf3oBOzH8G5b/qaJaQ6Y2KEAo8mtB1JNE4u0DVNcK/s5w26zHT0dhVfSFqfFWah2n/c8vB60zJqpeWiB1CKNlRbZybSxrID1LEGYmlHl5tjkX41HWpl9VCMNiYKg/P3xdwP3fvBiREpDA0AeyJ3+5bP1lbUCS0N2EkW7IPi8G9+OnbQDwtWdaXrusbdy34w9uTpxZYEFZBVnMEbZAQFrSN84deH+u1868n5FWk27O0Oj7gfv8eBp5a8v3jY3I/NmOKuQw8OaO3X+bbSGPW8+PcOzP0d9Mzytmj07a/ChK79N/2PkTnmSTDQn1MTsF+dexcPyvvJEj5N9Vcn457nrn/+8v9rb+KHLvf3xhR+S25dGJ1pYpcRdD++bLeRvX8nPLv3IY0oVhzGJUX5i3JRPif6TnJn8K8AAwd+h5JaM4DgAAAAASUVORK5CYII=";
                    break;
                default:
                    base64String = "iVBORw0KGgoAAAANSUhEUgAAADMAAAAzCAYAAAA6oTAqAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAABWxJREFUeNrcWk1sG0UUXtsrkfhHTmjakkaJTas4VYpC60oUqTQ1KrceGg6IcCjZAxzopfTSGzSFE0ioRkhcQIqjXoo41CBxQCJSXFGJVsqPgwpqotKYqE1LBHGwQxORKsxnds1kvbMzuztOIp40srTefTvfvL/vPdu3vr6u1EueH7l2kHw0UZdmfzjRO1uv9/lkgSEbT5EPYwFE1Ob2AoCRNUpWlgCc3HIw+sm/TVYfZ/M8AbgsWWkvlnMFRrfCIFnH6+Atw9DtBpQjMAQE/D9D1iml/vKxDqooHQwB0qcDiSqbJ3C/PtGY8gsCSZOPq5sMBBIja4K8X5MChiiCNc4qWytD+oG6dzMdyICyfWSYuJzm2DLbEAhkwM7lLC2jPzCkbF95kVholAtGL4QTXt92fOcOpTMS3nDt2sLvynSpLAPMEllxc9pWLW7MeAVxLrFPeaqhoea7N56OKROLS8qlmTteQSGrIiFoTMt4dS+AeLW9Teje93+6rXwz/1Cqu5kTwKBbrSdbdwsDgbzT3aUkm5u8ghm0zGa6VWJuNEZUtWIVp/Jud8JzaOo8scYynqwSVlXHzyGuEGMeRdsARs9gMfdB3+J6JxJcbUAnwFXLaF60tTY+4frZRDgsI1X30WBSXjRZpeFNlhQN5lkvmlA73Mr9lRV5YOhssBUbmpHDCGKGZeJeNbktfuW1NRmFs9rKSwEzvlis8C6ncmXunlIigGSJX5Yi0JOZ8rIja37+S0FmEohLA4MTPjOWF0oGXxCLALxkiasytQHQW+P5SiHsJzwt2RytMoMHJEmMEaAAIqkNqBG1HkoRQ1ibLKN+5X8ksAz6gQsyOku4F+jJoeaobTqeKS0r0+VyJQlIdLmi78h3OaTmu14Y85t7Y64pDWLpM5LVvNYb0qT5Kp0mKThFxeGAL0H6ezRYneGQbfqd19kBLNZrQ/eRBc9P3XJbd3IETMpIAHC1U06sgWaM1cOg3iD1ml0IB/BpssfyObjm1aPPKWfGp9y43ihdNLNOgMAiLCCIifP5W5YbwjVsliXQCbCJiOO2IOsYDF7Ca49p17ISALKLDwNQRLxzLRiDdb8ePIiZYZEhBK89zglwNF4NwjvwLkHJWHGzDM+9jGCv8Kq7hUo1dyPzK6uWFAc6DTrUq6d6juDmdA0Yff6UYz2F9GsmiZem79RwrIRNdqNrkpmkQhd0jhX/s1o/f3SVpqeaftakwzx0oOtIRA1sAEYD6u9o446lYGUaCB1DdKzAOjaxs8EqNWD03xEv1oKJ1sQOnXFoQADN8nds7MOeA9W4MwOxGiQm2WxCM8+a/RaVFPOzPH3tcFMTN4XSgLCpy0cOVz5bCTisf68lq1THCojVITDi5iuyz6woa8boZtJgBZ2REDOF0kXO2JzBDFgWEgXCGEXlWeHgZ/CcWWPigVNlpWOWhV6/OW7ZpKG1fvn6TWEgBjMwxYnG+gWa9zOgRjLP0AfEz3mDCSsaApDGQeAe8/c8IIbgcMizAJKy++XZz2GimYZA4KXltcfrvCJnRUOweaNRcwsEsjcUXOABERpoXDywf+Tr+/OvFf569MgNIDt+JyK3S+ViwOfrEfkvgPCfGjKzv8b2hUPZYy07DvJcDnHBovKIQbBjERn5bSF3YtdO4SGlcNusxTsKBMihL+funcNp2Vmo08Y6J/fsFrLG5cLcMSdAXM3NXmlvS3dFws08UEzL/c1uviaLSw+gF/pPx9q/d6rb8//NyAm+EA8FP+kIBrtiwcZGNGanb4zZ0hm4mZHlEIs//1n6dmF19SM3AKSCoeXsxI+7bvyx2K3XKJRuq/hCIBePtjw5FQqo1997Zv9DWe//R4ABAKP0i2KVkEIwAAAAAElFTkSuQmCC";
                    break;
            }
            byte[] bytes = Convert.FromBase64String(base64String);
            CloudBlobContainer blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
            string filename = userId + ".png";
            CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
            imgBlockBlob.DeleteIfExistsAsync();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                imgBlockBlob.UploadFromStreamAsync(ms);
            }
        }

        //check children
        public void SaveParentDefaultRelationPic(string userId, string channelId, int relationId)
        {
            string base64String = "";
            switch (relationId)
            {
                case 2:
                    base64String = "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";
                    break;
                case 1:
                    base64String = "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";
                    break;
                case 3:
                    base64String = "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";
                    break;
                default:
                    base64String = "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";
                    break;
            }
            byte[] bytes = Convert.FromBase64String(base64String);
            CloudBlobContainer blobContainer = GetBlobContainerCreateIfDoesntExists(Shared.ProfileContainer);
            string filename = userId + "_" + channelId + ".png";
            CloudBlockBlob imgBlockBlob = blobContainer.GetBlockBlobReference(filename);
            imgBlockBlob.DeleteIfExistsAsync();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                imgBlockBlob.UploadFromStreamAsync(ms);
            }
        }

        //public void CopyAndRenameProfilePicInAzureStorage(string containerName, string filename, string profileId)
        //{
        //    CloudBlobContainer blobContainer = GetBlobContainerCreateIfDoesntExists(containerName);
        //    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filename);

        //    CloudBlockBlob newBlob = blobContainer.GetBlockBlobReference(profileId + ".jpg");
        //    newBlob.CopyFromBlob(blob);
        //    blob.Delete();
        //}


        //Not used but with error

        public IEnumerable<BlobInfo> GetListOfBlobsFromContainer(string containerName)
        {
            List<BlobInfo> blobInfos = new List<BlobInfo>();

            CloudBlobContainer container = GetBlobContainerCreateIfDoesntExists(containerName);

            dynamic temp = container.ListBlobsSegmentedAsync(null);
            // Only traverse first level no directories, no pages
            foreach (IListBlobItem item in temp)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    BlobInfo blobInfo = new BlobInfo();
                    blobInfo.Uri = blob.Uri;
                    blobInfo.Filename = blob.Name;
                    blobInfo.ParentContainerName = blob.Parent.Container.Name;
                    blobInfo.FileLengthKilobytes = ConvertBytesToKilobytes(blob.Properties.Length);

                    blobInfos.Add(blobInfo);
                }
            }
            return blobInfos;
        }

        public Stream DownloadBlob(BlobInfo blobInfo)
        {
            Stream stream = new MemoryStream();
            CloudBlobContainer container = GetBlobContainerCreateIfDoesntExists(blobInfo.ParentContainerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobInfo.Filename);
            blockBlob.DownloadToStreamAsync(stream);
            return stream;
        }

        private CloudBlobContainer GetBlobContainerCreateIfDoesntExists(string containerName)
        {
            CloudBlobContainer container = null;
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();
            container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container;
        }

        private long ConvertBytesToKilobytes(long byteslength)
        {
            return (byteslength / 1024);
        }

    }

    public class FilesStatus
    {
        public const string HandlerPath = "/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        //public string progress { get; set; }
        //public string url { get; set; }
        public string error { get; set; }
        public class FileUploadDetails
        {
            public string filetype { get; set; }
            public string filename { get; set; }
            public string filesize { get; set; }
            public string base64 { get; set; }
            public bool isProfile { get; set; }
            public bool isSchool { get; set; }
            public int profileId { get; set; }
        }
        public FilesStatus()
        {
        }

        public FilesStatus(FileInfo fileInfo)
        {
            SetValues(fileInfo.Name, (int)fileInfo.Length);
        }

        public FilesStatus(string fileName, int fileLength)
        {
            GetType(fileName);
            SetValues(fileName, fileLength);
        }

        private void GetType(string fileName)
        {

        }

        private void SetValues(string fileName, int fileLength)
        {
            name = fileName;
            type = FileExtensionToMimeType.GetMimeType(fileName.Substring(fileName.LastIndexOf('.')));
            size = fileLength;
            //progress = "1.0";
            //url = HandlerPath + "api/Upload?f=" + fileName;
        }
    }

    public class FilesStatus2
    {
        public const string HandlerPath = "/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        //public string progress { get; set; }
        //public string url { get; set; }
        public string error { get; set; }
        //public class FileUploadDetails2
        //{
        //    public string filetype { get; set; }
        //    public string filename { get; set; }
        //    public string filesize { get; set; }
        //    public string base64 { get; set; }
        //    public bool isProfile { get; set; }
        //    public bool isSchool { get; set; }
        //    public Guid? schoolId { get; set; }
        //    public Guid? messageId { get; set; }
        //    public string Subject { get; set; }
        //    public string Attachment { get; set; }
        //    public int mType { get; set; }
        //    public string profileId { get; set; }
        //}

        public class FileUploadDetails2
        {
            public string filetype { get; set; }
            public string filename { get; set; }
            public int filesize { get; set; }
            public string base64 { get; set; }
            public bool isProfile { get; set; }
            public bool isSchool { get; set; }
            public string schoolId { get; set; }
            public string messageId { get; set; }
            public string Subject { get; set; }
            public string Attachment { get; set; }
            public string mType { get; set; }
            public string profileId { get; set; }
        }

        public class FileUploadDetails3
        {
            public string filetype { get; set; }
            public string filename { get; set; }
            public int filesize { get; set; }
            public string base64 { get; set; }
            public bool isProfile { get; set; }
            public bool isSchool { get; set; }
            public int? schoolId { get; set; }
            public string messageId { get; set; }
            public string Subject { get; set; }
            public string Attachment { get; set; }
            public string mType { get; set; }
            public int? profileId { get; set; }
        }

        public FilesStatus2()
        {
        }

        public FilesStatus2(FileInfo fileInfo)
        {
            SetValues(fileInfo.Name, (int)fileInfo.Length);
        }

        public FilesStatus2(string fileName, int fileLength)
        {
            GetType(fileName);
            SetValues(fileName, fileLength);
        }

        private void GetType(string fileName)
        {

        }

        private void SetValues(string fileName, int fileLength)
        {
            name = fileName;
            type = FileExtensionToMimeType.GetMimeType(fileName.Substring(fileName.LastIndexOf('.')));
            size = fileLength;
            //progress = "1.0";
            //url = HandlerPath + "api/Upload?f=" + fileName;
        }
    }

    public class WebAPIStreamResponseHelper
    {
        public HttpResponseMessage returnStreamResponseInformingMimeType(string fileName, Stream stream)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            string fileExtension = Path.GetExtension(fileName);
            result.Content.Headers.ContentType =
                                new MediaTypeHeaderValue(FileExtensionToMimeType.GetMimeType(fileExtension));
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return result;
        }

    }
    public class FileExtensionToMimeType
    {
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region Big list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
    }


}
