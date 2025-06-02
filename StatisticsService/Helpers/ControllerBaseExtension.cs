using Microsoft.AspNetCore.Mvc;

namespace StatisticsService.Helpers
{
	public static class ControllerBaseExtension
	{
		public static int ExtractIdFromToken(this ControllerBase ctx)
			=> int.Parse(ctx.HttpContext.User.Claims.First(c => c.Type == "userId").Value);
	}
}
