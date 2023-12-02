using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using TestApp.AppServices.Helpers;
using TestApp.Data.Repositories;
using TestApp.Models;

namespace TestApp.AppServices.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly DepartmentRepository _departmentRepository;
        private readonly JobTitleRepository _jobTitleRepository;

        public EmployeeService(EmployeeRepository employeeRepository, DepartmentRepository departmentRepository, JobTitleRepository jobTitleRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _jobTitleRepository = jobTitleRepository;
        }

        public async Task Import(IFormFile data)
        {
            var employees = await TsvHelper.ParseData(data, SetEmployee);
            var names = employees.Select(x=>x.FullName);
            var dbEmployees = await _employeeRepository.GetByNames(names);
            var updateEmployees = dbEmployees
                .IntersectBy(employees.Select(x => x.FullName.ToLower()), x => x.FullName.ToLower()).ToList();
            var newEmployees = employees
                .ExceptBy(dbEmployees.Select(x => x.FullName.ToLower()), x => x.FullName.ToLower()).ToList();

            await SetEmployeeInfo(employees, updateEmployees, newEmployees);

            _employeeRepository.UpdateRange(updateEmployees);
            await _employeeRepository.AddRangeAsync(newEmployees);
            await UpdateDepartmentManagers(newEmployees);
            await _employeeRepository.SaveChangesAsync();
        }

        private async Task SetEmployeeInfo(List<Employee> employees, List<Employee> updateEmployees, 
            List<Employee> newEmployees)
        {
            var departments = await _departmentRepository.GetByName(employees.Select(x => x.TsvDepartmentName));
            var jobTitles = await _jobTitleRepository.GetByNames(employees.Select(x => x.TsvJobTitleName));

            foreach (var employee in updateEmployees)
            {
                var newData = employees.FirstOrDefault(x => x.FullName.ToLower().Equals(employee.FullName.ToLower()));
                if(newData == null)
                    continue;
                employee.Password = newData.Password;
                employee.Login = newData.Login;
                employee.TsvDepartmentName = newData.TsvDepartmentName;
                employee.TsvJobTitleName = newData.TsvJobTitleName;
                SetForeignKeys(employee, departments, jobTitles);
            }

            foreach (var employee in newEmployees)
            {
                SetForeignKeys(employee, departments, jobTitles);
            }
        }

        private static void SetForeignKeys(Employee employee, List<Department> departments, List<JobTitle> jobTitles)
        {
            employee.Department = departments.FirstOrDefault(x =>
                x.Name.Equals(employee.TsvDepartmentName, StringComparison.OrdinalIgnoreCase));
            employee.JobTitle = jobTitles.FirstOrDefault(x =>
                x.Name.Equals(employee.TsvJobTitleName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task UpdateDepartmentManagers(List<Employee> employees)
        {
            var names = employees.Select(x => x.FullName);
            var departments = await _departmentRepository.GetByManagers(names);
            foreach (var department in departments)
            {
                department.Manager = employees.FirstOrDefault(x => x.FullName.Equals(department.TsvManagerName, StringComparison.OrdinalIgnoreCase));
            }
            _departmentRepository.UpdateRange(departments);
        }

        private static Employee SetEmployee(List<string> fields)
        {
            return new Employee
            {
                TsvDepartmentName = fields[0],
                FullName = fields[1],
                Login = fields[2],
                Password = fields[3],
                TsvJobTitleName = fields[4],
            };
        }
    }
}
