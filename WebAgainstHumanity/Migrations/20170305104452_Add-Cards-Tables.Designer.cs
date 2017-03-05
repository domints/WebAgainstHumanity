using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Migrations
{
    [DbContext(typeof(WahContext))]
    [Migration("20170305104452_Add-Cards-Tables")]
    partial class AddCardsTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("WebAgainstHumanity.Models.Db.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("crdid");

                    b.Property<int>("CardSetId")
                        .HasColumnName("crdcdsid");

                    b.Property<string>("Content")
                        .HasColumnName("crdcontent");

                    b.Property<string>("Guid")
                        .HasColumnName("crdguid");

                    b.Property<bool>("IsQuestion")
                        .HasColumnName("crdisquestion");

                    b.HasKey("Id");

                    b.HasIndex("CardSetId");

                    b.ToTable("card");
                });

            modelBuilder.Entity("WebAgainstHumanity.Models.Db.CardSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("cdsid");

                    b.Property<DateTime>("Added")
                        .HasColumnName("cdsadded");

                    b.Property<string>("Author")
                        .HasColumnName("cdsauthor");

                    b.Property<string>("Guid")
                        .HasColumnName("cdsguid");

                    b.Property<string>("Language")
                        .HasColumnName("cdslang");

                    b.Property<string>("Name")
                        .HasColumnName("cdsname");

                    b.HasKey("Id");

                    b.ToTable("cardset");
                });

            modelBuilder.Entity("WebAgainstHumanity.Models.Db.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("usrid");

                    b.Property<bool>("Active")
                        .HasColumnName("usractive");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnName("usrcreatedate");

                    b.Property<string>("Email")
                        .HasColumnName("usremail");

                    b.Property<string>("Nick")
                        .HasColumnName("usrnick");

                    b.Property<string>("Password")
                        .HasColumnName("usrpassword");

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("WebAgainstHumanity.Models.Db.Card", b =>
                {
                    b.HasOne("WebAgainstHumanity.Models.Db.CardSet", "CardSet")
                        .WithMany("Cards")
                        .HasForeignKey("CardSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
