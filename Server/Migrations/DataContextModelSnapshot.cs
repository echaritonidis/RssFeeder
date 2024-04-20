﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RssFeeder.Server.Infrastructure.Database;

#nullable disable

namespace RssFeeder.Server.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Feed", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Default")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Favorite")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("FeedGroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Href")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FeedGroupId");

                    b.ToTable("Feed");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.FeedGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Initial")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FeedGroup");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f5de41b1-89c4-488a-9bad-f92231fdfc33"),
                            Color = "#000",
                            Description = "Generic specific category or topic",
                            Initial = true,
                            Order = 0,
                            Title = "Unclassified"
                        });
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FeedId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FeedId");

                    b.ToTable("Label");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Settings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("DarkMode")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Feed", b =>
                {
                    b.HasOne("RssFeeder.Server.Infrastructure.Model.FeedGroup", "FeedGroup")
                        .WithMany("Feeds")
                        .HasForeignKey("FeedGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FeedGroup");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Label", b =>
                {
                    b.HasOne("RssFeeder.Server.Infrastructure.Model.Feed", "Feed")
                        .WithMany("Labels")
                        .HasForeignKey("FeedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feed");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.Feed", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("RssFeeder.Server.Infrastructure.Model.FeedGroup", b =>
                {
                    b.Navigation("Feeds");
                });
#pragma warning restore 612, 618
        }
    }
}