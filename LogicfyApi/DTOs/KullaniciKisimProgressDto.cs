namespace LogicfyApi.DTOs
{
    public class KullaniciKisimProgressDto
    {
        public int Id { get; set; }
        public int KisimId { get; set; }
        public int TamamlananDersSayisi { get; set; }
        public int ToplamDersSayisi { get; set; }
        public int IlerlemeOrani { get; set; }
    }
}
