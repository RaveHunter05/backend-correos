using Amazon;
using Amazon.S3;
using Amazon.S3.Model;


using System.Text;
using System.Security.Cryptography;



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

		// hash name
		public string GenerateHashName(string fileName)
		{

			var extension = fileName.Split('.').Last();
			var hash = new StringBuilder();
			using (var algorithm = SHA256.Create())
			{
				var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(fileName));
				foreach (var t in bytes)
				{
					hash.Append(t.ToString("X2"));
				}
			}

			hash.Append($".{extension}");
			return hash.ToString();
		}

		public string GeneratePutPresignedUrl(string key, int durationInMinutes){

			var request = new GetPreSignedUrlRequest
			{
				BucketName = _bucketName,
					   Key = key,
					   Verb = HttpVerb.PUT,
					   Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
					   ContentType = "application/octet-stream"
			};

			try {
				var url = _s3Client.GetPreSignedURL(request);
				return url;
			} catch (AmazonS3Exception e) {
				throw new Exception(e.Message);
			}
		}

		public string GenerateGetPresignedUrl(string key, int durationInMinutes){
			var request = new GetPreSignedUrlRequest
			{
				BucketName = _bucketName,
					   Key = key,
					   Verb = HttpVerb.GET,
					   Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
			};

			try {
				return _s3Client.GetPreSignedURL(request);
			} catch (AmazonS3Exception e) {
				throw new Exception(e.Message);
			}
		}

	}
}
