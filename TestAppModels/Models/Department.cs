using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    public class Department : BaseEntity
    {
        public long? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public Department? Parent { get; set; }
        public long? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public Employee? Manager { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Department> Children { get; set; }
        public string? TsvParentName { get; set; }
        public string? TsvManagerName { get; set; }
    }
}
