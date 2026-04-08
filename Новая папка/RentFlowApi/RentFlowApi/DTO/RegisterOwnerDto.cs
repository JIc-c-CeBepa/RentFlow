namespace RentFlowApi.DTO
{
    public class RegisterOwnerDto
    {
        public string Phone { get; set; } = null;
        public string Password { get; set; } = null;
        
        public string CompanyName { get; set; } = null;
        public string Telegram { get; set; } = null;
        public string Email { get; set; } = null;
        public string Description { get; set; } = null;
        public string FirstName { get; set; }
    }
}
