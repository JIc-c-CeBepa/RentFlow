namespace RentFlowApi.DTO
{
    public class UploadPropertyImageRequest
    {
        public IFormFile File { get; set; } = null!;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }
}
