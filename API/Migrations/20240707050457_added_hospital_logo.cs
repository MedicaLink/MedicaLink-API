using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class added_hospital_logo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredDate",
                table: "Patients",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 7, 10, 34, 55, 322, DateTimeKind.Local).AddTicks(269),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2024, 6, 30, 21, 45, 53, 819, DateTimeKind.Local).AddTicks(3948));

            migrationBuilder.AddColumn<string>(
                name: "LogoImage",
                table: "Hospitals",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoImage",
                table: "Hospitals");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredDate",
                table: "Patients",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 30, 21, 45, 53, 819, DateTimeKind.Local).AddTicks(3948),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2024, 7, 7, 10, 34, 55, 322, DateTimeKind.Local).AddTicks(269));
        }
    }
}
