namespace LogicfyApi.Models
{
    public class DersTakip : BaseEntity
    {
        public int DersId { get; set; }
        public int TakipEdenKullaniciSayisi { get; set; }

        public Ders Ders { get; set; }
    }
    public class UniteTakip : BaseEntity
    {
        public int UniteId { get; set; }
        public int TakipEdenKullaniciSayisi { get; set; }

        public Unite Unite { get; set; }
    }

}
