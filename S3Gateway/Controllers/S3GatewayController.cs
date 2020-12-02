using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace S3Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3GatewayController : ControllerBase
    {
        private List<StorageInfo> _storageInfo;
        public S3GatewayController(IOptions<List<StorageInfo>> storageInfo)
        {
            _storageInfo = storageInfo.Value;
        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var storage = _storageInfo.Where(x => x.BucketName == "Bucket").FirstOrDefault();

            Stream fileStream = await ReadObjectData(storage, fileName);
            Response.Headers.Add("Content-Disposition", new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            }.ToString());

            return File(fileStream, MimeTypesMap.GetMimeType(fileName));
        }

        private async Task<Stream> ReadObjectData(StorageInfo info, string fileName)
        {
            try
            {
                using (var client = new AmazonS3Client(info.AccessKey, info.SecretKey, RegionEndpoint.GetBySystemName(info.Region)))
                {
                    var request = new GetObjectRequest
                    {
                        BucketName = info.BucketPath,
                        Key = fileName
                    };

                    using(var getObjectResponse = await client.GetObjectAsync(request))
                    using(var responseSteam = getObjectResponse.ResponseStream)
                    {
                        var stream = new MemoryStream();
                        await responseSteam.CopyToAsync(stream);
                        stream.Position = 0;
                        return stream;
                    }
                    
                }
            }
            catch(Exception exception)
            {
                throw new Exception("Read object operation failed.", exception);
            }
        }
    }
}