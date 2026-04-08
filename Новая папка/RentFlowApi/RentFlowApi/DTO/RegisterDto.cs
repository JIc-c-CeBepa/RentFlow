using RentFlowApi.Model;

namespace RentFlowApi.DTO
{
    public class RegisterUserDto
    {
        public string Phone { get; set; } = null;
        public string Password { get; set; } = null;
        
        public int LeadSourceId { get; set; } = 1;

        public string FirstName { get; set; }

    }
}
