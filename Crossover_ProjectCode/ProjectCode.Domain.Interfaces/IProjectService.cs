using ProjectCode.Domain.Model;
using System.Threading.Tasks;

namespace ProjectCode.Domain.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectModel> Get(long id);

        Task<ProjectModel> Create(string project);

        Task<ProjectModel> Update(long id, string project);
    }
}
