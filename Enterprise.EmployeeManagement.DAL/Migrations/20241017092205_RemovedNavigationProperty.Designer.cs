﻿// <auto-generated />
using System;
using Enterprise.EmployeeManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enterprise.EmployeeManagement.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241017092205_RemovedNavigationProperty")]
    partial class RemovedNavigationProperty
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Enterprise.EmployeeManagement.DAL.Models.RoleModel", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("RoleName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Enterprise.EmployeeManagement.DAL.Models.SiteUserModel", b =>
                {
                    b.Property<int>("SiteUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("LogInIpAddress")
                        .HasColumnType("varchar(45) CHARACTER SET utf8mb4")
                        .HasMaxLength(45);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("SiteUserId");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.ToTable("SiteUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
