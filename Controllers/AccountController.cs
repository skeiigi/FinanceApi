using FinanceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CrmIntegrationService.Service1 _crmService = new CrmIntegrationService.Service1();

        public AccountController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            return Ok(new
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                try
                {
                    await _crmService.SendContactToCrm(
                        "Новый пользователь",
                        model.Email,
                        false,
                        "152",
                        0
                    );
                }
                catch (Exception)
                {

                    throw;
                }

                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("update")]
        [Authorize] // Только для залогиненных
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound("Пользователь не найден");

            // Обновляем поля
            user.Email = model.Email;
            user.UserName = model.Email; // Обычно UserName совпадает с почтой
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Профиль обновлен" });
            }

            return BadRequest(result.Errors);
        }
    }
}
