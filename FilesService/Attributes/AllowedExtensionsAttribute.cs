using FilesService.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FilesService.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null && !FileValidator.IsFileExtensionAllowed(file, _extensions)) return new ValidationResult($"{file.ContentType} is not allowed ({string.Join(",", _extensions)})");
            return ValidationResult.Success!;
        }
    }
}
