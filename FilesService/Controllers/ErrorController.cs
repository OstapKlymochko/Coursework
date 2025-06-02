using Common.CommonTypes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FilesService.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult GetErrorResponse([FromServices] IHostEnvironment hostEnvironment)
        {
            Response.StatusCode = 500;
            if (hostEnvironment.IsDevelopment())
            {
                var exceptionHandlerFeature =
                    HttpContext.Features.Get<IExceptionHandlerFeature>()!;

                return Problem(
                    detail: exceptionHandlerFeature.Error.StackTrace,
                    title: exceptionHandlerFeature.Error.Message);
            }
            return new JsonResult(new BasicResponse("Internal server error"));
        }
    }
}
