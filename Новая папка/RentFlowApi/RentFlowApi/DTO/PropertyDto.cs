namespace RentFlowApi.DTO
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? MainImageUrl { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
    }
}
