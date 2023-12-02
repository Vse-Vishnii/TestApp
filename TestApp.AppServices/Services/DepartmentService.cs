using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using TestApp.AppServices.Helpers;
using TestApp.Data.Repositories;
using TestApp.Models;
using TestAppModels.Enums;

namespace TestApp.AppServices.Services
{
    public class DepartmentService
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly DepartmentRepository _departmentRepository;

        public DepartmentService(EmployeeRepository employeeRepository, DepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task Import(IFormFile data)
        {
            var departments = await TsvHelper.ParseData(data, SetDepartment, 4);
            var names = departments.Select(x => x.Name);
            var parentNames = departments.Select(x=>x.TsvParentName).Distinct();
            var dbDepartments = await _departmentRepository.GetByNameOrParent(names, parentNames);
            var updateDepartments = dbDepartments.IntersectBy(departments.Select(x => x.Name), x => x.Name).ToList();
            var newDepartments = departments.ExceptBy(dbDepartments.Select(x => x.Name), x => x.Name).ToList();
            await SetDepartmentInfo(newDepartments, departments, updateDepartments, dbDepartments);

            _departmentRepository.UpdateRange(updateDepartments);
            await _departmentRepository.AddRangeAsync(newDepartments);
            await UpdateEmployeeDepartments(newDepartments);
            await UpdateParentDepartments(newDepartments);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task<string> GetDepartmentsById(long id)
        {
            var result = await _departmentRepository.GetDepartmentById(id);
            var department = result.department;
            if (department == null)
                return "Отдел не найден";
            var level = result.level;

            var builder = new StringBuilder();
            var depSymbolBuilder = new StringBuilder();
            for (var i = 0; i < level; i++)
            {
                depSymbolBuilder.Append("=");
            }
            var depInfo = $"{depSymbolBuilder.ToString()} {department.Name} ID={department.Id}\n";
            builder.Append(depInfo);

            SetEmployeesInfo(department, builder);

            while (department.Parent != null)
            {
                department = department.Parent;
                depSymbolBuilder.Remove(depSymbolBuilder.Length - 1, 1);
                depInfo = $"{depSymbolBuilder.ToString()} {department.Name} ID={department.Id}\n";
                builder.Insert(0, depInfo);
            }

            return builder.ToString();
        }

        public async Task<string> GetDepartments()
        {
            var departments = await _departmentRepository.GetAllDepartments();
            var builder = new StringBuilder();
            var depSymbol = "=";
            SetDepartmentsInfo(builder, depSymbol, departments);

            return builder.ToString();
        }

        private async Task SetDepartmentInfo(List<Department> newDepartments, List<Department> departments, 
            List<Department> updateDepartments, List<Department> dbDepartments)
        {
            var groupedNewDepartments = newDepartments.GroupBy(x => x.TsvParentName);
            var managerNames = departments.Select(x => x.TsvManagerName);
            var managers = await _employeeRepository.GetByNames(managerNames);
            foreach (var department in updateDepartments)
            {
                var newData = departments.FirstOrDefault(x => x.Name.ToLower().Equals(department.Name.ToLower()));
                if (newData == null)
                    continue;
                department.Phone = newData.Phone;
                department.Manager = managers.FirstOrDefault(x => CompareManager(x, newData));
            }

            foreach (var group in groupedNewDepartments)
            {
                if (group.Key.Equals(string.Empty))
                    foreach (var department in group)
                        department.Manager = managers.FirstOrDefault(x => CompareManager(x, department));
                else
                {
                    var parent = dbDepartments.FirstOrDefault(x => CompareNameAndKey(x, group)) ??
                                 departments.FirstOrDefault(x => CompareNameAndKey(x, group));
                    foreach (var department in group)
                    {
                        department.Parent = parent;
                        department.Manager = managers.FirstOrDefault(x => CompareManager(x, department));
                    }
                }
            }
        }

        private static bool CompareManager(Employee x, Department department)
        {
            return x.FullName.Equals(department.TsvManagerName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CompareNameAndKey(Department x, IGrouping<string?, Department> group)
        {
            return x.Name.Equals(group.Key, StringComparison.OrdinalIgnoreCase);
        }

        private void SetDepartmentsInfo(StringBuilder builder, string depSymbol, List<Department> departments)
        {
            foreach (var department in departments)
            {
                var depInfo = $"{depSymbol} {department.Name} ID={department.Id}\n";
                builder.Append(depInfo);
                SetEmployeesInfo(department, builder);
                SetDepartmentsInfo(builder, depSymbol + "=", department.Children);
            }
        }

        private static void SetEmployeesInfo(Department department, StringBuilder builder)
        {
            foreach (var employee in department.Employees)
            {
                var empSymbol = employee.Id == department.ManagerId ? "*" : "-";
                var job = employee.JobTitle;
                var jobInfo = job == null ? "" : $"({job.Name} ID={job.Id})";
                var empInfo = $"{empSymbol} {employee.FullName} {jobInfo}\n";
                builder.Append(empInfo);
            }
        }

        private async Task UpdateParentDepartments(List<Department> departments)
        {
            var departmentNames = departments.Select(x => x.Name);
            var departs = await _departmentRepository.GetParentByName(departmentNames);
            foreach (var department in departs)
            {
                department.Parent = departments.FirstOrDefault(x => x.Name.Equals(department.TsvParentName, StringComparison.OrdinalIgnoreCase));
            }
            _departmentRepository.UpdateRange(departs);
        }

        private async Task UpdateEmployeeDepartments(List<Department> departments)
        {
            var departmentNames = departments.Select(x => x.Name);
            var employees = await _employeeRepository.GetByDepartments(departmentNames);
            foreach (var employee in employees)
            {
                employee.Department = departments.FirstOrDefault(x => x.Name.Equals(employee.TsvDepartmentName, StringComparison.OrdinalIgnoreCase));
            }
            _employeeRepository.UpdateRange(employees);
        }

        private static Department SetDepartment(List<string> fields)
        {
            return new Department
            {
                Name = fields[0],
                TsvParentName = fields[1],
                TsvManagerName = fields[2],
                Phone = fields[3]
            };
        }
    }
}
