using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Organization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Advisee",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Advisor",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ManagedRooms",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Reservation",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Reservations",
                table: "Account");

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagedRooms = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Advisee = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    Reservation = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reservations = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    Advisor = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    Permissions = table.Column<List<Guid>>(type: "uuid[]", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "Servant");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Advisee",
                table: "Account",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Advisor",
                table: "Account",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Account",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "ManagedRooms",
                table: "Account",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Permissions",
                table: "Account",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Reservation",
                table: "Account",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Reservations",
                table: "Account",
                type: "uuid[]",
                nullable: true);
        }
    }
}
