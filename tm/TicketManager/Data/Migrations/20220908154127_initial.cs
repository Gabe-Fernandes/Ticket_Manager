using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.CreateTable(
            //        name: "AspNetRoles",
            //        columns: table => new
            //        {
            //            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetUsers",
            //        columns: table => new
            //        {
            //            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            AssignedRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            //            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Projects",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //            GitHubLink = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            ProjectCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Projects", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetRoleClaims",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
            //                column: x => x.RoleId,
            //                principalTable: "AspNetRoles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetUserClaims",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
            //                column: x => x.UserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetUserLogins",
            //        columns: table => new
            //        {
            //            LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //            ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            //            table.ForeignKey(
            //                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
            //                column: x => x.UserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetUserRoles",
            //        columns: table => new
            //        {
            //            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
            //            table.ForeignKey(
            //                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
            //                column: x => x.RoleId,
            //                principalTable: "AspNetRoles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
            //                column: x => x.UserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AspNetUserTokens",
            //        columns: table => new
            //        {
            //            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            //            table.ForeignKey(
            //                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
            //                column: x => x.UserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Messages",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            From = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            To = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Messages", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Messages_AspNetUsers_AppUserId",
            //                column: x => x.AppUserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id");
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "ProjectAppUsers",
            //        columns: table => new
            //        {
            //            ProjectId = table.Column<int>(type: "int", nullable: false),
            //            AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_ProjectAppUsers", x => new { x.ProjectId, x.AppUserId });
            //            table.ForeignKey(
            //                name: "FK_ProjectAppUsers_AspNetUsers_AppUserId",
            //                column: x => x.AppUserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_ProjectAppUsers_Projects_ProjectId",
            //                column: x => x.ProjectId,
            //                principalTable: "Projects",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Tickets",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
            //            Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            PriorityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            AssignedFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ProjectId = table.Column<int>(type: "int", nullable: false),
            //            AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Tickets", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Tickets_AspNetUsers_AppUserId",
            //                column: x => x.AppUserId,
            //                principalTable: "AspNetUsers",
            //                principalColumn: "Id");
            //            table.ForeignKey(
            //                name: "FK_Tickets_Projects_ProjectId",
            //                column: x => x.ProjectId,
            //                principalTable: "Projects",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Comments",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            TicketId = table.Column<int>(type: "int", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Comments", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Comments_Tickets_TicketId",
            //                column: x => x.TicketId,
            //                principalTable: "Tickets",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AspNetRoleClaims_RoleId",
            //        table: "AspNetRoleClaims",
            //        column: "RoleId");

            //    migrationBuilder.CreateIndex(
            //        name: "RoleNameIndex",
            //        table: "AspNetRoles",
            //        column: "NormalizedName",
            //        unique: true,
            //        filter: "[NormalizedName] IS NOT NULL");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AspNetUserClaims_UserId",
            //        table: "AspNetUserClaims",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AspNetUserLogins_UserId",
            //        table: "AspNetUserLogins",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AspNetUserRoles_RoleId",
            //        table: "AspNetUserRoles",
            //        column: "RoleId");

            //    migrationBuilder.CreateIndex(
            //        name: "EmailIndex",
            //        table: "AspNetUsers",
            //        column: "NormalizedEmail");

            //    migrationBuilder.CreateIndex(
            //        name: "UserNameIndex",
            //        table: "AspNetUsers",
            //        column: "NormalizedUserName",
            //        unique: true,
            //        filter: "[NormalizedUserName] IS NOT NULL");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Comments_TicketId",
            //        table: "Comments",
            //        column: "TicketId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Messages_AppUserId",
            //        table: "Messages",
            //        column: "AppUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_ProjectAppUsers_AppUserId",
            //        table: "ProjectAppUsers",
            //        column: "AppUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_AppUserId",
            //        table: "Tickets",
            //        column: "AppUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_ProjectId",
            //        table: "Tickets",
            //        column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ProjectAppUsers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
