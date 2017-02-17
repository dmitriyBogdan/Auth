using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Auth.DAL;

namespace Auth.DAL.Migrations
{
    [DbContext(typeof(AuthContext))]
    [Migration("20170217103136_AddFieldsToExternalUser")]
    partial class AddFieldsToExternalUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Auth.DAL.Entities.Claim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("Auth.DAL.Entities.ExternalUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<string>("NameIdentifier");

                    b.Property<string>("Provider");

                    b.Property<string>("Surname");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExternalUsers");
                });

            modelBuilder.Entity("Auth.DAL.Entities.LocalUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<string>("Salt");

                    b.HasKey("Id");

                    b.ToTable("LocalUsers");
                });

            modelBuilder.Entity("Auth.DAL.Entities.LocalUserClaim", b =>
                {
                    b.Property<int>("LocalUserId");

                    b.Property<int>("ClaimId");

                    b.HasKey("LocalUserId", "ClaimId");

                    b.HasIndex("ClaimId");

                    b.ToTable("LocalUserClaims");
                });

            modelBuilder.Entity("Auth.DAL.Entities.ExternalUser", b =>
                {
                    b.HasOne("Auth.DAL.Entities.LocalUser", "User")
                        .WithMany("ExternalUsers")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Auth.DAL.Entities.LocalUserClaim", b =>
                {
                    b.HasOne("Auth.DAL.Entities.Claim", "Claim")
                        .WithMany("LocalUserClaims")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Auth.DAL.Entities.LocalUser", "LocalUser")
                        .WithMany("LocalUserClaims")
                        .HasForeignKey("LocalUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
