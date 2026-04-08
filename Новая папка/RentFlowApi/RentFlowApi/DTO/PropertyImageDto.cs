namespace RentFlowApi.DTO
{
    public class PropertyImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }
}
