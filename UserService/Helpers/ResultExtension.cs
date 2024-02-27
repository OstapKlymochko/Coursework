using Common.Errors;
using Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Helpers
{
	public static class ResultExtension
	{
		public static IActionResult MapToResponse<TRes>(this ServiceResult<TRes> result)
		{
			if (result.IsSuccess) return new OkObjectResult(result.Value);
			switch (result.Error)
			{
				case NotFoundError:
					return new NotFoundObjectResult(result.Error);
				case ModelError:
					return new BadRequestObjectResult(result.Error);
				default:
					return new StatusCodeResult(500);
			}
		}
	}
}
