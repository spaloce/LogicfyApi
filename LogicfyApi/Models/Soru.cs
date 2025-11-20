namespace LogicfyApi.Models
{
    public class Soru : BaseEntity
    {
        public int DersId { get; set; }
        public int SoruTipi { get; set; }   // 1-4
        public string SoruMetni { get; set; }
        public string KodMetni { get; set; }
        public int Seviye { get; set; }
        public int? DogruCevapId { get; set; }
        public string EkVeriJson { get; set; }

        // Navigation Properties
        public Ders Ders { get; set; }
        public SoruSecenek DogruCevap { get; set; }
        public List<SoruSecenek> Secenekler { get; set; } = new();
        public List<SoruKelimeBlok> KelimeBloklar { get; set; } = new();
        public List<SoruFonksiyonCozum> FonksiyonCozumler { get; set; } = new();
        public List<SoruCanliPreview> CanliPreviews { get; set; } = new();
    }

    public class SoruSecenek : BaseEntity
    {
        public int SoruId { get; set; }
        public string SecenekMetni { get; set; }

        public Soru Soru { get; set; }
    }

    public class SoruKelimeBlok : BaseEntity
    {
        public int SoruId { get; set; }
        public string DogruKod { get; set; }
        public string KelimelerJson { get; set; }

        public Soru Soru { get; set; }
    }

    public class SoruFonksiyonCozum : BaseEntity
    {
        public int SoruId { get; set; }
        public string CozumKod { get; set; }

        public Soru Soru { get; set; }
    }

    public class SoruCanliPreview : BaseEntity
    {
        public int SoruId { get; set; }
        public string DogruHtml { get; set; }
        public string DogruCss { get; set; }
        public string GerekenEtiketlerJson { get; set; }
        public string GerekenStillerJson { get; set; }

        public Soru Soru { get; set; }
    }


}
