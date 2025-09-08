using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class reservation_icalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_reservation_room_id",
                table: "reservation",
                newName: "ix_reservation_room_id");

            migrationBuilder.RenameIndex(
                name: "IX_reservation_account_id",
                table: "reservation",
                newName: "ix_reservation_account_id");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "reservation",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

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
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "reservation",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "reservation_exception",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reservation_id = table.Column<int>(type: "integer", nullable: false),
                    exception_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation_exception", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservation_exception_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_end_time",
                table: "reservation",
                column: "end_time");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_time_range",
                table: "reservation",
                columns: new[] { "room_id", "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_start_time",
                table: "reservation",
                column: "start_time");

            migrationBuilder.AddCheckConstraint(
                name: "ck_reservation_end_after_start",
                table: "reservation",
                sql: "end_time > start_time");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_exception_created_at",
                table: "reservation_exception",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_exception_is_active",
                table: "reservation_exception",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_exception_reservation_id",
                table: "reservation_exception",
                column: "reservation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservation_exception");

            migrationBuilder.DropIndex(
                name: "ix_reservation_end_time",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_room_time_range",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_start_time",
                table: "reservation");

            migrationBuilder.DropCheckConstraint(
                name: "ck_reservation_end_after_start",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "description",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "is_recurring",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "recurrence_rule",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "recurrent_until",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "status",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "title",
                table: "reservation");

            migrationBuilder.RenameIndex(
                name: "ix_reservation_room_id",
                table: "reservation",
                newName: "IX_reservation_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_reservation_account_id",
                table: "reservation",
                newName: "IX_reservation_account_id");
        }
    }
}
