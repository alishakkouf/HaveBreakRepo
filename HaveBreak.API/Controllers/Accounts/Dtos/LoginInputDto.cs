using System.ComponentModel.DataAnnotations;

namespace HaveBreak.API.Controllers.Accounts.Dtos
{
    public class LoginInputDto
    {
        public required string UserName { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLength{2}")]
        public required string Password { get; set; }

        public void Normalize()
        {
            UserName = UserName.Trim();
        }
    }
}
