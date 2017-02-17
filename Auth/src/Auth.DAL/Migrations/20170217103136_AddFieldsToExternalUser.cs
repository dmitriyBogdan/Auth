using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auth.DAL.Migrations
{
    public partial class AddFieldsToExternalUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ExternalUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ExternalUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ExternalUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameIdentifier",
                table: "ExternalUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "ExternalUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "ExternalUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ExternalUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ExternalUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ExternalUsers");

            migrationBuilder.DropColumn(
                name: "NameIdentifier",
                table: "ExternalUsers");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "ExternalUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "ExternalUsers");
        }
    }
}
