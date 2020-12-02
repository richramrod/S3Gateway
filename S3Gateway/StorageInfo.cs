using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S3Gateway
{
    public class StorageInfo
    {
      public string BucketName { get; set; }
      public string AccessKey { get; set; }
      public string SecretKey { get; set; }
      public string Region { get; set; }
      public string BucketPath { get; set; }
    }
}
