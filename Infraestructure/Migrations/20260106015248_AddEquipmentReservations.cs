using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_equipment_room_details_room_details_id",
                table: "equipment");

            migrationBuilder.DropIndex(
                name: "IX_equipment_room_details_id",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "room_equipments");

            migrationBuilder.DropColumn(
                name: "room_details_id",
                table: "equipment");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "room_equipments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "room_equipments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "room_equipments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.CreateTable(
                name: "equipment_reservations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    equipment_id = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    room_reservation_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "0=Criado, 1=Aprovado, 2=Cancelado"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_reservations", x => x.id);
                    table.ForeignKey(
                        name: "FK_equipment_reservations_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_equipment_reservations_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_equipment_reservations_reservation_room_reservation_id",
                        column: x => x.room_reservation_id,
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_room_equipments_created_at",
                table: "room_equipments",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_room_equipments_is_active",
                table: "room_equipments",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_RoomDetailsId",
                table: "room_equipments",
                column: "room_details_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_reservations_created_at",
                table: "equipment_reservations",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_reservations_is_active",
                table: "equipment_reservations",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_AccountId",
                table: "equipment_reservations",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_Availability",
                table: "equipment_reservations",
                columns: new[] { "equipment_id", "start_time", "end_time", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_EquipmentId",
                table: "equipment_reservations",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_Period",
                table: "equipment_reservations",
                columns: new[] { "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_RoomReservationId",
                table: "equipment_reservations",
                column: "room_reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReservations_Status",
                table: "equipment_reservations",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipment_reservations");

            migrationBuilder.DropIndex(
                name: "ix_room_equipments_created_at",
                table: "room_equipments");

            migrationBuilder.DropIndex(
                name: "ix_room_equipments_is_active",
                table: "room_equipments");

            migrationBuilder.DropIndex(
                name: "IX_RoomEquipments_RoomDetailsId",
                table: "room_equipments");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "room_equipments");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "room_equipments");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "room_equipments");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "room_equipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "room_details_id",
                table: "equipment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_room_details_id",
                table: "equipment",
                column: "room_details_id");

            migrationBuilder.AddForeignKey(
                name: "FK_equipment_room_details_room_details_id",
                table: "equipment",
                column: "room_details_id",
                principalTable: "room_details",
                principalColumn: "id");
        }
    }
}
