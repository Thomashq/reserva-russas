using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class _20250828RoomDetailsEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    is_reserveable = table.Column<bool>(type: "boolean", nullable: false),
                    room_type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_details_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    room_details_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.id);
                    table.ForeignKey(
                        name: "FK_equipment_room_details_room_details_id",
                        column: x => x.room_details_id,
                        principalTable: "room_details",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "room_equipments",
                columns: table => new
                {
                    room_details_id = table.Column<int>(type: "integer", nullable: false),
                    equipment_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_equipments", x => new { x.room_details_id, x.equipment_id });
                    table.ForeignKey(
                        name: "FK_room_equipments_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_room_equipments_room_details_room_details_id",
                        column: x => x.room_details_id,
                        principalTable: "room_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_created_at",
                table: "equipment",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_is_active",
                table: "equipment",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Name",
                table: "equipment",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_room_details_id",
                table: "equipment",
                column: "room_details_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_details_created_at",
                table: "room_details",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_room_details_is_active",
                table: "room_details",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_RoomDetails_RoomId",
                table: "room_details",
                column: "room_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_EquipmentId",
                table: "room_equipments",
                column: "equipment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_equipments");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "room_details");
        }
    }
}
