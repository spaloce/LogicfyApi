namespace LogicfyApi.Models
{
    public class KisimIcerik : BaseEntity
    {
        public int ProgramlamaDiliId { get; set; }
        public int UniteId { get; set; }
        public int KisimId { get; set; }

        public string Baslik { get; set; }
        public string AciklamaHtml { get; set; }
        public string OrnekKod { get; set; }
        public string EkstraJson { get; set; }

        public ProgramlamaDili ProgramlamaDili { get; set; }
        public Unite Unite { get; set; }
        public Kisim Kisim { get; set; }
    }

}
