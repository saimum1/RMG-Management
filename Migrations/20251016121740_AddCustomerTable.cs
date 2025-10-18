using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetWbapi.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryChallanDetailCreation");

            migrationBuilder.DropTable(
                name: "DeliveryChallans");

            migrationBuilder.DropTable(
                name: "GatepassDetails");

            migrationBuilder.DropTable(
                name: "PendingDeliveryItemCreation");

            migrationBuilder.AlterColumn<string>(
                name: "RentedAmount",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "OutTime",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "OutDate",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "HireTruckNo",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ExistingTruckNo",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "agdl",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descriptions",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "quantity",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeclarationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclarationSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclarationSettings");

            migrationBuilder.DropColumn(
                name: "agdl",
                table: "DeliveryChallanHeaderCreation");

            migrationBuilder.DropColumn(
                name: "descriptions",
                table: "DeliveryChallanHeaderCreation");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "DeliveryChallanHeaderCreation");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "DeliveryChallanHeaderCreation");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "DeliveryChallanHeaderCreation");

            migrationBuilder.AlterColumn<string>(
                name: "RentedAmount",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OutTime",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OutDate",
                table: "DeliveryChallanHeaderCreation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HireTruckNo",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExistingTruckNo",
                table: "DeliveryChallanHeaderCreation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryChallanDetailCreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryChallanHeaderCreationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CBM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyWiseCBM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RentAmount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryChallanDetailCreation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryChallanDetailCreation_DeliveryChallanHeaderCreation_DeliveryChallanHeaderCreationId",
                        column: x => x.DeliveryChallanHeaderCreationId,
                        principalTable: "DeliveryChallanHeaderCreation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChallans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HSCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryChallans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatepassDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatepassHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GateOutTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SlNo = table.Column<int>(type: "int", nullable: false),
                    VehicleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatepassDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GatepassDetails_GatepassHeaders_GatepassHeaderId",
                        column: x => x.GatepassHeaderId,
                        principalTable: "GatepassHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingDeliveryItemCreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Buyer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cbm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ctn = table.Column<int>(type: "int", nullable: false),
                    DepotName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionOfGoods = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExfactoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GmtQty = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    PoNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sl = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Style = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalCBM = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCTN = table.Column<int>(type: "int", nullable: false),
                    TotalGMTQty = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingDeliveryItemCreation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallanDetailCreation_DeliveryChallanHeaderCreationId",
                table: "DeliveryChallanDetailCreation",
                column: "DeliveryChallanHeaderCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_GatepassDetails_GatepassHeaderId",
                table: "GatepassDetails",
                column: "GatepassHeaderId");
        }
    }
}
