using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    public class Employee : BaseEntity
    {
        public long? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long? JobTitleId { get; set; }
        [ForeignKey(nameof(JobTitleId))]
        public JobTitle? JobTitle { get; set; }
        public string? TsvDepartmentName { get; set; }
        public string? TsvJobTitleName { get; set; }
    }
}
