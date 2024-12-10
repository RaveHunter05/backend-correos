using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


namespace correos_backend.Services
{
	public class S3Service
	{
		private readonly IAmazonS3 _s3Client;
		private readonly string _bucketName = "correosnicaragua";

		private readonly string _accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
		private readonly string _secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");




		public S3Service(IConfiguration configuration)
		{
			var s3Config = new AmazonS3Config
			{
				RegionEndpoint = RegionEndpoint.USEast1,
				SignatureVersion = "v4",
			};

			_s3Client = new AmazonS3Client(
					_accessKey,
					_secretKey,
					s3Config
					);
		}

		public string GeneratePresignedUrl(string key, int durationInMinutes){
			var request = new GetPreSignedUrlRequest
			{
				BucketName = _bucketName,
					   Key = key,
					   Verb = HttpVerb.PUT,
					   Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
					   ContentType = "application/octet-stream"
			};

			try {
				return _s3Client.GetPreSignedURL(request);
			} catch (AmazonS3Exception e) {
				throw new Exception(e.Message);
			}
		}

	}
}
