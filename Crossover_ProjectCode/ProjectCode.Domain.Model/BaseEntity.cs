using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectCode.Domain.Model
{
    public class BaseEntity
    {
        public long? id { get; set; }

        public DateTime? createdDate { get; set; }

        public DateTime? lastModifiedDate { get; set; }
    }
}
