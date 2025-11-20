namespace LogicfyApi.Requests
{
    public class CreateSoruCevapRequest
    {
        public int SoruId { get; set; }
        public bool DogruMu { get; set; }
        public string CevapJson { get; set; }
        public int SureMs { get; set; }
    }
}
