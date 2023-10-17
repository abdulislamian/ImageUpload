using System.ComponentModel.DataAnnotations;

namespace TaskOne.Models
{
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly string[] AllowedExtensions;
        private readonly long MaxSizeInBytes;

        public FileValidationAttribute(string[] allowedExtensions, long maxSizeInBytes)
        {
            AllowedExtensions = allowedExtensions;
            MaxSizeInBytes = maxSizeInBytes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<IFormFile> files)
            {
                if (files.Count > 5)
                {
                    return new ValidationResult($"Maximum 5 Files are Allowed to Attach.");
                }
                foreach (var file in files)
                {

                    if (file.Length > MaxSizeInBytes)
                    {
                        return new ValidationResult($"File size exceeds the maximum allowed size of {MaxSizeInBytes / 1024} KB.");
                    }

                    var fileExtension = Path.GetExtension(file.FileName);
                    if (!AllowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        return new ValidationResult($"Only {string.Join(", ", AllowedExtensions)} files are allowed.");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}