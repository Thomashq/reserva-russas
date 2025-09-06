using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class aspidentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    mail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    account_permission = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rr_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rr_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "manager",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manager", x => x.id);
                    table.ForeignKey(
                        name: "FK_manager_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servant",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servant", x => x.id);
                    table.ForeignKey(
                        name: "FK_servant_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student", x => x.id);
                    table.ForeignKey(
                        name: "FK_student_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    manager_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_rooms_manager_manager_id",
                        column: x => x.manager_id,
                        principalTable: "manager",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_advisor",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    servant_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_advisor", x => x.id);
                    table.ForeignKey(
                        name: "FK_student_advisor_servant_servant_id",
                        column: x => x.servant_id,
                        principalTable: "servant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_student_advisor_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_student_permission_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    servant_id = table.Column<int>(type: "integer", nullable: true),
                    student_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.id);
                    table.ForeignKey(
                        name: "FK_reservation_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reservation_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservation_servant_servant_id",
                        column: x => x.servant_id,
                        principalTable: "servant",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_reservation_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_created_at",
                table: "account",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_account_is_active",
                table: "account",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_account_mail",
                table: "account",
                column: "mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_account_phone",
                table: "account",
                column: "phone",
                filter: "phone IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_account_user_name",
                table: "account",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_manager_account_id",
                table: "manager",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_manager_created_at",
                table: "manager",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_manager_is_active",
                table: "manager",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_account_id",
                table: "reservation",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_created_at",
                table: "reservation",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_is_active",
                table: "reservation",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_room_id",
                table: "reservation",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_servant_id",
                table: "reservation",
                column: "servant_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_student_id",
                table: "reservation",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_created_at",
                table: "rooms",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_is_active",
                table: "rooms",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_manager_id",
                table: "rooms",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "ix_rr_users_created_at",
                table: "rr_users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_rr_users_is_active",
                table: "rr_users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_rr_users_normalized_email",
                table: "rr_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ux_rr_users_normalized_user_name",
                table: "rr_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servant_account_id",
                table: "servant",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_servant_created_at",
                table: "servant",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_servant_is_active",
                table: "servant",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_student_account_id",
                table: "student",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_student_created_at",
                table: "student",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_student_is_active",
                table: "student",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_student_advisor_created_at",
                table: "student_advisor",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_student_advisor_is_active",
                table: "student_advisor",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_student_advisor_servant_id",
                table: "student_advisor",
                column: "servant_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_advisor_student_id",
                table: "student_advisor",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_permission_created_at",
                table: "student_permission",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_student_permission_is_active",
                table: "student_permission",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_student_permission_student_id",
                table: "student_permission",
                column: "student_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "rr_users");

            migrationBuilder.DropTable(
                name: "student_advisor");

            migrationBuilder.DropTable(
                name: "student_permission");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "servant");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "manager");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
