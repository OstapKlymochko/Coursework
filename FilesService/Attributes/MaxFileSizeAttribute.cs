using FilesService.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FilesService.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;
        public MaxFileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null && !FileValidator.IsFileSizeWithinLimit(file, _maxFileSize)) return new ValidationResult(this.ErrorMessage);
            return ValidationResult.Success!;
        }
    }
}
