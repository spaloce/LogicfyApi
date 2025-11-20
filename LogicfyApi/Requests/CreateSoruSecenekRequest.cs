namespace LogicfyApi.Requests
{
    public class CreateSoruSecenekRequest
    {
        public int SoruId { get; set; }
        public string SecenekMetni { get; set; }
        public bool IsDogruCevap { get; set; }
    }
}
