using System;
using System.Collections.Generic;

namespace MimaCare.Application.Dtos.Branch
{
    public class AssignDoctorBranchesDto
    {
        public List<Guid> BranchIds { get; set; }
            = new List<Guid>();
    }
}