namespace LogicfyApi.Requests
{
    public class CreateSoruKelimeBlokRequest
    {
        public int SoruId { get; set; }
        public string DogruKod { get; set; }
        public string KelimelerJson { get; set; }
    }
}
