﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MuzakBot.Database;

#nullable disable

namespace MuzakBot.Database.Migrations.AlbumReleaseDb
{
    [DbContext(typeof(AlbumReleaseDbContext))]
    [Migration("20240510025424_Update_ulong-to-string")]
    partial class Update_ulongtostring
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("MuzakBot.Lib.Models.Database.AlbumRelease.AlbumReleaseReminder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("id")
                        .HasAnnotation("Cosmos:PropertyName", "id")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<string>("AlbumId")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("albumId")
                        .HasAnnotation("Cosmos:PropertyName", "albumId")
                        .HasAnnotation("Relational:JsonPropertyName", "albumId");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("channelId")
                        .HasAnnotation("Cosmos:PropertyName", "channelId")
                        .HasAnnotation("Relational:JsonPropertyName", "channelId");

                    b.Property<string>("GuildId")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("guildId")
                        .HasAnnotation("Cosmos:PropertyName", "guildId")
                        .HasAnnotation("Relational:JsonPropertyName", "guildId");

                    b.Property<string>("PartitionKey")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("partitionKey")
                        .HasAnnotation("Cosmos:PropertyName", "partitionKey")
                        .HasAnnotation("Relational:JsonPropertyName", "partitionKey");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("releaseDate")
                        .HasAnnotation("Cosmos:PropertyName", "releaseDate")
                        .HasAnnotation("Relational:JsonPropertyName", "releaseDate");

                    b.Property<bool>("ReminderSent")
                        .HasColumnType("INTEGER")
                        .HasColumnName("reminderSent")
                        .HasAnnotation("Cosmos:PropertyName", "reminderSent")
                        .HasAnnotation("Relational:JsonPropertyName", "reminderSent");

                    b.Property<string>("UserIdsToRemind")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("userIdsToRemind")
                        .HasAnnotation("Cosmos:PropertyName", "userIdsToRemind")
                        .HasAnnotation("Relational:JsonPropertyName", "userIdsToRemind");

                    b.HasKey("Id");

                    b.ToTable("release_reminders", (string)null);

                    b
                        .HasAnnotation("Cosmos:ContainerName", "release_reminders")
                        .HasAnnotation("Cosmos:PartitionKeyName", "PartitionKey");
                });
#pragma warning restore 612, 618
        }
    }
}
