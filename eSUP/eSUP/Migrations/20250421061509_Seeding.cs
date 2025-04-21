using Microsoft.EntityFrameworkCore.Migrations;
using static eSUP.Classes.Enumerations;

#nullable disable

namespace eSUP.Migrations
{
    /// <inheritdoc />
    public partial class Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var role in Enum.GetNames<UserRoles>())
            {
                var roleId = Guid.NewGuid().ToString();
                var concurrencyStamp = Guid.NewGuid().ToString();
                migrationBuilder.InsertData("AspNetRoles", ["Id", "Name", "NormalizedName", "ConcurrencyStamp"], [roleId, role, role.ToUpper(), concurrencyStamp]);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
