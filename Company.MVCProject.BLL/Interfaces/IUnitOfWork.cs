using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.MVCProject.BLL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IDepartmentRepository DepartmentRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        Task<int> CompleteAsync();
    }
}
