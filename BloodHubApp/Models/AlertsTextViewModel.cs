using System.ComponentModel.DataAnnotations;

namespace BloodHub.Models
{
    namespace BloodHub.Models
    {
        public class AlertsTextViewModel
        {
            [Required]
            [StringLength(1000)]
            public string Text { get; set; } = string.Empty;
        }
    }

}
