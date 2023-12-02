using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Data.Extensions;
using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>
    {
        public DepartmentRepository(DbContext context) : base(context)
        {
        }

        public async Task<(Department department, int level)> GetDepartmentById(long id)
        {
            var department = await Table()
                .IncludeAllDepartments()
                .Include(x => x.Employees)
                .ThenInclude(x => x.JobTitle)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (department == null)
                return (null, 0);
            var level = 0;
            var parent = department;
            do
            {
                level++;
                while (parent.Parent != null)
                {
                    level++;
                    parent = parent.Parent;
                }

                if (parent.ParentId != null)
                    parent = await GetDepartmentParentsById(parent.ParentId.Value);
            } while (parent != null && parent.ParentId != null);
            return (department, level);
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await Table()
                .Include(x => x.Employees).ThenInclude(x => x.JobTitle)
                .Include(x => x.Children)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        private async Task<Department> GetDepartmentParentsById(long id)
        {
            return await Table().IncludeAllDepartments().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Department>> GetByName(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.Name.ToLower())).ToListAsync();
        }

        public async Task<List<Department>> GetByNameOrParent(IEnumerable<string> names, IEnumerable<string> parentNames)
        {
            return await Table()
                .Where(x => names.Select(x => x.ToLower()).Contains(x.Name.ToLower()) || 
                            parentNames.Select(x => x.ToLower()).Contains(x.Name.ToLower())).ToListAsync();
        }

        public async Task<List<Department>> GetParentByName(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.TsvParentName.ToLower())).ToListAsync();
        }

        public async Task<List<Department>> GetByManagers(IEnumerable<string> employees)
        {
            return await Table().Where(x => employees.Select(x => x.ToLower()).Contains(x.TsvManagerName.ToLower())).ToListAsync();
        }
    }
}
