using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFormBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_token_users_user_id",
                table: "refresh_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_token",
                table: "refresh_token");

            migrationBuilder.RenameTable(
                name: "refresh_token",
                newName: "refresh_tokens");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_token_user_id",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_token_token",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_token");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "forms",
                type: "text",
                nullable: false,
                defaultValue: "Draft",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "forms",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_forms_slug",
                table: "forms",
                column: "slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "ix_forms_slug",
                table: "forms");

            migrationBuilder.DropPrimaryKey(
                name: "pk_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "forms");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "refresh_token");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_token",
                newName: "ix_refresh_token_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_token",
                newName: "ix_refresh_token_token");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "forms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Draft");

            migrationBuilder.AddPrimaryKey(
                name: "pk_refresh_token",
                table: "refresh_token",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_token_users_user_id",
                table: "refresh_token",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
