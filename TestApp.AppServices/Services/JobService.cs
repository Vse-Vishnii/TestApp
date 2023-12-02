using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.AppServices.Helpers;
using TestApp.Data.Repositories;
using TestApp.Models;

namespace TestApp.AppServices.Services
{
    public class JobService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly JobTitleRepository _jobTitleRepository;

        public JobService(EmployeeRepository employeeRepository, JobTitleRepository jobTitleRepository)
        {
            _employeeRepository = employeeRepository;
            _jobTitleRepository = jobTitleRepository;
        }

        public async Task Import(IFormFile data)
        {
            var jobs = await TsvHelper.ParseData(data, SetJob, 1);
            var names = jobs.Select(x => x.Name);
            var dbJobs = await _jobTitleRepository.GetByNames(names);
            var newJobs = jobs.ExceptBy(dbJobs.Select(x => x.Name), x => x.Name).ToList();
            
            await _jobTitleRepository.AddRangeAsync(newJobs);
            await UpdateEmployeeJob(newJobs);
            await _jobTitleRepository.SaveChangesAsync();
        }

        private async Task UpdateEmployeeJob(List<JobTitle> jobs)
        {
            var names = jobs.Select(x => x.Name);
            var employees = await _employeeRepository.GetByJobs(names);
            foreach (var employee in employees)
            {
                employee.JobTitle = jobs.FirstOrDefault(x => x.Name.Equals(employee.TsvJobTitleName, StringComparison.OrdinalIgnoreCase));
            }
            _employeeRepository.UpdateRange(employees);
        }

        private static JobTitle SetJob(List<string> fields)
        {
            return new JobTitle()
            {
                Name = fields[0],
            };
        }
    }
}
