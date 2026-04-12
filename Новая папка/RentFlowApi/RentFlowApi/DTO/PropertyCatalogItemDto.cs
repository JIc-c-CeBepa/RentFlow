namespace RentFlowApi.DTO
{
    public class PropertyCatalogItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public int MaxGuests { get; set; }
        public bool IsContactlessCheckInAvailable { get; set; }
        public string? Description { get; set; }

        public string? BookingMode { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
