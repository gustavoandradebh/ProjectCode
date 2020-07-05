using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProjectCode.Domain.Model
{
    [ExcludeFromCodeCoverage]
    public class SdlcSystemModel: BaseEntity
    {
        [MaxLength(255)]
        public string baseUrl { get; set; }

        [MaxLength(255)]
        public string description { get; set; }
    }
}
