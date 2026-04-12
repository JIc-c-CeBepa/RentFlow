namespace RentFlowApi.DTO
{
    public class PropertyDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public int MaxGuests { get; set; }
        public bool IsContactlessCheckInAvailable { get; set; }
        public string? BookingMode { get; set; }
        public string? MainImageUrl { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
        public List<AmenityDto> Amenities { get; set; } = new();
        public List<RuleDto> Rules { get; set; } = new();
    }
}
