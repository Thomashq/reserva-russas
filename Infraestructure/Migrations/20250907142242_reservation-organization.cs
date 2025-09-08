using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class reservationorganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reservation_exception_reservation_id",
                table: "reservation_exception");

            migrationBuilder.AlterColumn<DateTime>(
                name: "exception_date",
                table: "reservation_exception",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "action",
                table: "reservation_exception",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "new_end_time",
                table: "reservation_exception",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "new_room_id",
                table: "reservation_exception",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "new_start_time",
                table: "reservation_exception",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "original_date",
                table: "reservation_exception",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "series_id",
                table: "reservation_exception",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "reservation",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "recurrent_until",
                table: "reservation",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "reservation",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "moved_from_reservation_id",
                table: "reservation",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "origin",
                table: "reservation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "series_id",
                table: "reservation",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "reservation_series",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    default_room_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    window_start = table.Column<DateTime>(type: "date", nullable: false),
                    window_end = table.Column<DateTime>(type: "date", nullable: false),
                    recurrence_rule = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    days_of_week = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    time_start = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    time_end = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    series_status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation_series", x => x.id);
                    table.CheckConstraint("ck_reservation_series_window_end_after_start", "window_end >= window_start");
                    table.ForeignKey(
                        name: "fk_reservation_series_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reservation_series_default_room_id",
                        column: x => x.default_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_exception_new_room_id",
                table: "reservation_exception",
                column: "new_room_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_exception_reservation_date",
                table: "reservation_exception",
                columns: new[] { "reservation_id", "exception_date" });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_exception_series_id",
                table: "reservation_exception",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_moved_from_reservation_id",
                table: "reservation",
                column: "moved_from_reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_start",
                table: "reservation",
                columns: new[] { "series_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ux_reservation_series_start_room",
                table: "reservation",
                columns: new[] { "series_id", "start_time", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_account_id",
                table: "reservation_series",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_created_at",
                table: "reservation_series",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_default_room_id",
                table: "reservation_series",
                column: "default_room_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_is_active",
                table: "reservation_series",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_window_start",
                table: "reservation_series",
                column: "window_start");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_moved_from_id",
                table: "reservation",
                column: "moved_from_reservation_id",
                principalTable: "reservation",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_series_id",
                table: "reservation",
                column: "series_id",
                principalTable: "reservation_series",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_exception_reservation_series_series_id",
                table: "reservation_exception",
                column: "series_id",
                principalTable: "reservation_series",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservation_exception_rooms_new_room_id",
                table: "reservation_exception",
                column: "new_room_id",
                principalTable: "rooms",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_moved_from_id",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_series_id",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_reservation_exception_reservation_series_series_id",
                table: "reservation_exception");

            migrationBuilder.DropForeignKey(
                name: "FK_reservation_exception_rooms_new_room_id",
                table: "reservation_exception");

            migrationBuilder.DropTable(
                name: "reservation_series");

            migrationBuilder.DropIndex(
                name: "IX_reservation_exception_new_room_id",
                table: "reservation_exception");

            migrationBuilder.DropIndex(
                name: "ix_reservation_exception_reservation_date",
                table: "reservation_exception");

            migrationBuilder.DropIndex(
                name: "IX_reservation_exception_series_id",
                table: "reservation_exception");

            migrationBuilder.DropIndex(
                name: "IX_reservation_moved_from_reservation_id",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_series_start",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ux_reservation_series_start_room",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "action",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "new_end_time",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "new_room_id",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "new_start_time",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "original_date",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "reservation_exception");

            migrationBuilder.DropColumn(
                name: "moved_from_reservation_id",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "origin",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "reservation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "exception_date",
                table: "reservation_exception",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "reservation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "recurrent_until",
                table: "reservation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "reservation",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_exception_reservation_id",
                table: "reservation_exception",
                column: "reservation_id");
        }
    }
}
