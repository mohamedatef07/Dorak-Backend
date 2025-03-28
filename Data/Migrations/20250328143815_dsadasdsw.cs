using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class dsadasdsw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderCertifications_Providers_ProviderId",
                table: "ProviderCertifications");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderCertifications_Providers_ProviderId",
                table: "ProviderCertifications",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderCertifications_Providers_ProviderId",
                table: "ProviderCertifications");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderCertifications_Providers_ProviderId",
                table: "ProviderCertifications",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
