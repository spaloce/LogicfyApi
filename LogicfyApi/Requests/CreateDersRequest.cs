namespace LogicfyApi.Requests
{
    public class CreateDersRequest
    {
        public int KisimId { get; set; }
        public string Baslik { get; set; }
        public int TahminiSure { get; set; }
        public int ZorlukSeviyesi { get; set; }
    }
}
