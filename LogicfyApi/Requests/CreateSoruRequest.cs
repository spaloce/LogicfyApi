namespace LogicfyApi.Requests
{
    public class CreateSoruRequest
    {
        public int DersId { get; set; }
        public int SoruTipi { get; set; }
        public string SoruMetni { get; set; }
        public string KodMetni { get; set; }
        public int Seviye { get; set; }
        public string EkVeriJson { get; set; }
    }
}
