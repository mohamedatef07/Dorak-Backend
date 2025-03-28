using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class dsadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminCentersManagement_AspNetUsers_AdminId",
                table: "AdminCentersManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_AdminCentersManagement_Centers_CenterId",
                table: "AdminCentersManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_LiveQueues_LiveQueueId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Operators_OperatorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Providers_ProviderId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Shifts_ShiftId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CenterServices_Centers_CenterId",
                table: "CenterServices");

            migrationBuilder.DropForeignKey(
                name: "FK_CenterServices_Services_ServiceId",
                table: "CenterServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveQueues_Operators_OperatorId",
                table: "LiveQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Payments_PaymentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderAssignments_Centers_CenterId",
                table: "ProviderAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderAssignments_Providers_ProviderId",
                table: "ProviderAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers_AspNetUsers_ProviderId",
                table: "Providers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Operators_OperatorId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminCentersManagement_AspNetUsers_AdminId",
                table: "AdminCentersManagement",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminCentersManagement_Centers_CenterId",
                table: "AdminCentersManagement",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_LiveQueues_LiveQueueId",
                table: "Appointments",
                column: "LiveQueueId",
                principalTable: "LiveQueues",
                principalColumn: "LiveQueueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Operators_OperatorId",
                table: "Appointments",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Providers_ProviderId",
                table: "Appointments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Shifts_ShiftId",
                table: "Appointments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_CenterServices_Centers_CenterId",
                table: "CenterServices",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CenterServices_Services_ServiceId",
                table: "CenterServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveQueues_Operators_OperatorId",
                table: "LiveQueues",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Payments_PaymentId",
                table: "Notifications",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderAssignments_Centers_CenterId",
                table: "ProviderAssignments",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderAssignments_Providers_ProviderId",
                table: "ProviderAssignments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_AspNetUsers_ProviderId",
                table: "Providers",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                table: "ProviderServices",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                table: "ProviderServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Operators_OperatorId",
                table: "Shifts",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminCentersManagement_AspNetUsers_AdminId",
                table: "AdminCentersManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_AdminCentersManagement_Centers_CenterId",
                table: "AdminCentersManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_LiveQueues_LiveQueueId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Operators_OperatorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Providers_ProviderId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Shifts_ShiftId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CenterServices_Centers_CenterId",
                table: "CenterServices");

            migrationBuilder.DropForeignKey(
                name: "FK_CenterServices_Services_ServiceId",
                table: "CenterServices");

            migrationBuilder.DropForeignKey(
                name: "FK_LiveQueues_Operators_OperatorId",
                table: "LiveQueues");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Payments_PaymentId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderAssignments_Centers_CenterId",
                table: "ProviderAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderAssignments_Providers_ProviderId",
                table: "ProviderAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers_AspNetUsers_ProviderId",
                table: "Providers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Operators_OperatorId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminCentersManagement_AspNetUsers_AdminId",
                table: "AdminCentersManagement",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AdminCentersManagement_Centers_CenterId",
                table: "AdminCentersManagement",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_LiveQueues_LiveQueueId",
                table: "Appointments",
                column: "LiveQueueId",
                principalTable: "LiveQueues",
                principalColumn: "LiveQueueId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Operators_OperatorId",
                table: "Appointments",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Providers_ProviderId",
                table: "Appointments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Shifts_ShiftId",
                table: "Appointments",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CenterServices_Centers_CenterId",
                table: "CenterServices",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CenterServices_Services_ServiceId",
                table: "CenterServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveQueues_Operators_OperatorId",
                table: "LiveQueues",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Appointments_AppointmentId",
                table: "Notifications",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Payments_PaymentId",
                table: "Notifications",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Appointments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "AppointmentId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_AspNetUsers_ClientId",
                table: "Payments",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderAssignments_Centers_CenterId",
                table: "ProviderAssignments",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "CenterId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderAssignments_Providers_ProviderId",
                table: "ProviderAssignments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_AspNetUsers_ProviderId",
                table: "Providers",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                table: "ProviderServices",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "ProviderId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                table: "ProviderServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Operators_OperatorId",
                table: "Shifts",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AspNetUsers_ClientId",
                table: "Wallets",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
