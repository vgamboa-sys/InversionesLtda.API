namespace ApiRest.Dto.Request
{
    public class ResetPasswordRequestDto
    {
        public string UsernameOrEmail { get; set; }
        public string NewPassword { get; set; }
    }
}
