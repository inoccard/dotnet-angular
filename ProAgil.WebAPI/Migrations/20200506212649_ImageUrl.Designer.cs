﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProAgil.WebAPI.Data;

namespace ProAgil.WebAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200506212649_ImageUrl")]
    partial class ImageUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("ProAgil.WebAPI.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Local")
                        .HasColumnType("TEXT");

                    b.Property<string>("Lot")
                        .HasColumnType("TEXT");

                    b.Property<int>("PersonQtd")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });
#pragma warning restore 612, 618
        }
    }
}