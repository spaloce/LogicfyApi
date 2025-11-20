namespace LogicfyApi.Models
{
    public class DersPerformansAnalitik : BaseEntity
    {
        public int DersId { get; set; }
        public double OrtalamaTamamlamaSuresi { get; set; }
        public double OrtalamaDogruOrani { get; set; }
        public int EnZorSoruId { get; set; }

        public Ders Ders { get; set; }
    }

    public class SoruAnalitik : BaseEntity
    {
        public int SoruId { get; set; }
        public int CevaplanmaSayisi { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public double OrtalamaSure { get; set; }

        public Soru Soru { get; set; }
    }

}
