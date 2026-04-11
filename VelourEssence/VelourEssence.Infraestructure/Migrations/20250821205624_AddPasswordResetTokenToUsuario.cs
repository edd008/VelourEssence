using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelourEssence.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokenToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiration",
                table: "Usuario",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiration",
                table: "Usuario");
        }
    }
}
