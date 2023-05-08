﻿// <auto-generated />
namespace _3DNetworkSimulatorAPI.Migrations

open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Storage.ValueConversion

[<DbContext(typeof<MyDbContext.ApplicationDbContext>)>]
type ApplicationDbContextModelSnapshot() =
    inherit ModelSnapshot()

    override this.BuildModel(modelBuilder: ModelBuilder) =
        modelBuilder.HasAnnotation("ProductVersion", "6.0.16") |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", (fun b ->

            b.Property<string>("Id")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("Name")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("NormalizedName")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("Id")
                |> ignore


            b.HasIndex("NormalizedName")
                .IsUnique()
                .HasDatabaseName("RoleNameIndex") |> ignore

            b.ToTable("AspNetRoles") |> ignore


            b.HasData([|
                {| Id = "f46fcbd1-8ad5-447a-adf2-3682b3592b22"; ConcurrencyStamp = "dbc3b644-2ec8-49cc-aef5-b58e2b413adc"; Name = "admin"; NormalizedName = "ADMIN" |}
                {| Id = "de3657b8-f385-46ab-be8a-9451b4994c61"; ConcurrencyStamp = "c3849663-1d22-4164-9dad-6e4efe43b1a1"; Name = "user"; NormalizedName = "USER" |}
             |]) |> ignore
        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (fun b ->

            b.Property<int>("Id")
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("ClaimType")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("ClaimValue")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("RoleId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("Id")
                |> ignore


            b.HasIndex("RoleId")
                |> ignore

            b.ToTable("AspNetRoleClaims") |> ignore

        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", (fun b ->

            b.Property<string>("Id")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<int>("AccessFailedCount")
                .IsRequired(true)
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("Email")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<bool>("EmailConfirmed")
                .IsRequired(true)
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<bool>("LockoutEnabled")
                .IsRequired(true)
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<Nullable<DateTimeOffset>>("LockoutEnd")
                .IsRequired(false)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("NormalizedEmail")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("NormalizedUserName")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("PasswordHash")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("PhoneNumber")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<bool>("PhoneNumberConfirmed")
                .IsRequired(true)
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("SecurityStamp")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<bool>("TwoFactorEnabled")
                .IsRequired(true)
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("UserName")
                .IsRequired(true)
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("Id")
                |> ignore


            b.HasIndex("NormalizedEmail")
                .HasDatabaseName("EmailIndex") |> ignore


            b.HasIndex("NormalizedUserName")
                .IsUnique()
                .HasDatabaseName("UserNameIndex") |> ignore

            b.ToTable("AspNetUsers") |> ignore

        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (fun b ->

            b.Property<int>("Id")
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("ClaimType")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("ClaimValue")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("UserId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("Id")
                |> ignore


            b.HasIndex("UserId")
                |> ignore

            b.ToTable("AspNetUserClaims") |> ignore

        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (fun b ->

            b.Property<string>("LoginProvider")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("ProviderKey")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("ProviderDisplayName")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("UserId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("LoginProvider", "ProviderKey")
                |> ignore


            b.HasIndex("UserId")
                |> ignore

            b.ToTable("AspNetUserLogins") |> ignore

        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (fun b ->

            b.Property<string>("UserId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("RoleId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("UserId", "RoleId")
                |> ignore


            b.HasIndex("RoleId")
                |> ignore

            b.ToTable("AspNetUserRoles") |> ignore

        )) |> ignore

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (fun b ->

            b.Property<string>("UserId")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("LoginProvider")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("Name")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.Property<string>("Value")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("UserId", "LoginProvider", "Name")
                |> ignore


            b.ToTable("AspNetUserTokens") |> ignore

        )) |> ignore
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (fun b ->
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore

        )) |> ignore
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (fun b ->
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore

        )) |> ignore
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (fun b ->
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore

        )) |> ignore
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (fun b ->
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore

        )) |> ignore
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (fun b ->
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                |> ignore

        )) |> ignore

