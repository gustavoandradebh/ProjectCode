using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCode.Domain.Interfaces
{
    public interface IProjectValidator
    {
        bool ValidatePostProject(ProjectModel project);

        bool ValidatePatchProject(long projectId, Project projectDTO, dynamic projectPatchInput);
    }
}
