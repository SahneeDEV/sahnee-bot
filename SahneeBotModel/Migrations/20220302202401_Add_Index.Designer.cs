﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SahneeBotModel;

#nullable disable

namespace SahneeBotModel.Migrations
{
    [DbContext(typeof(SahneeBotModelContext))]
    [Migration("20220302202401_Add_Index")]
    partial class Add_Index
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SahneeBotModel.Models.GuildState", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("BoundChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("LastChangelogVersion")
                        .HasColumnType("text");

                    b.Property<bool>("SetRoles")
                        .HasColumnType("boolean");

                    b.Property<string>("WarningRoleColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WarningRolePrefix")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GuildId");

                    b.HasIndex("GuildId", "BoundChannelId");

                    b.HasIndex("GuildId", "SetRoles");

                    b.HasIndex("GuildId", "WarningRoleColor");

                    b.HasIndex("GuildId", "WarningRolePrefix");

                    b.ToTable("GuildStates");
                });

            modelBuilder.Entity("SahneeBotModel.Models.Role", b =>
                {
                    b.Property<decimal>("RoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("RoleType")
                        .HasColumnType("integer");

                    b.HasKey("RoleId", "GuildId");

                    b.HasIndex("GuildId");

                    b.HasIndex("RoleId");

                    b.HasIndex("GuildId", "RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("SahneeBotModel.Models.UserGuildState", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("HasReceivedOptOutHint")
                        .HasColumnType("boolean");

                    b.Property<bool>("MessageOptOut")
                        .HasColumnType("boolean");

                    b.Property<long>("WarningNumber")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "GuildId");

                    b.ToTable("UserGuildStates");
                });

            modelBuilder.Entity("SahneeBotModel.Models.UserState", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime?>("LastDataDeletion")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserStates");
                });

            modelBuilder.Entity("SahneeBotModel.Models.Warning", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("IssuerUserId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Number")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("UserId");

                    b.HasIndex("GuildId", "IssuerUserId");

                    b.HasIndex("GuildId", "Time");

                    b.HasIndex("GuildId", "UserId");

                    b.HasIndex("GuildId", "IssuerUserId", "Time");

                    b.HasIndex("GuildId", "UserId", "Time");

                    b.ToTable("Warnings");
                });
#pragma warning restore 612, 618
        }
    }
}
