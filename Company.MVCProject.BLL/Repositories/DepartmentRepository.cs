using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.DAL.Data.Contexts;
using Company.MVCProject.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.MVCProject.BLL.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(CompanyDbContext context) : base(context)
        {
            
        }

    }
}
