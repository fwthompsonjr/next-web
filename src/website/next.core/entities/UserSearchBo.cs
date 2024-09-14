using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace next.core.entities
{
    internal class UserSearchBo
    {
        private IEnumerable<CountyParameterModel> _parameters =
            Enumerable.Empty<CountyParameterModel>();

        public string UserName { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        public string State { get; set; } = string.Empty;
        [Required]
        public UserSearchCounty County { get; set; } = new();
        [Required]
        [JsonProperty("start")]
        public long? StartDate { get; set; }
        [Required]
        [JsonProperty("end")]
        public long? EndDate { get; set; }

        [JsonProperty("details")]
        public IEnumerable<CountyParameterModel> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                SearchStarted = DateTime.UtcNow;
                ParameterChanged?.Invoke();
            }
        }

        public Action? ParameterChanged { get; internal set; }
        internal DateTime? SearchStarted { get; set; }
    }
}
