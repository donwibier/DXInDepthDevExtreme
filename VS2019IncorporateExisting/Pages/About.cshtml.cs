using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VS2019IncorporateExisting.Pages
{
	public class AboutModel : PageModel
	{
		public string Message { get; set; }

		public void OnGet()
		{
			Message = "Your application description page.";
		}
	}
}
