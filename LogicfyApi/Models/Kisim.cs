namespace LogicfyApi.Models
{
    public class Kisim : BaseEntity
    {
        public int UniteId { get; set; }
        public string Baslik { get; set; }
        public int Sira { get; set; }
        public int DersSayisiCache { get; set; }

        public Unite Unite { get; set; }
        public ICollection<Ders> Dersler { get; set; }
    }

}
