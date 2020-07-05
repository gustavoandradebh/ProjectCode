using System.ComponentModel.DataAnnotations;

namespace ProjectCode.Domain.DataTransferObject
{
    public class Project: BaseEntityDTO
    {
        [Required]
        [MaxLength(255)]
        public string externalId { get; set; }

        [MaxLength(255)]
        public string name { get; set; }

        [Required]
        public long sdlcSystemId { get; set; }
    }
}
