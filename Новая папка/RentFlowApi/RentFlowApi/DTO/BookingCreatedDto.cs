namespace RentFlowApi.DTO
{
    public class BookingCreatedDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string? PropertyTitle { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public DateOnly DepartureDate { get; set; }
        public int NightsCount { get; set; }
        public int GuestsCount { get; set; }
        public string? Status { get; set; }
        public int StatusId { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PrepaymentPercent { get; set; }
        public decimal PrepaymentAmount { get; set; }
        public bool NeedsContactlessCheckin { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
