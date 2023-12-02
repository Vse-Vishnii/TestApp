using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TestApp.AppServices.Services;
using TestAppModels.Enums;

namespace TestApp.Controllers
{
    [Area("Parser")]
    [Route("[area]")]
    public class HomeController : Controller
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly JobService _jobService;

        public HomeController(DepartmentService departmentService, EmployeeService employeeService, JobService jobService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
            _jobService = jobService;
        }

        [HttpGet("/get")]
        public async Task<string> Get(long? id = null)
        {
            var departments = id == null
                ? await _departmentService.GetDepartments()
                : await _departmentService.GetDepartmentsById(id.Value);
            return departments;
        }

        /// <summary>
        /// Импорт данных
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type">Тип таблицы:
        /// 0 - Отдел,
        /// 1 - Сотрудник,
        /// 2 - Работа</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HttpPost("/import")]
        public async Task<IActionResult> Import(IFormFile data, ImportType type)
        {
            switch (type)
            {
                case ImportType.Department:
                    await _departmentService.Import(data);
                    break;
                case ImportType.Employee:
                    await _employeeService.Import(data);
                    break;
                case ImportType.JobTitle:
                    await _jobService.Import(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            return Ok("Success");
        }
    }
}
