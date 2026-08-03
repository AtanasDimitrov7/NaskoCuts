using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class AssignAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
                SELECT u.[Id], r.[Id]
                FROM [AspNetUsers] u
                CROSS JOIN [AspNetRoles] r
                WHERE u.[Email] = 'admin@naskocuts.bg'
                  AND r.[Name] = 'Admin'
                  AND NOT EXISTS (
                      SELECT 1 FROM [AspNetUserRoles] ur
                      WHERE ur.[UserId] = u.[Id] AND ur.[RoleId] = r.[Id]
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE ur
                FROM [AspNetUserRoles] ur
                JOIN [AspNetUsers] u ON u.[Id] = ur.[UserId]
                JOIN [AspNetRoles] r ON r.[Id] = ur.[RoleId]
                WHERE u.[Email] = 'admin@naskocuts.bg' AND r.[Name] = 'Admin';
            ");
        }
    }
}