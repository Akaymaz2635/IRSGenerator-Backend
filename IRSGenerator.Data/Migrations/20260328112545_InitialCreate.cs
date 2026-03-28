using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    WindowsAccount = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefectTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DefectTypes_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DispositionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    CssClass = table.Column<string>(type: "text", nullable: false),
                    IsNeutralizing = table.Column<bool>(type: "boolean", nullable: false),
                    IsInitial = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispositionTypes", x => x.Id);
                    table.UniqueConstraint("AK_DispositionTypes_Code", x => x.Code);
                    table.ForeignKey(
                        name: "FK_DispositionTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispositionTypes_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IRSProjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectType = table.Column<string>(type: "text", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    SerialNumber = table.Column<string>(type: "text", nullable: false),
                    OpSheetPath = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRSProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRSProjects_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRSProjects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRSProjects_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roles_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisualProjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualProjects_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisualProjects_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VisualSystemConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualSystemConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisualSystemConfigs_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisualSystemConfigs_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DefectFields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefectTypeId = table.Column<long>(type: "bigint", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    FieldType = table.Column<string>(type: "text", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    MinValue = table.Column<double>(type: "double precision", nullable: true),
                    MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectFields_DefectTypes_DefectTypeId",
                        column: x => x.DefectTypeId,
                        principalTable: "DefectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefectFields_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DefectFields_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DispositionTransitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromCode = table.Column<string>(type: "text", nullable: true),
                    ToCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispositionTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispositionTransitions_DispositionTypes_FromCode",
                        column: x => x.FromCode,
                        principalTable: "DispositionTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispositionTransitions_DispositionTypes_ToCode",
                        column: x => x.ToCode,
                        principalTable: "DispositionTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispositionTransitions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispositionTransitions_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemNo = table.Column<string>(type: "text", nullable: false),
                    Dimension = table.Column<string>(type: "text", nullable: false),
                    Badge = table.Column<string>(type: "text", nullable: true),
                    Tooling = table.Column<string>(type: "text", nullable: true),
                    BPZone = table.Column<string>(type: "text", nullable: true),
                    InspectionLevel = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    LowerLimit = table.Column<double>(type: "double precision", nullable: false),
                    UpperLimit = table.Column<double>(type: "double precision", nullable: false),
                    InspectionResult = table.Column<string>(type: "text", nullable: false, defaultValue: "Unidentified"),
                    IRSProjectId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_IRSProjects_IRSProjectId",
                        column: x => x.IRSProjectId,
                        principalTable: "IRSProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IrsProjectId = table.Column<long>(type: "bigint", nullable: true),
                    VisualProjectId = table.Column<long>(type: "bigint", nullable: true),
                    PartNumber = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    OperationNumber = table.Column<string>(type: "text", nullable: true),
                    Inspector = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    InspectorId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_IRSProjects_IrsProjectId",
                        column: x => x.IrsProjectId,
                        principalTable: "IRSProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspections_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inspections_Users_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Inspections_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inspections_VisualProjects_VisualProjectId",
                        column: x => x.VisualProjectId,
                        principalTable: "VisualProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CategoricalPartResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<string>(type: "text", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoricalPartResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoricalPartResults_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoricalPartResults_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoricalPartResults_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoricalZoneResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZoneName = table.Column<string>(type: "text", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoricalZoneResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoricalZoneResults_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoricalZoneResults_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoricalZoneResults_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumericPartResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Actual = table.Column<double>(type: "double precision", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericPartResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumericPartResults_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumericPartResults_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumericPartResults_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InspectionId = table.Column<long>(type: "bigint", nullable: false),
                    DefectTypeId = table.Column<long>(type: "bigint", nullable: false),
                    OriginDefectId = table.Column<long>(type: "bigint", nullable: true),
                    Depth = table.Column<double>(type: "double precision", nullable: true),
                    Width = table.Column<double>(type: "double precision", nullable: true),
                    Length = table.Column<double>(type: "double precision", nullable: true),
                    Radius = table.Column<double>(type: "double precision", nullable: true),
                    Angle = table.Column<double>(type: "double precision", nullable: true),
                    Height = table.Column<double>(type: "double precision", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    HighMetal = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defects_DefectTypes_DefectTypeId",
                        column: x => x.DefectTypeId,
                        principalTable: "DefectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Defects_Defects_OriginDefectId",
                        column: x => x.OriginDefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Defects_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Defects_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Defects_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InspectionId = table.Column<long>(type: "bigint", nullable: false),
                    Filename = table.Column<string>(type: "text", nullable: false),
                    Filepath = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Photos_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Photos_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Dispositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefectId = table.Column<long>(type: "bigint", nullable: false),
                    Decision = table.Column<string>(type: "text", nullable: false),
                    EnteredBy = table.Column<string>(type: "text", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: false),
                    SpecRef = table.Column<string>(type: "text", nullable: true),
                    Engineer = table.Column<string>(type: "text", nullable: true),
                    Reinspector = table.Column<string>(type: "text", nullable: true),
                    ConcessionNo = table.Column<string>(type: "text", nullable: true),
                    VoidReason = table.Column<string>(type: "text", nullable: true),
                    RepairRef = table.Column<string>(type: "text", nullable: true),
                    ScrapReason = table.Column<string>(type: "text", nullable: true),
                    MeasurementsSnapshot = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositions_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispositions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dispositions_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PhotoDefects",
                columns: table => new
                {
                    PhotoId = table.Column<long>(type: "bigint", nullable: false),
                    DefectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoDefects", x => new { x.PhotoId, x.DefectId });
                    table.ForeignKey(
                        name: "FK_PhotoDefects_Defects_DefectId",
                        column: x => x.DefectId,
                        principalTable: "Defects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoDefects_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DispositionTypes",
                columns: new[] { "Id", "Active", "Code", "CreatedAt", "CreatedById", "CreatedByUserId", "CssClass", "IsInitial", "IsNeutralizing", "Label", "SortOrder", "UpdatedAt", "UpdatedById", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1L, true, "USE_AS_IS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-accepted", true, true, "Kabul (Spec)", 1, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 2L, true, "KABUL_RESIM", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-accepted", true, true, "Kabul (Resim)", 2, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 3L, true, "CONFORMS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-conforms", false, true, "Uygun (Inspector)", 3, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 4L, true, "REWORK", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-rework", true, false, "Rework", 4, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 5L, true, "RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-re-inspect", true, false, "Yeniden İnceleme", 5, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 6L, true, "CTP_RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-mrb-ctp", true, false, "CTP — Sonraki Op. Yeniden İnceleme", 6, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 7L, true, "MRB_SUBMITTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-mrb-submitted", true, false, "MRB Gönderildi", 7, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 8L, true, "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-mrb-ctp", true, false, "CTP — MRB (Devam)", 8, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 9L, true, "MRB_ACCEPTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-mrb-accepted", false, true, "MRB Kabul", 9, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 10L, true, "MRB_REJECTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-mrb-rejected", false, true, "MRB Ret", 10, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 11L, true, "VOID", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-void", true, true, "Void", 11, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 12L, true, "REPAIR", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-repair", true, true, "Repair", 12, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 13L, true, "SCRAP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "disp-scrap", true, true, "Scrap", 13, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedById", "DisplayName", "EmployeeId", "FirstName", "LastName", "PasswordHash", "Role", "UpdatedAt", "UpdatedById", "WindowsAccount" },
                values: new object[] { 1L, true, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "Erdem.Demirtaş", "6518", "Erdem", "Demirtaş", null, "inspector", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "TEIDOM\\k6518" });

            migrationBuilder.InsertData(
                table: "DispositionTransitions",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "CreatedByUserId", "FromCode", "ToCode", "UpdatedAt", "UpdatedById", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "USE_AS_IS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "KABUL_RESIM", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "REWORK", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 4L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 5L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "CTP_RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 6L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "MRB_SUBMITTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 7L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 8L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "VOID", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 9L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "REPAIR", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 10L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, null, "SCRAP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 11L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "CONFORMS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 12L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "USE_AS_IS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 13L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "KABUL_RESIM", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 14L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "REWORK", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 15L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 16L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "CTP_RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 17L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "MRB_SUBMITTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 18L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 19L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "VOID", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 20L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "REPAIR", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 21L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "REWORK", "SCRAP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 22L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "CONFORMS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 23L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "USE_AS_IS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 24L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "KABUL_RESIM", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 25L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "REWORK", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 26L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 27L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "CTP_RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 28L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "MRB_SUBMITTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 29L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 30L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "VOID", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 31L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "REPAIR", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 32L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "RE_INSPECT", "SCRAP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 33L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "CONFORMS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 34L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "USE_AS_IS", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 35L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "KABUL_RESIM", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 36L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "REWORK", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 37L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 38L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "CTP_RE_INSPECT", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 39L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "MRB_SUBMITTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 40L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 41L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "VOID", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 42L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "REPAIR", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 43L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "CTP_RE_INSPECT", "SCRAP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 44L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "MRB_SUBMITTED", "MRB_CTP", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 45L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "MRB_SUBMITTED", "MRB_ACCEPTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 46L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "MRB_SUBMITTED", "MRB_REJECTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 47L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "MRB_CTP", "MRB_ACCEPTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null },
                    { 48L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null, "MRB_CTP", "MRB_REJECTED", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedById", "Description", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1L, "irsproject.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[IRSProject] write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 2L, "character.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[Character] write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 3L, "categoricalpartresult.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[CategoricalPartResult] write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 4L, "categoricalzoneresult.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[CategoricalZoneResult] write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 5L, "numericalpartresult.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[NumericalPartResult] write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 6L, "irsproject.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[IRSProject] read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 7L, "character.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[Character] read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 8L, "categoricalpartresult.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[CategoricalPartResult] read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 9L, "categoricalzoneresult.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[CategoricalZoneResult] read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 10L, "numericalpartresult.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "[NumericalPartResult] read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 11L, "authorization.write", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "Authorization write permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 12L, "authorization.read", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "Authorization read permission.", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "Name", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "Admin", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "UserWriter", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "UserReader", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedById", "DisplayName", "EmployeeId", "FirstName", "LastName", "PasswordHash", "Role", "UpdatedAt", "UpdatedById", "WindowsAccount" },
                values: new object[] { 2L, true, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "Uras.Erken", "5956", "Uras", "Erken", null, "inspector", new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, "TEIDOM\\k5956" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "PermissionId", "RoleId", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 11L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 12L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 4L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 5L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 3L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 6L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 4L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 7L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 5L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 8L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 9L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 10L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 3L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 11L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 4L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 12L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 5L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 13L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 6L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 14L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 7L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 15L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 8L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 16L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 9L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L },
                    { 17L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 10L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "RoleId", "UpdatedAt", "UpdatedById", "UserId" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L },
                    { 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L },
                    { 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L },
                    { 4L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L },
                    { 5L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 1L },
                    { 6L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 3L, new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), 1L, 2L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalPartResults_CharacterId",
                table: "CategoricalPartResults",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalPartResults_CreatedById",
                table: "CategoricalPartResults",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalPartResults_UpdatedById",
                table: "CategoricalPartResults",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalZoneResults_CharacterId",
                table: "CategoricalZoneResults",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalZoneResults_CreatedById",
                table: "CategoricalZoneResults",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CategoricalZoneResults_UpdatedById",
                table: "CategoricalZoneResults",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedById",
                table: "Characters",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_IRSProjectId",
                table: "Characters",
                column: "IRSProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UpdatedById",
                table: "Characters",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DefectFields_CreatedByUserId",
                table: "DefectFields",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectFields_DefectTypeId",
                table: "DefectFields",
                column: "DefectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectFields_UpdatedByUserId",
                table: "DefectFields",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_CreatedByUserId",
                table: "Defects",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_DefectTypeId",
                table: "Defects",
                column: "DefectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_InspectionId",
                table: "Defects",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_OriginDefectId",
                table: "Defects",
                column: "OriginDefectId");

            migrationBuilder.CreateIndex(
                name: "IX_Defects_UpdatedByUserId",
                table: "Defects",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectTypes_CreatedByUserId",
                table: "DefectTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefectTypes_UpdatedByUserId",
                table: "DefectTypes",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_CreatedByUserId",
                table: "Dispositions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_DefectId",
                table: "Dispositions",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_UpdatedByUserId",
                table: "Dispositions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTransitions_CreatedByUserId",
                table: "DispositionTransitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTransitions_FromCode",
                table: "DispositionTransitions",
                column: "FromCode");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTransitions_ToCode",
                table: "DispositionTransitions",
                column: "ToCode");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTransitions_UpdatedByUserId",
                table: "DispositionTransitions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTypes_Code",
                table: "DispositionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTypes_CreatedByUserId",
                table: "DispositionTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DispositionTypes_UpdatedByUserId",
                table: "DispositionTypes",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_CreatedByUserId",
                table: "Inspections",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectorId",
                table: "Inspections",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_IrsProjectId",
                table: "Inspections",
                column: "IrsProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_UpdatedByUserId",
                table: "Inspections",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VisualProjectId",
                table: "Inspections",
                column: "VisualProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IRSProjects_CreatedById",
                table: "IRSProjects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRSProjects_OwnerId",
                table: "IRSProjects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_IRSProjects_UpdatedById",
                table: "IRSProjects",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NumericPartResults_CharacterId",
                table: "NumericPartResults",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_NumericPartResults_CreatedById",
                table: "NumericPartResults",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NumericPartResults_UpdatedById",
                table: "NumericPartResults",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedById",
                table: "Permissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdatedById",
                table: "Permissions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDefects_DefectId",
                table: "PhotoDefects",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CreatedByUserId",
                table: "Photos",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_InspectionId",
                table: "Photos",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UpdatedByUserId",
                table: "Photos",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_CreatedById",
                table: "RolePermissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_UpdatedById",
                table: "RolePermissions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedById",
                table: "Roles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedById",
                table: "Roles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_CreatedById",
                table: "UserRoles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UpdatedById",
                table: "UserRoles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedById",
                table: "Users",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisualProjects_CreatedByUserId",
                table: "VisualProjects",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualProjects_UpdatedByUserId",
                table: "VisualProjects",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualSystemConfigs_CreatedByUserId",
                table: "VisualSystemConfigs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisualSystemConfigs_UpdatedByUserId",
                table: "VisualSystemConfigs",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoricalPartResults");

            migrationBuilder.DropTable(
                name: "CategoricalZoneResults");

            migrationBuilder.DropTable(
                name: "DefectFields");

            migrationBuilder.DropTable(
                name: "Dispositions");

            migrationBuilder.DropTable(
                name: "DispositionTransitions");

            migrationBuilder.DropTable(
                name: "NumericPartResults");

            migrationBuilder.DropTable(
                name: "PhotoDefects");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VisualSystemConfigs");

            migrationBuilder.DropTable(
                name: "DispositionTypes");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Defects");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "DefectTypes");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "IRSProjects");

            migrationBuilder.DropTable(
                name: "VisualProjects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
