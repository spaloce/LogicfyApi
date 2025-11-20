using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace LogicfyApi.Models
{
    public class Kullanici : IdentityUser
    {
        public string AdSoyad { get; set; }
        public string Rol { get; set; } = "User"; // User, Admin, vs.

        // Gamification
        public int XP { get; set; } = 0;
        public int Seviye { get; set; } = 1;
        public int Streak { get; set; } = 0;

        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }

        // Progress tabloları
        public List<KullaniciDersIlerleme> DersIlerlemeleri { get; set; }
        public List<KullaniciSoruCevap> SoruCevaplari { get; set; }
        public List<KullaniciXpLog> XpLoglari { get; set; }
        public ICollection<KullaniciDersKaydi> DersKayitlari { get; set; }

    }

    public class KullaniciDersKaydi : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int DersId { get; set; }
        public bool AktifMi { get; set; }

        public Kullanici Kullanici { get; set; }
        public Ders Ders { get; set; }
    }

    public class KullaniciDersIlerleme : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int DersId { get; set; }
        public int TamamlananSoruSayisi { get; set; }
        public int ToplamSoruSayisi { get; set; }
        public int IlerlemeOrani { get; set; }
        public bool TamamlandiMi { get; set; }

        public Kullanici Kullanici { get; set; }
        public Ders Ders { get; set; }
    }

    public class KullaniciUnitProgress : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int UniteId { get; set; }
        public int TamamlananDersSayisi { get; set; }
        public int ToplamDersSayisi { get; set; }
        public int IlerlemeOrani { get; set; }

        public Kullanici Kullanici { get; set; }
        public Unite Unite { get; set; }
    }

    public class KullaniciKisimProgress : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int KisimId { get; set; }
        public int TamamlananDersSayisi { get; set; }
        public int ToplamDersSayisi { get; set; }
        public int IlerlemeOrani { get; set; }

        public Kullanici Kullanici { get; set; }
        public Kisim Kisim { get; set; }
    }

    public class KullaniciSoruCevap : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int SoruId { get; set; }
        public bool DogruMu { get; set; }
        public string CevapJson { get; set; }
        public int SureMs { get; set; }

        public Kullanici Kullanici { get; set; }
        public Soru Soru { get; set; }
    }

    public class KullaniciXpLog : BaseEntity
    {
        public string KullaniciId { get; set; }
        public string Kaynak { get; set; }
        public int Xp { get; set; }

        public Kullanici Kullanici { get; set; }
    }

    public class KullaniciGunlukSeri : BaseEntity
    {
        public string KullaniciId { get; set; }
        public int SeriSayisi { get; set; }
        public DateTime SonGiris { get; set; }

        public Kullanici Kullanici { get; set; }
    }
}
