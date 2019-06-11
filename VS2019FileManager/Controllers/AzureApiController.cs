using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Mvc.FileManagement;

namespace E.VS2019FileManager.Controllers
{
	[Route("api/[controller]")]
	public class AzureApiController : Controller
	{
		IHostingEnvironment _hostingEnvironment;

		public AzureApiController(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
		}
		public IActionResult FileSystem(FileSystemCommand command, string arguments)
		{
			var config = new FileSystemConfiguration
			{
				Request = Request,
				FileSystemProvider = new AzureBlobFileProvider(),
				AllowCopy = true,
				AllowCreate = true,
				AllowMove = true,
				AllowRemove = true,
				AllowRename = true,
				AllowUpload = true,
				UploadTempPath = _hostingEnvironment.ContentRootPath + "/wwwroot/UploadTemp"
			};
			var processor = new FileSystemCommandProcessor(config);
			var result = processor.Execute(command, arguments);
			return Ok(result.GetClientCommandResult());
		}
	}
}
