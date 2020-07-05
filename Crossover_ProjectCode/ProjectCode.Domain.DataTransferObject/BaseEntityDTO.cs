using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCode.Domain.DataTransferObject
{
    public class BaseEntityDTO
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        public DateTime createdDate { get; set; }

        [Required]
        public DateTime lastModifiedDate { get; set; }
    }
}
