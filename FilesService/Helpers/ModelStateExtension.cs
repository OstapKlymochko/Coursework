using Common.CommonTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FilesService.Helpers
{
    public static class ModelStateExtension
    {
        public static BasicResponse GetModelErrors(this ModelStateDictionary ModelState)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage);
            return new BasicResponse(string.Join(", ", errors));
        }
    }
}
