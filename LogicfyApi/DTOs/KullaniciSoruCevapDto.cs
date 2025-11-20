namespace LogicfyApi.DTOs
{
    public class KullaniciSoruCevapDto
    {
        public int Id { get; set; }
        public int SoruId { get; set; }
        public bool DogruMu { get; set; }
        public string CevapJson { get; set; }
        public int SureMs { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
