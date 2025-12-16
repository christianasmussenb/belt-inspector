using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeltInspector.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileDownloadUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "Files",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Files"
                SET "DownloadUrl" = '/api/files/' || "Id"::text || '/download'
                WHERE coalesce("DownloadUrl", '') = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "Files");
        }
    }
}
