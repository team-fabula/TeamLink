using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamLink.Api.Data.Migrations;

/// <inheritdoc />
public partial class _20261127033034_InitialTeamLinkSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: false),
                AvatarUrl = table.Column<string>(type: "text", nullable: true),
                Bio = table.Column<string>(type: "text", nullable: true),
                IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Conversations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                AvatarUrl = table.Column<string>(type: "text", nullable: true),
                CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Conversations", x => x.Id);
                table.CheckConstraint("CK_Conversations_GroupFields", "\"Type\" = 'GROUP' OR (\"Name\" IS NULL AND \"AvatarUrl\" IS NULL)");
                table.CheckConstraint("CK_Conversations_Type", "\"Type\" IN ('PRIVATE', 'GROUP')");
                table.ForeignKey(
                    name: "FK_Conversations_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "Sessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                RefreshTokenHash = table.Column<string>(type: "text", nullable: false),
                Platform = table.Column<string>(type: "text", nullable: false),
                DeviceInfo = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sessions", x => x.Id);
                table.CheckConstraint("CK_Sessions_Platform", "\"Platform\" IN ('WEB', 'IOS', 'ANDROID')");
                table.ForeignKey(
                    name: "FK_Sessions_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                SenderId = table.Column<Guid>(type: "uuid", nullable: true),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Messages", x => x.Id);
                table.CheckConstraint("CK_Messages_DeletionState", "(\"IsDeleted\" = false AND \"DeletedAt\" IS NULL) OR (\"IsDeleted\" = true AND \"DeletedAt\" IS NOT NULL AND \"Content\" IS NULL)");
                table.CheckConstraint("CK_Messages_Type", "\"Type\" IN ('TEXT', 'FILE', 'SYSTEM')");
                table.ForeignKey(
                    name: "FK_Messages_Conversations_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Messages_Users_SenderId",
                    column: x => x.SenderId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "Attachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                FileName = table.Column<string>(type: "text", nullable: false),
                MimeType = table.Column<string>(type: "text", nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                StorageKey = table.Column<string>(type: "text", nullable: false),
                UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Attachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Attachments_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ConversationParticipants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Role = table.Column<string>(type: "text", nullable: false),
                AddedById = table.Column<Guid>(type: "uuid", nullable: true),
                JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LeftAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                RemovedById = table.Column<Guid>(type: "uuid", nullable: true),
                LastReadMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                LastReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConversationParticipants", x => x.Id);
                table.CheckConstraint("CK_ConversationParticipants_Role", "\"Role\" IN ('MEMBER', 'ADMIN')");
                table.ForeignKey(
                    name: "FK_ConversationParticipants_Conversations_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ConversationParticipants_Messages_LastReadMessageId",
                    column: x => x.LastReadMessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_ConversationParticipants_Users_AddedById",
                    column: x => x.AddedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_ConversationParticipants_Users_RemovedById",
                    column: x => x.RemovedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_ConversationParticipants_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MessageReceipts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MessageReceipts", x => x.Id);
                table.ForeignKey(
                    name: "FK_MessageReceipts_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MessageReceipts_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                MessageId = table.Column<Guid>(type: "uuid", nullable: true),
                Type = table.Column<string>(type: "text", nullable: false),
                Text = table.Column<string>(type: "text", nullable: false),
                IsRead = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
                table.CheckConstraint("CK_Notifications_Type", "\"Type\" IN ('NEW_MESSAGE', 'ADDED_TO_GROUP', 'REMOVED_FROM_GROUP', 'ADMIN_GRANTED', 'ADMIN_REVOKED')");
                table.ForeignKey(
                    name: "FK_Notifications_Conversations_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Notifications_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Notifications_Users_RecipientId",
                    column: x => x.RecipientId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PinnedMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                PinnedById = table.Column<Guid>(type: "uuid", nullable: false),
                PinnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PinnedMessages", x => x.Id);
                table.ForeignKey(
                    name: "FK_PinnedMessages_Conversations_ConversationId",
                    column: x => x.ConversationId,
                    principalTable: "Conversations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PinnedMessages_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PinnedMessages_Users_PinnedById",
                    column: x => x.PinnedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Reactions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                ReactionType = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reactions", x => x.Id);
                table.CheckConstraint("CK_Reactions_ReactionType", "\"ReactionType\" IN ('LIKE', 'LOVE', 'LAUGH', 'SAD', 'THUMBS_UP')");
                table.ForeignKey(
                    name: "FK_Reactions_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reactions_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Attachments_MessageId",
            table: "Attachments",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_ConversationParticipants_AddedById",
            table: "ConversationParticipants",
            column: "AddedById");

        migrationBuilder.CreateIndex(
            name: "IX_ConversationParticipants_ConversationId_UserId",
            table: "ConversationParticipants",
            columns: new[] { "ConversationId", "UserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ConversationParticipants_LastReadMessageId",
            table: "ConversationParticipants",
            column: "LastReadMessageId");

        migrationBuilder.CreateIndex(
            name: "IX_ConversationParticipants_RemovedById",
            table: "ConversationParticipants",
            column: "RemovedById");

        migrationBuilder.CreateIndex(
            name: "IX_ConversationParticipants_UserId",
            table: "ConversationParticipants",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Conversations_CreatedById",
            table: "Conversations",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Conversations_LastMessageAt",
            table: "Conversations",
            column: "LastMessageAt");

        migrationBuilder.CreateIndex(
            name: "IX_MessageReceipts_MessageId_UserId",
            table: "MessageReceipts",
            columns: new[] { "MessageId", "UserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MessageReceipts_UserId",
            table: "MessageReceipts",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_Content_Active_Trigram",
            table: "Messages",
            column: "Content",
            filter: "\"IsDeleted\" = false AND \"Content\" IS NOT NULL")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

        migrationBuilder.CreateIndex(
            name: "IX_Messages_ConversationId_CreatedAt",
            table: "Messages",
            columns: new[] { "ConversationId", "CreatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Messages_SenderId",
            table: "Messages",
            column: "SenderId");

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_ConversationId",
            table: "Notifications",
            column: "ConversationId");

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_MessageId",
            table: "Notifications",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_RecipientId_IsRead_CreatedAt",
            table: "Notifications",
            columns: new[] { "RecipientId", "IsRead", "CreatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_PinnedMessages_ConversationId",
            table: "PinnedMessages",
            column: "ConversationId");

        migrationBuilder.CreateIndex(
            name: "IX_PinnedMessages_MessageId",
            table: "PinnedMessages",
            column: "MessageId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PinnedMessages_PinnedById",
            table: "PinnedMessages",
            column: "PinnedById");

        migrationBuilder.CreateIndex(
            name: "IX_Reactions_MessageId_UserId",
            table: "Reactions",
            columns: new[] { "MessageId", "UserId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Reactions_UserId",
            table: "Reactions",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Sessions_UserId_ExpiresAt",
            table: "Sessions",
            columns: new[] { "UserId", "ExpiresAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Attachments");

        migrationBuilder.DropTable(
            name: "ConversationParticipants");

        migrationBuilder.DropTable(
            name: "MessageReceipts");

        migrationBuilder.DropTable(
            name: "Notifications");

        migrationBuilder.DropTable(
            name: "PinnedMessages");

        migrationBuilder.DropTable(
            name: "Reactions");

        migrationBuilder.DropTable(
            name: "Sessions");

        migrationBuilder.DropTable(
            name: "Messages");

        migrationBuilder.DropTable(
            name: "Conversations");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
    }
}
