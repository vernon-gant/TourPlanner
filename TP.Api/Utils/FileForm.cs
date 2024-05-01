using System.ComponentModel.DataAnnotations;

namespace TP.Api.Utils;

public class FileForm
{
    [Required]
    public required IFormFile SubmittedFile { get; set; }
}