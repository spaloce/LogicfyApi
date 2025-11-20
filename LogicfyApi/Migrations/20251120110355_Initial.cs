using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicfyApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XP = table.Column<int>(type: "int", nullable: false),
                    Seviye = table.Column<int>(type: "int", nullable: false),
                    Streak = table.Column<int>(type: "int", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramlamaDilleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IkonUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramlamaDilleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciGunlukSerileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeriSayisi = table.Column<int>(type: "int", nullable: false),
                    SonGiris = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciGunlukSerileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciGunlukSerileri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciXpLoglari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kaynak = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Xp = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciXpLoglari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciXpLoglari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Uniteler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramlamaDiliId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uniteler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Uniteler_ProgramlamaDilleri_ProgramlamaDiliId",
                        column: x => x.ProgramlamaDiliId,
                        principalTable: "ProgramlamaDilleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kisimlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniteId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    DersSayisiCache = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kisimlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kisimlar_Uniteler_UniteId",
                        column: x => x.UniteId,
                        principalTable: "Uniteler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciUniteIlerlemeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniteId = table.Column<int>(type: "int", nullable: false),
                    TamamlananDersSayisi = table.Column<int>(type: "int", nullable: false),
                    ToplamDersSayisi = table.Column<int>(type: "int", nullable: false),
                    IlerlemeOrani = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciUniteIlerlemeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciUniteIlerlemeleri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciUniteIlerlemeleri_Uniteler_UniteId",
                        column: x => x.UniteId,
                        principalTable: "Uniteler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniteTakipler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniteId = table.Column<int>(type: "int", nullable: false),
                    TakipEdenKullaniciSayisi = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniteTakipler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniteTakipler_Uniteler_UniteId",
                        column: x => x.UniteId,
                        principalTable: "Uniteler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dersler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisimId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    TahminiSure = table.Column<int>(type: "int", nullable: false),
                    SoruSayisiCache = table.Column<int>(type: "int", nullable: false),
                    ZorlukSeviyesi = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dersler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dersler_Kisimlar_KisimId",
                        column: x => x.KisimId,
                        principalTable: "Kisimlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciKisimIlerlemeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KisimId = table.Column<int>(type: "int", nullable: false),
                    TamamlananDersSayisi = table.Column<int>(type: "int", nullable: false),
                    ToplamDersSayisi = table.Column<int>(type: "int", nullable: false),
                    IlerlemeOrani = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciKisimIlerlemeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciKisimIlerlemeleri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciKisimIlerlemeleri_Kisimlar_KisimId",
                        column: x => x.KisimId,
                        principalTable: "Kisimlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DersPerformansAnalitikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DersId = table.Column<int>(type: "int", nullable: false),
                    OrtalamaTamamlamaSuresi = table.Column<double>(type: "float", nullable: false),
                    OrtalamaDogruOrani = table.Column<double>(type: "float", nullable: false),
                    EnZorSoruId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersPerformansAnalitikler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DersPerformansAnalitikler_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DersTakipler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DersId = table.Column<int>(type: "int", nullable: false),
                    TakipEdenKullaniciSayisi = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersTakipler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DersTakipler_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciDersIlerlemeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DersId = table.Column<int>(type: "int", nullable: false),
                    TamamlananSoruSayisi = table.Column<int>(type: "int", nullable: false),
                    ToplamSoruSayisi = table.Column<int>(type: "int", nullable: false),
                    IlerlemeOrani = table.Column<int>(type: "int", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDersIlerlemeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciDersIlerlemeleri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciDersIlerlemeleri_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciDersKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DersId = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    DersId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDersKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciDersKayitlari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciDersKayitlari_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciDersKayitlari_Dersler_DersId1",
                        column: x => x.DersId1,
                        principalTable: "Dersler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KullaniciSoruCevaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    DogruMu = table.Column<bool>(type: "bit", nullable: false),
                    CevapJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SureMs = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciSoruCevaplari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciSoruCevaplari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoruAnalitikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    CevaplanmaSayisi = table.Column<int>(type: "int", nullable: false),
                    DogruSayisi = table.Column<int>(type: "int", nullable: false),
                    YanlisSayisi = table.Column<int>(type: "int", nullable: false),
                    OrtalamaSure = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoruAnalitikler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoruCanliPreviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    DogruHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogruCss = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GerekenEtiketlerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GerekenStillerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoruCanliPreviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoruFonksiyonCozumler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    CozumKod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoruFonksiyonCozumler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoruKelimeBloklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    DogruKod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KelimelerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoruKelimeBloklar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sorular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DersId = table.Column<int>(type: "int", nullable: false),
                    SoruTipi = table.Column<int>(type: "int", nullable: false),
                    SoruMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KodMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seviye = table.Column<int>(type: "int", nullable: false),
                    DogruCevapId = table.Column<int>(type: "int", nullable: true),
                    EkVeriJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DersId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sorular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sorular_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sorular_Dersler_DersId1",
                        column: x => x.DersId1,
                        principalTable: "Dersler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SoruSecenekleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    SecenekMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoruSecenekleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoruSecenekleri_Sorular_SoruId",
                        column: x => x.SoruId,
                        principalTable: "Sorular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dersler_Baslik",
                table: "Dersler",
                column: "Baslik");

            migrationBuilder.CreateIndex(
                name: "IX_Dersler_KisimId",
                table: "Dersler",
                column: "KisimId");

            migrationBuilder.CreateIndex(
                name: "IX_Dersler_ZorlukSeviyesi",
                table: "Dersler",
                column: "ZorlukSeviyesi");

            migrationBuilder.CreateIndex(
                name: "IX_DersPerformansAnalitikler_DersId",
                table: "DersPerformansAnalitikler",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_DersTakipler_DersId",
                table: "DersTakipler",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Kisimlar_Baslik",
                table: "Kisimlar",
                column: "Baslik");

            migrationBuilder.CreateIndex(
                name: "IX_Kisimlar_UniteId",
                table: "Kisimlar",
                column: "UniteId");

            migrationBuilder.CreateIndex(
                name: "IX_Kisimlar_UniteId_Sira",
                table: "Kisimlar",
                columns: new[] { "UniteId", "Sira" });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersIlerlemeleri_DersId",
                table: "KullaniciDersIlerlemeleri",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersIlerlemeleri_KullaniciId_DersId",
                table: "KullaniciDersIlerlemeleri",
                columns: new[] { "KullaniciId", "DersId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersKayitlari_DersId",
                table: "KullaniciDersKayitlari",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersKayitlari_DersId1",
                table: "KullaniciDersKayitlari",
                column: "DersId1");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersKayitlari_KullaniciId_DersId",
                table: "KullaniciDersKayitlari",
                columns: new[] { "KullaniciId", "DersId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciGunlukSerileri_KullaniciId",
                table: "KullaniciGunlukSerileri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKisimIlerlemeleri_KisimId",
                table: "KullaniciKisimIlerlemeleri",
                column: "KisimId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKisimIlerlemeleri_KullaniciId",
                table: "KullaniciKisimIlerlemeleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSoruCevaplari_KullaniciId_SoruId",
                table: "KullaniciSoruCevaplari",
                columns: new[] { "KullaniciId", "SoruId" });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSoruCevaplari_SoruId",
                table: "KullaniciSoruCevaplari",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciUniteIlerlemeleri_KullaniciId",
                table: "KullaniciUniteIlerlemeleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciUniteIlerlemeleri_UniteId",
                table: "KullaniciUniteIlerlemeleri",
                column: "UniteId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciXpLoglari_KullaniciId",
                table: "KullaniciXpLoglari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramlamaDilleri_Ad",
                table: "ProgramlamaDilleri",
                column: "Ad");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramlamaDilleri_AktifMi",
                table: "ProgramlamaDilleri",
                column: "AktifMi");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramlamaDilleri_Kod",
                table: "ProgramlamaDilleri",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoruAnalitikler_SoruId",
                table: "SoruAnalitikler",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_SoruCanliPreviews_SoruId",
                table: "SoruCanliPreviews",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_SoruFonksiyonCozumler_SoruId",
                table: "SoruFonksiyonCozumler",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_SoruKelimeBloklar_SoruId",
                table: "SoruKelimeBloklar",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_DersId",
                table: "Sorular",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_DersId1",
                table: "Sorular",
                column: "DersId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sorular_DogruCevapId",
                table: "Sorular",
                column: "DogruCevapId");

            migrationBuilder.CreateIndex(
                name: "IX_SoruSecenekleri_SoruId",
                table: "SoruSecenekleri",
                column: "SoruId");

            migrationBuilder.CreateIndex(
                name: "IX_Uniteler_Baslik",
                table: "Uniteler",
                column: "Baslik");

            migrationBuilder.CreateIndex(
                name: "IX_Uniteler_ProgramlamaDiliId",
                table: "Uniteler",
                column: "ProgramlamaDiliId");

            migrationBuilder.CreateIndex(
                name: "IX_Uniteler_ProgramlamaDiliId_Sira",
                table: "Uniteler",
                columns: new[] { "ProgramlamaDiliId", "Sira" });

            migrationBuilder.CreateIndex(
                name: "IX_Uniteler_Sira",
                table: "Uniteler",
                column: "Sira");

            migrationBuilder.CreateIndex(
                name: "IX_UniteTakipler_UniteId",
                table: "UniteTakipler",
                column: "UniteId");

            migrationBuilder.AddForeignKey(
                name: "FK_KullaniciSoruCevaplari_Sorular_SoruId",
                table: "KullaniciSoruCevaplari",
                column: "SoruId",
                principalTable: "Sorular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoruAnalitikler_Sorular_SoruId",
                table: "SoruAnalitikler",
                column: "SoruId",
                principalTable: "Sorular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoruCanliPreviews_Sorular_SoruId",
                table: "SoruCanliPreviews",
                column: "SoruId",
                principalTable: "Sorular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoruFonksiyonCozumler_Sorular_SoruId",
                table: "SoruFonksiyonCozumler",
                column: "SoruId",
                principalTable: "Sorular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SoruKelimeBloklar_Sorular_SoruId",
                table: "SoruKelimeBloklar",
                column: "SoruId",
                principalTable: "Sorular",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sorular_SoruSecenekleri_DogruCevapId",
                table: "Sorular",
                column: "DogruCevapId",
                principalTable: "SoruSecenekleri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dersler_Kisimlar_KisimId",
                table: "Dersler");

            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_Dersler_DersId",
                table: "Sorular");

            migrationBuilder.DropForeignKey(
                name: "FK_Sorular_Dersler_DersId1",
                table: "Sorular");

            migrationBuilder.DropForeignKey(
                name: "FK_SoruSecenekleri_Sorular_SoruId",
                table: "SoruSecenekleri");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DersPerformansAnalitikler");

            migrationBuilder.DropTable(
                name: "DersTakipler");

            migrationBuilder.DropTable(
                name: "KullaniciDersIlerlemeleri");

            migrationBuilder.DropTable(
                name: "KullaniciDersKayitlari");

            migrationBuilder.DropTable(
                name: "KullaniciGunlukSerileri");

            migrationBuilder.DropTable(
                name: "KullaniciKisimIlerlemeleri");

            migrationBuilder.DropTable(
                name: "KullaniciSoruCevaplari");

            migrationBuilder.DropTable(
                name: "KullaniciUniteIlerlemeleri");

            migrationBuilder.DropTable(
                name: "KullaniciXpLoglari");

            migrationBuilder.DropTable(
                name: "SoruAnalitikler");

            migrationBuilder.DropTable(
                name: "SoruCanliPreviews");

            migrationBuilder.DropTable(
                name: "SoruFonksiyonCozumler");

            migrationBuilder.DropTable(
                name: "SoruKelimeBloklar");

            migrationBuilder.DropTable(
                name: "UniteTakipler");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Kisimlar");

            migrationBuilder.DropTable(
                name: "Uniteler");

            migrationBuilder.DropTable(
                name: "ProgramlamaDilleri");

            migrationBuilder.DropTable(
                name: "Dersler");

            migrationBuilder.DropTable(
                name: "Sorular");

            migrationBuilder.DropTable(
                name: "SoruSecenekleri");
        }
    }
}
