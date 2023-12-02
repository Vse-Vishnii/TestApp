using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public class JobTitleRepository : BaseRepository<JobTitle>
    {
        public JobTitleRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<JobTitle>> GetByNames(IEnumerable<string> names)
        {
            return await Table().Where(x => names.Select(x => x.ToLower()).Contains(x.Name.ToLower())).ToListAsync();
        }
    }
}
