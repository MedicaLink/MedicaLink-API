using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addedNewAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Patients",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredDate",
                table: "Patients",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 18, 21, 36, 9, 695, DateTimeKind.Local).AddTicks(1388));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "RegisteredDate",
                table: "Patients");
        }
    }
}
