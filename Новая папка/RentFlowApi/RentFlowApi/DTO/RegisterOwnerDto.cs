namespace RentFlowApi.DTO
{
    public class RegisterOwnerDto
    {
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }

        public string? Telegram { get; set; }
    }
}
