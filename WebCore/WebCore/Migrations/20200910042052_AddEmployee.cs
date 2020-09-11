using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebCore.Migrations
{
    public partial class AddEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Divisionya_tbl_Department_DepartmentId",
                table: "tbl_Divisionya");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "tbl_Divisionya",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_Employee",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false),
                    isDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Employee_tbl_m_user_Id",
                        column: x => x.Id,
                        principalTable: "tbl_m_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Divisionya_tbl_Department_DepartmentId",
                table: "tbl_Divisionya",
                column: "DepartmentId",
                principalTable: "tbl_Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Divisionya_tbl_Department_DepartmentId",
                table: "tbl_Divisionya");

            migrationBuilder.DropTable(
                name: "tbl_Employee");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "tbl_Divisionya",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Divisionya_tbl_Department_DepartmentId",
                table: "tbl_Divisionya",
                column: "DepartmentId",
                principalTable: "tbl_Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
