using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CountCallories.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class WithDoublePrimaryKeyCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                columns: new[] { "UserId", "DateWhenAdd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "DateWhenAdd");
        }
    }
}
