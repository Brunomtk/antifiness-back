namespace Core.DTO.User
{
    public class TokenJWT
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
