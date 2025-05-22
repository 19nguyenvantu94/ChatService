using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatService.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAttachment_Messages_MessageId1",
                table: "MessageAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageEmoji_Messages_MessageId1",
                table: "MessageEmoji");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageEmoji",
                table: "MessageEmoji");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageAttachment",
                table: "MessageAttachment");

            migrationBuilder.RenameTable(
                name: "MessageEmoji",
                newName: "MessageEmojis");

            migrationBuilder.RenameTable(
                name: "MessageAttachment",
                newName: "MessageAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_MessageEmoji_MessageId1",
                table: "MessageEmojis",
                newName: "IX_MessageEmojis_MessageId1");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachment_MessageId1",
                table: "MessageAttachments",
                newName: "IX_MessageAttachments_MessageId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageEmojis",
                table: "MessageEmojis",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageAttachments",
                table: "MessageAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAttachments_Messages_MessageId1",
                table: "MessageAttachments",
                column: "MessageId1",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageEmojis_Messages_MessageId1",
                table: "MessageEmojis",
                column: "MessageId1",
                principalTable: "Messages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAttachments_Messages_MessageId1",
                table: "MessageAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageEmojis_Messages_MessageId1",
                table: "MessageEmojis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageEmojis",
                table: "MessageEmojis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageAttachments",
                table: "MessageAttachments");

            migrationBuilder.RenameTable(
                name: "MessageEmojis",
                newName: "MessageEmoji");

            migrationBuilder.RenameTable(
                name: "MessageAttachments",
                newName: "MessageAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_MessageEmojis_MessageId1",
                table: "MessageEmoji",
                newName: "IX_MessageEmoji_MessageId1");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachments_MessageId1",
                table: "MessageAttachment",
                newName: "IX_MessageAttachment_MessageId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageEmoji",
                table: "MessageEmoji",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageAttachment",
                table: "MessageAttachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAttachment_Messages_MessageId1",
                table: "MessageAttachment",
                column: "MessageId1",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageEmoji_Messages_MessageId1",
                table: "MessageEmoji",
                column: "MessageId1",
                principalTable: "Messages",
                principalColumn: "Id");
        }
    }
}
