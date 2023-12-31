﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestApp.Data;

#nullable disable

namespace TestApp.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231201082201_RefactorAndRelation")]
    partial class RefactorAndRelation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TestApp.Models.Department", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("ManagerId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TsvManagerName")
                        .HasColumnType("text");

                    b.Property<string>("TsvParentName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.HasIndex("ParentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("TestApp.Models.Employee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("DepartmentId")
                        .HasColumnType("bigint");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("JobTitleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TsvDepartmentName")
                        .HasColumnType("text");

                    b.Property<string>("TsvJobTitleName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("TestApp.Models.JobTitle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("JobTitles");
                });

            modelBuilder.Entity("TestApp.Models.Department", b =>
                {
                    b.HasOne("TestApp.Models.Employee", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerId");

                    b.HasOne("TestApp.Models.Department", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Manager");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("TestApp.Models.Employee", b =>
                {
                    b.HasOne("TestApp.Models.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("TestApp.Models.JobTitle", "JobTitle")
                        .WithMany()
                        .HasForeignKey("JobTitleId");

                    b.Navigation("Department");

                    b.Navigation("JobTitle");
                });

            modelBuilder.Entity("TestApp.Models.Department", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
