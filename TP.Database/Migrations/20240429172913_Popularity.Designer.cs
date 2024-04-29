﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TP.Database;

#nullable disable

namespace TP.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240429172913_Popularity")]
    partial class Popularity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TP.Domain.Tour", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool?>("ChildFriendliness")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Distance")
                        .HasPrecision(10, 3)
                        .HasColumnType("numeric(10,3)");

                    b.Property<string>("End")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<TimeSpan>("EstimatedTime")
                        .HasColumnType("interval");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Popularity")
                        .HasColumnType("text");

                    b.Property<string>("Start")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TransportType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UnprocessedLogsCounter")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Tours");
                });

            modelBuilder.Entity("TP.Domain.TourLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("timezone('utc', CURRENT_TIMESTAMP)");

                    b.Property<string>("Difficulty")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<short>("Rating")
                        .HasColumnType("smallint");

                    b.Property<decimal>("TotalDistance")
                        .HasPrecision(12, 3)
                        .HasColumnType("numeric(12,3)");

                    b.Property<TimeSpan>("TotalTime")
                        .HasColumnType("interval");

                    b.Property<Guid>("TourId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("TourId");

                    b.ToTable("TourLogs");
                });

            modelBuilder.Entity("TP.Domain.TourLog", b =>
                {
                    b.HasOne("TP.Domain.Tour", "Tour")
                        .WithMany("TourLogs")
                        .HasForeignKey("TourId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tour");
                });

            modelBuilder.Entity("TP.Domain.Tour", b =>
                {
                    b.Navigation("TourLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
