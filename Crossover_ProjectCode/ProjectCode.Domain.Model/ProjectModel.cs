using System.ComponentModel.DataAnnotations;

namespace ProjectCode.Domain.Model
{
    public class ProjectModel: BaseEntity
    {
        [MaxLength(255)]
        public string externalId { get; set; }

        [MaxLength(255)]
        public string name { get; set; }

        public SdlcSystemModel sdlcSystem { get; set; }
    }
}
