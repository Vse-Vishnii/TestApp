using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TestApp.Models;

namespace TestApp.Data.Extensions
{
    public static class IncludableQueryableExtension
    {
        public static IIncludableQueryable<Department, Department> IncludeAllDepartments(this IQueryable<Department> query, 
            IIncludableQueryable<Department, Department> includable = null, int depth = 10)
        {
            if (depth <= 0)
                return includable;

            includable ??= query.Include(d => d.Parent);
            includable = includable.ThenInclude(x => x.Parent);
            
            return includable.IncludeAllDepartments(includable, depth - 1);
        }
    }
}
