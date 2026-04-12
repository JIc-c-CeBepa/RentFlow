namespace RentFlowApi.DTO
{
    public class PropertyCatalogFilterRequest
    {
        public string? OwnerSlug { get; set; }   
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? GuestsCount { get; set; }
        public bool? NeedContactlessCheckIn { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public DateOnly? DepartureDate { get; set; }
    }
}
