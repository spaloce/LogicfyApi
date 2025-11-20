namespace LogicfyApi.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string AdSoyad { get; set; }
        public string Rol { get; set; }
        public int XP { get; set; }
        public int Seviye { get; set; }
        public int Streak { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public List<KullaniciDersKaydiDto> DersKayitlari { get; set; }
    }
}
