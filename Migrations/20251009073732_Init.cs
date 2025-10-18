using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetWbapi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryChallanHeaderCreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallanNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExistingTruckNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HireTruckNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    To = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutTime = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TruckCBM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepotName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransportCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LockNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RentedAmount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryChallanHeaderCreation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChallans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HSCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryChallans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GatepassHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatePassNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TruckNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransportCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverLicense = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GatepassHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingDeliveryItemCreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sl = table.Column<int>(type: "int", nullable: false),
                    DescriptionOfGoods = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Style = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PoNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Buyer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ctn = table.Column<int>(type: "int", nullable: false),
                    Cbm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GmtQty = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExfactoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepotName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalCTN = table.Column<int>(type: "int", nullable: false),
                    TotalCBM = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalGMTQty = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingDeliveryItemCreation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TruckNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TruckLicense = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransportCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriverLicense = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DriverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChallanDetailCreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryChallanHeaderCreationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyWiseCBM = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CBM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                name: "GatepassDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatepassHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlNo = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GateOutTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorizedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallanDetailCreation_DeliveryChallanHeaderCreationId",
                table: "DeliveryChallanDetailCreation",
                column: "DeliveryChallanHeaderCreationId");

            migrationBuilder.CreateIndex(
                name: "IX_GatepassDetails_GatepassHeaderId",
                table: "GatepassDetails",
                column: "GatepassHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryChallanDetailCreation");

            migrationBuilder.DropTable(
                name: "DeliveryChallans");

            migrationBuilder.DropTable(
                name: "GatepassDetails");

            migrationBuilder.DropTable(
                name: "PendingDeliveryItemCreation");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "DeliveryChallanHeaderCreation");

            migrationBuilder.DropTable(
                name: "GatepassHeaders");
        }
    }
}
