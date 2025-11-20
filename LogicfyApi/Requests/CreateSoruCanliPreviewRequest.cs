namespace LogicfyApi.Requests
{
    public class CreateSoruCanliPreviewRequest
    {
        public int SoruId { get; set; }
        public string DogruHtml { get; set; }
        public string DogruCss { get; set; }
        public string GerekenEtiketlerJson { get; set; }
        public string GerekenStillerJson { get; set; }
    }
}
