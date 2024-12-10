using Microsoft.AspNetCore.Mvc;
using correos_backend.Services;

namespace correos_backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class FilesS3Controller : ControllerBase
{
	private readonly S3Service _s3Service;

	public FilesS3Controller(S3Service s3Service)
	{
		_s3Service = s3Service;
	}

	[HttpGet]
	public IActionResult GeneratePresignedUrl(string fileName)
	{

		if(string.IsNullOrEmpty(fileName))
		{
			return BadRequest("File name is required");
		}
		var url = _s3Service.GeneratePresignedUrl(fileName, 10);
		return Ok(new { url });
	}


}
