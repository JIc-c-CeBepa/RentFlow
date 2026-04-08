namespace RentFlowApi.DTO
{
    public class RegManagerDTO
    {
        public string Phone { get; set; } = null;
        public string Password { get; set; } = null;
        

        public int OwnerId { get; set; } = 1;
        public string FirstName { get; set; }
    }
}
