namespace LogicfyApi.Requests
{
    public class UpdateSoruRequest
    {
        public string SoruMetni { get; set; }
        public string KodMetni { get; set; }
        public int Seviye { get; set; }
        public string EkVeriJson { get; set; }
    }
}
