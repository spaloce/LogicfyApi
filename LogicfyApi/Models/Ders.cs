namespace LogicfyApi.Models
{
    public class Ders : BaseEntity
    {
        public int KisimId { get; set; }
        public string Baslik { get; set; }
        public int Sira { get; set; }
        public int TahminiSure { get; set; }
        public int SoruSayisiCache { get; set; }
        public int ZorlukSeviyesi { get; set; }

        public Kisim Kisim { get; set; }
        public ICollection<Soru> Sorular { get; set; }
        public ICollection<KullaniciDersKaydi> DersKayitlari { get; set; }
    }
}
