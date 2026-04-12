namespace RentFlowApi.DTO
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string MainImageUrl { get; set; }
        public string? Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool IsContactlessCheckInAvailable { get; set; }
        public string? BookingMode { get; set; }
        public int? MaxGuests { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
    }
}
