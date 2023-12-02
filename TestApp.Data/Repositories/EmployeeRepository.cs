using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>
    {
        public EmployeeRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<Employee>> GetByDepartments(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.TsvDepartmentName.ToLower())).ToListAsync();
        }

        public async Task<List<Employee>> GetByNames(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.FullName.ToLower())).ToListAsync();
        }

        public async Task<List<Employee>> GetByJobs(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.TsvJobTitleName.ToLower())).ToListAsync();
        }
    }
}
