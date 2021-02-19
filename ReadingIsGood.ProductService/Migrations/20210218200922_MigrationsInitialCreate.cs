using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReadingIsGood.ProductService.Migrations
{
    public partial class MigrationsInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "ProductService");

            migrationBuilder.CreateTable(
                "Product",
                schema: "ProductService",
                columns: table => new
                {
                    Id = table.Column<Guid>("uniqueidentifier", nullable: false),
                    Name = table.Column<string>("VARCHAR(100)", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>("decimal(18,2)", nullable: false),
                    Stock = table.Column<int>("int", nullable: false),
                    AuditInformation_CreatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    AuditInformation_CreatedDate = table.Column<DateTime>("datetime2", nullable: true),
                    AuditInformation_UpdatedBy = table.Column<Guid>("uniqueidentifier", nullable: true),
                    AuditInformation_LastModifiedDate = table.Column<DateTime>("datetime2", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Product", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Product",
                "ProductService");
        }
    }
}