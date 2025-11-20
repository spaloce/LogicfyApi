namespace LogicfyApi.Models
{
    public class Unite : BaseEntity
    {
        public int ProgramlamaDiliId { get; set; }
        public string Baslik { get; set; }
        public int Sira { get; set; }
        public string Aciklama { get; set; }
        public ProgramlamaDili ProgramlamaDili { get; set; }
        public ICollection<Kisim> Kisimlar { get; set; } = new List<Kisim>();
    }

}
