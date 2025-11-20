using LogicfyApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace LogicfyApi.Data
{
    public class AppDbContext : IdentityDbContext<Kullanici>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Kullanıcı İlişkili Tablolar
        public DbSet<KullaniciDersKaydi> KullaniciDersKayitlari { get; set; }
        public DbSet<KullaniciDersIlerleme> KullaniciDersIlerlemeleri { get; set; }
        public DbSet<KullaniciUnitProgress> KullaniciUniteIlerlemeleri { get; set; }
        public DbSet<KullaniciKisimProgress> KullaniciKisimIlerlemeleri { get; set; }
        public DbSet<KullaniciSoruCevap> KullaniciSoruCevaplari { get; set; }
        public DbSet<KullaniciXpLog> KullaniciXpLoglari { get; set; }
        public DbSet<KullaniciGunlukSeri> KullaniciGunlukSerileri { get; set; }

        // Diğer Tablolar (Örnek)
        public DbSet<ProgramlamaDili> ProgramlamaDilleri { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<Unite> Uniteler { get; set; }
        public DbSet<Kisim> Kisimlar { get; set; }
        public DbSet<Soru> Sorular { get; set; }
        public DbSet<SoruSecenek> SoruSecenekleri { get; set; }
        public DbSet<SoruKelimeBlok> SoruKelimeBloklar { get; set; }
        public DbSet<SoruFonksiyonCozum> SoruFonksiyonCozumler { get; set; }
        public DbSet<SoruCanliPreview> SoruCanliPreviews { get; set; }
        public DbSet<DersTakip> DersTakipler { get; set; }
        public DbSet<UniteTakip> UniteTakipler { get; set; }
        public DbSet<DersPerformansAnalitik> DersPerformansAnalitikler { get; set; }
        public DbSet<SoruAnalitik> SoruAnalitikler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kullanici - KullaniciDersKaydi (1:N)
            modelBuilder.Entity<KullaniciDersKaydi>()
                .HasOne(x => x.Kullanici)
                .WithMany(x => x.DersKayitlari)
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // KullaniciDersKaydi - Ders (N:1)
            modelBuilder.Entity<KullaniciDersKaydi>()
                .HasOne(x => x.Ders)
                .WithMany()
                .HasForeignKey(x => x.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciDersIlerleme (1:N)
            modelBuilder.Entity<KullaniciDersIlerleme>()
                .HasOne(x => x.Kullanici)
                .WithMany(x => x.DersIlerlemeleri)
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // KullaniciDersIlerleme - Ders (N:1)
            modelBuilder.Entity<KullaniciDersIlerleme>()
                .HasOne(x => x.Ders)
                .WithMany()
                .HasForeignKey(x => x.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciUnitProgress (1:N)
            modelBuilder.Entity<KullaniciUnitProgress>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // KullaniciUnitProgress - Unite (N:1)
            modelBuilder.Entity<KullaniciUnitProgress>()
                .HasOne(x => x.Unite)
                .WithMany()
                .HasForeignKey(x => x.UniteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciKisimProgress (1:N)
            modelBuilder.Entity<KullaniciKisimProgress>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // KullaniciKisimProgress - Kisim (N:1)
            modelBuilder.Entity<KullaniciKisimProgress>()
                .HasOne(x => x.Kisim)
                .WithMany()
                .HasForeignKey(x => x.KisimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciSoruCevap (1:N)
            modelBuilder.Entity<KullaniciSoruCevap>()
                .HasOne(x => x.Kullanici)
                .WithMany(x => x.SoruCevaplari)
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // KullaniciSoruCevap - Soru (N:1)
            modelBuilder.Entity<KullaniciSoruCevap>()
                .HasOne(x => x.Soru)
                .WithMany()
                .HasForeignKey(x => x.SoruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciXpLog (1:N)
            modelBuilder.Entity<KullaniciXpLog>()
                .HasOne(x => x.Kullanici)
                .WithMany(x => x.XpLoglari)
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - KullaniciGunlukSeri (1:1)
            modelBuilder.Entity<KullaniciGunlukSeri>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<KullaniciDersKaydi>()
                .HasIndex(x => new { x.KullaniciId, x.DersId })
                .IsUnique();

            modelBuilder.Entity<KullaniciDersIlerleme>()
                .HasIndex(x => new { x.KullaniciId, x.DersId })
                .IsUnique();

            modelBuilder.Entity<KullaniciSoruCevap>()
                .HasIndex(x => new { x.KullaniciId, x.SoruId });

            // ===== SORU İLİŞKİLERİ =====

            // Soru - Ders (N:1)
            modelBuilder.Entity<Soru>()
                .HasOne(x => x.Ders)
                .WithMany()
                .HasForeignKey(x => x.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soru - DogruCevap (SoruSecenek) (1:1)
            modelBuilder.Entity<Soru>()
                .HasOne(x => x.DogruCevap)
                .WithMany()
                .HasForeignKey(x => x.DogruCevapId)
                .OnDelete(DeleteBehavior.NoAction);

            // Soru - SoruSecenek (1:N)
            modelBuilder.Entity<SoruSecenek>()
                .HasOne(x => x.Soru)
                .WithMany(x => x.Secenekler)
                .HasForeignKey(x => x.SoruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soru - SoruKelimeBlok (1:N)
            modelBuilder.Entity<SoruKelimeBlok>()
                .HasOne(x => x.Soru)
                .WithMany(x => x.KelimeBloklar)
                .HasForeignKey(x => x.SoruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soru - SoruFonksiyonCozum (1:N)
            modelBuilder.Entity<SoruFonksiyonCozum>()
                .HasOne(x => x.Soru)
                .WithMany(x => x.FonksiyonCozumler)
                .HasForeignKey(x => x.SoruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Soru - SoruCanliPreview (1:N)
            modelBuilder.Entity<SoruCanliPreview>()
                .HasOne(x => x.Soru)
                .WithMany(x => x.CanliPreviews)
                .HasForeignKey(x => x.SoruId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEXES =====

            modelBuilder.Entity<Soru>()
                .HasIndex(x => x.DersId);

            modelBuilder.Entity<SoruSecenek>()
                .HasIndex(x => x.SoruId);

            modelBuilder.Entity<SoruKelimeBlok>()
                .HasIndex(x => x.SoruId);

            modelBuilder.Entity<SoruFonksiyonCozum>()
                .HasIndex(x => x.SoruId);

            modelBuilder.Entity<SoruCanliPreview>()
                .HasIndex(x => x.SoruId);

            // ===== KISIM İLİŞKİLERİ =====

            // Kisim - Unite (N:1)
            modelBuilder.Entity<Kisim>()
                .HasOne(x => x.Unite)
                .WithMany(x => x.Kisimlar)
                .HasForeignKey(x => x.UniteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEXES =====

            modelBuilder.Entity<Ders>()
                .HasIndex(x => x.KisimId);

            modelBuilder.Entity<Ders>()
                .HasIndex(x => x.ZorlukSeviyesi);

            modelBuilder.Entity<Ders>()
                .HasIndex(x => x.Baslik);

            modelBuilder.Entity<Kisim>()
                .HasIndex(x => x.UniteId);

            modelBuilder.Entity<Kisim>()
                .HasIndex(x => x.Baslik);

            modelBuilder.Entity<Unite>()
                .HasIndex(x => x.Baslik);

            // ===== KISIM İLİŞKİLERİ =====

            // Kisim - Unite (N:1)
            modelBuilder.Entity<Kisim>()
                .HasOne(x => x.Unite)
                .WithMany(x => x.Kisimlar)
                .HasForeignKey(x => x.UniteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kisim - Ders (1:N) - Already configured in Ders entity
            modelBuilder.Entity<Ders>()
                .HasOne(x => x.Kisim)
                .WithMany(x => x.Dersler)
                .HasForeignKey(x => x.KisimId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEXES =====

            modelBuilder.Entity<Kisim>()
                .HasIndex(x => x.UniteId);

            modelBuilder.Entity<Kisim>()
                .HasIndex(x => x.Baslik);

            modelBuilder.Entity<Kisim>()
                .HasIndex(x => new { x.UniteId, x.Sira });

            modelBuilder.Entity<Unite>()
                .HasIndex(x => x.Baslik);

            modelBuilder.Entity<Unite>()
                .HasIndex(x => x.Sira);

            // ===== PROGRAMLAMA DİLİ - UNITE İLİŞKİSİ =====

            // ProgramlamaDili - Unite (1:N)
            modelBuilder.Entity<Unite>()
                .HasOne(x => x.ProgramlamaDili)
                .WithMany(x => x.Uniteler)
                .HasForeignKey(x => x.ProgramlamaDiliId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unite - Kisim (1:N) - Already configured in Kisim entity
            modelBuilder.Entity<Kisim>()
                .HasOne(x => x.Unite)
                .WithMany(x => x.Kisimlar)
                .HasForeignKey(x => x.UniteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== INDEXES =====

            modelBuilder.Entity<ProgramlamaDili>()
                .HasIndex(x => x.Kod)
                .IsUnique();

            modelBuilder.Entity<ProgramlamaDili>()
                .HasIndex(x => x.Ad);

            modelBuilder.Entity<ProgramlamaDili>()
                .HasIndex(x => x.AktifMi);

            modelBuilder.Entity<Unite>()
                .HasIndex(x => x.ProgramlamaDiliId);

            modelBuilder.Entity<Unite>()
                .HasIndex(x => x.Baslik);

            modelBuilder.Entity<Unite>()
                .HasIndex(x => new { x.ProgramlamaDiliId, x.Sira });
        }
    }
}
