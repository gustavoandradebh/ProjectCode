using System.ComponentModel.DataAnnotations;

namespace ProjectCode.Domain.DataTransferObject
{
    public class Sdlc_System: BaseEntityDTO
    {
        [Required]
        [MaxLength(255)]
        public string baseUrl { get; set; }

        [MaxLength(255)]
        public string description { get; set; }
    }
}
