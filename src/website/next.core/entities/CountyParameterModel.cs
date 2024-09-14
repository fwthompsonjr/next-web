using System.ComponentModel.DataAnnotations;

namespace next.core.entities
{
    internal class CountyParameterModel
    {
        [Required]
        public string? Name { get; set; } = string.Empty;
        [Required]
        public string? Text { get; set; } = string.Empty;
        [Required]
        public string? Value { get; set; } = string.Empty;
    }
}
