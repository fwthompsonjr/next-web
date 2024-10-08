using System.ComponentModel.DataAnnotations;

namespace next.web.core.models
{
    public class CountyCodeRequest
    {
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
