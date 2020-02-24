namespace IdentityServer.Api.DTO.Requests
{
    public class Register
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberVerified { get; set; }
    }
}