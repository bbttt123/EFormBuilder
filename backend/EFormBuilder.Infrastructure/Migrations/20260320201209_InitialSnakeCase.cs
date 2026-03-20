using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFormBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "forms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forms", x => x.id);
                    table.ForeignKey(
                        name: "fk_forms_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fields",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    field_type = table.Column<string>(type: "text", nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    form_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fields", x => x.id);
                    table.ForeignKey(
                        name: "fk_fields_forms_form_id",
                        column: x => x.form_id,
                        principalTable: "forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "responses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    responder_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    form_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_responses", x => x.id);
                    table.ForeignKey(
                        name: "fk_responses_forms_form_id",
                        column: x => x.form_id,
                        principalTable: "forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "field_options",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_field_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_field_options_fields_field_id",
                        column: x => x.field_id,
                        principalTable: "fields",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    answer_text = table.Column<string>(type: "text", nullable: true),
                    response_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_answers_fields_field_id",
                        column: x => x.field_id,
                        principalTable: "fields",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_answers_responses_response_id",
                        column: x => x.response_id,
                        principalTable: "responses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_answers_field_id",
                table: "answers",
                column: "field_id");

            migrationBuilder.CreateIndex(
                name: "ix_answers_response_id",
                table: "answers",
                column: "response_id");

            migrationBuilder.CreateIndex(
                name: "ix_field_options_field_id",
                table: "field_options",
                column: "field_id");

            migrationBuilder.CreateIndex(
                name: "ix_fields_form_id",
                table: "fields",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "ix_forms_user_id",
                table: "forms",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_responses_form_id",
                table: "responses",
                column: "form_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropTable(
                name: "field_options");

            migrationBuilder.DropTable(
                name: "responses");

            migrationBuilder.DropTable(
                name: "fields");

            migrationBuilder.DropTable(
                name: "forms");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
