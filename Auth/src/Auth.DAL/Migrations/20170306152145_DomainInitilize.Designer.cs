using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Auth.DAL.ProxyContext;

namespace Auth.DAL.Migrations.Proxy
{
    [DbContext(typeof(ProxyContext.ProxyContext))]
    [Migration("20170306152145_DomainInitilize")]
    partial class DomainInitilize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Auth.DAL.ProxyContext.Enteities.ProxyDomain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Domain");

                    b.Property<string>("Sid");

                    b.HasKey("Id");

                    b.ToTable("Domains");
                });
        }
    }
}
