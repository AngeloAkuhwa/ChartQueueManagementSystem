﻿// <auto-generated />
using System;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatQueueManagementSystem.Persistence.Migrations
{
    [DbContext(typeof(ChatQueueDbContext))]
    partial class ChatQueueDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Agent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrentConcurrentChats")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MaxConcurrentChats")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("OverflowId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SeniorityLevel")
                        .HasColumnType("int");

                    b.Property<double>("SeniorityMultiplier")
                        .HasColumnType("float");

                    b.Property<TimeSpan>("ShiftDuration")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("ShiftStartTime")
                        .HasColumnType("time");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Agent_Name");

                    b.HasIndex("OverflowId");

                    b.HasIndex("TeamId")
                        .HasDatabaseName("IX_Agent_TeamId");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.AssignmentIndexLogVersion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AgentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChatSessionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChatStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CurrentAgentIndex")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("QueueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AgentId", "ChatSessionId", "CurrentAgentIndex")
                        .HasDatabaseName("IX_AssignmentIndexLogVersion_AgentId_ChatSessionId_CurrentAgentIndex");

                    b.ToTable("AssignmentIndexLogVersions");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.ChatSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AgentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("InactivityCounter")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("QueueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AgentId")
                        .HasDatabaseName("IX_ChatSession_AgentId");

                    b.HasIndex("QueueId");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_ChatSession_Status");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_ChatSession_UserId");

                    b.ToTable("ChatSessions");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Overflow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Overflows");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Queue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOverflow")
                        .HasColumnType("bit");

                    b.Property<int>("QueueLength")
                        .HasColumnType("int");

                    b.Property<string>("QueueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("QueueName")
                        .HasDatabaseName("IX_Queue_QueueName");

                    b.ToTable("Queues");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Team_Name");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .HasDatabaseName("IX_User_Email");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Agent", b =>
                {
                    b.HasOne("ChatQueueManagementSystem.Domain.Entities.Overflow", null)
                        .WithMany("Agents")
                        .HasForeignKey("OverflowId");

                    b.HasOne("ChatQueueManagementSystem.Domain.Entities.Team", null)
                        .WithMany("Agents")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.ChatSession", b =>
                {
                    b.HasOne("ChatQueueManagementSystem.Domain.Entities.Queue", null)
                        .WithMany("ChatSessions")
                        .HasForeignKey("QueueId");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Overflow", b =>
                {
                    b.Navigation("Agents");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Queue", b =>
                {
                    b.Navigation("ChatSessions");
                });

            modelBuilder.Entity("ChatQueueManagementSystem.Domain.Entities.Team", b =>
                {
                    b.Navigation("Agents");
                });
#pragma warning restore 612, 618
        }
    }
}
