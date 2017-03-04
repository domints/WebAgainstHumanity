using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WebAgainstHumanity.Models.Db;

namespace WebAgainstHumanity.Migrations
{
    [DbContext(typeof(WahContext))]
    [Migration("20170304231039_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

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
        }
    }
}
