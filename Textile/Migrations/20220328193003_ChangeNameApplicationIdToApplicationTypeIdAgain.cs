﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Textile.Migrations
{
    public partial class ChangeNameApplicationIdToApplicationTypeIdAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationTypeId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId",
                principalTable: "ApplicationType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationTypeId",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ApplicationType_ApplicationTypeId",
                table: "Product",
                column: "ApplicationTypeId",
                principalTable: "ApplicationType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
