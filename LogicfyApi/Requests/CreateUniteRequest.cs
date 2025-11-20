namespace LogicfyApi.Requests
{
    public class CreateUniteRequest
    {
        public int ProgramlamaDiliId { get; set; }
        public string Baslik { get; set; }
        public int Sira { get; set; }
        public string Aciklama { get; set; }
    }
}
