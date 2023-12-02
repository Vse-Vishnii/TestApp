using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAndRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobTitleName",
                table: "Employees",
                newName: "TsvJobTitleName");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "Employees",
                newName: "TsvDepartmentName");

            migrationBuilder.RenameColumn(
                name: "ParentName",
                table: "Departments",
                newName: "TsvParentName");

            migrationBuilder.RenameColumn(
                name: "ManagerName",
                table: "Departments",
                newName: "TsvManagerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TsvJobTitleName",
                table: "Employees",
                newName: "JobTitleName");

            migrationBuilder.RenameColumn(
                name: "TsvDepartmentName",
                table: "Employees",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "TsvParentName",
                table: "Departments",
                newName: "ParentName");

            migrationBuilder.RenameColumn(
                name: "TsvManagerName",
                table: "Departments",
                newName: "ManagerName");
        }
    }
}
