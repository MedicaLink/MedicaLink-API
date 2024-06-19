using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class refinedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Patients_RegisteredBy",
                table: "Patients",
                column: "RegisteredBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Admins_RegisteredBy",
                table: "Patients",
                column: "RegisteredBy",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Admins_RegisteredBy",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_RegisteredBy",
                table: "Patients");
        }
    }
}
