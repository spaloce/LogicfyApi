namespace LogicfyApi.DTOs
{
    public class KullaniciDersIlerlemeDto
    {
        public int Id { get; set; }
        public int DersId { get; set; }
        public int TamamlananSoruSayisi { get; set; }
        public int ToplamSoruSayisi { get; set; }
        public int IlerlemeOrani { get; set; }
        public bool TamamlandiMi { get; set; }
    }
}
