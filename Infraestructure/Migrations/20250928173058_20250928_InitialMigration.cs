using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RR.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class _20250928_InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "asp_net_roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.ForeignKey(
                        name: "FK_account_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_roles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_tokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "reservation_series",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    default_room_id = table.Column<int>(type: "integer", nullable: false),
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
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    series_id = table.Column<int>(type: "integer", nullable: true),
                    origin = table.Column<int>(type: "integer", nullable: false),
                    moved_from_reservation_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    servant_id = table.Column<int>(type: "integer", nullable: true),
                    student_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation", x => x.id);
                    table.CheckConstraint("ck_reservation_end_after_start", "end_time > start_time");
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
                    table.ForeignKey(
                        name: "fk_reservation_moved_from_id",
                        column: x => x.moved_from_reservation_id,
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reservation_series_id",
                        column: x => x.series_id,
                        principalTable: "reservation_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_exception",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series_id = table.Column<int>(type: "integer", nullable: false),
                    reservation_id = table.Column<int>(type: "integer", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    original_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    exception_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    new_start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    new_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    new_room_id = table.Column<int>(type: "integer", nullable: true),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation_exception", x => x.id);
                    table.ForeignKey(
                        name: "FK_reservation_exception_reservation_series_series_id",
                        column: x => x.series_id,
                        principalTable: "reservation_series",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservation_exception_rooms_new_room_id",
                        column: x => x.new_room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reservation_exception_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "ix_account_user_id_unique",
                table: "account",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_account_user_name",
                table: "account",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asp_net_role_claims_role_id",
                table: "asp_net_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "asp_net_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asp_net_user_claims_user_id",
                table: "asp_net_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_asp_net_user_logins_user_id",
                table: "asp_net_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_asp_net_user_roles_role_id",
                table: "asp_net_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "asp_net_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_created_at",
                table: "asp_net_users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_is_active",
                table: "asp_net_users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "asp_net_users",
                column: "normalized_user_name",
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
                name: "ix_reservation_account_id",
                table: "reservation",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_created_at",
                table: "reservation",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_end_time",
                table: "reservation",
                column: "end_time");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_is_active",
                table: "reservation",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_moved_from_reservation_id",
                table: "reservation",
                column: "moved_from_reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_id",
                table: "reservation",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_time_range",
                table: "reservation",
                columns: new[] { "room_id", "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_series_start",
                table: "reservation",
                columns: new[] { "series_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_reservation_servant_id",
                table: "reservation",
                column: "servant_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_start_time",
                table: "reservation",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_student_id",
                table: "reservation",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ux_reservation_series_start_room",
                table: "reservation",
                columns: new[] { "series_id", "start_time", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_exception_created_at",
                table: "reservation_exception",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_exception_is_active",
                table: "reservation_exception",
                column: "is_active");

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
                name: "asp_net_role_claims");

            migrationBuilder.DropTable(
                name: "asp_net_user_claims");

            migrationBuilder.DropTable(
                name: "asp_net_user_logins");

            migrationBuilder.DropTable(
                name: "asp_net_user_roles");

            migrationBuilder.DropTable(
                name: "asp_net_user_tokens");

            migrationBuilder.DropTable(
                name: "reservation_exception");

            migrationBuilder.DropTable(
                name: "student_advisor");

            migrationBuilder.DropTable(
                name: "student_permission");

            migrationBuilder.DropTable(
                name: "asp_net_roles");

            migrationBuilder.DropTable(
                name: "reservation");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "servant");

            migrationBuilder.DropTable(
                name: "student");

            migrationBuilder.DropTable(
                name: "reservation_series");

            migrationBuilder.DropTable(
                name: "manager");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "asp_net_users");
        }
    }
}
