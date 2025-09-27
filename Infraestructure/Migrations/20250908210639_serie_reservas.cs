using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class serie_reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_recurring",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "recurrence_rule",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "recurrent_until",
                table: "reservation");

            migrationBuilder.AlterColumn<int>(
                name: "default_room_id",
                table: "reservation_series",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "default_room_id",
                table: "reservation_series",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "is_recurring",
                table: "reservation",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "recurrence_rule",
                table: "reservation",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "recurrent_until",
                table: "reservation",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
