
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelPlanningApi.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelPlanningApi.Requests;
using TravelPlanningApi.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TravelPlanningApi.Controllers;

public class AccountController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailValidator _emailValidator;
    private readonly PasswordHashValidator _passwordHashValidator; 
    private IConfiguration Configuration { get; }
    private readonly IUserValidationService _userValidationService;
    
    
    public AccountController(ApplicationDbContext context, EmailValidator emailValidator, 
							 PasswordHashValidator passwordHashValidator,
							 IConfiguration configuration, 
							 IUserValidationService userValidationService)  
    {
        _context = context;
        _emailValidator = emailValidator;
        _passwordHashValidator = passwordHashValidator;
        Configuration = configuration;
        _userValidationService = userValidationService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
            #region Declaration(s)
	        var user = await _context.Usuarios
									.Where(e => e.Email == request.Email)
									.FirstOrDefaultAsync();
	        
	        List<string> errors = new();
	        ValidationResult emailResult = _emailValidator.Validate(request.Email);
	        bool passwordValidator = _passwordHashValidator.ValidatePassword(user!.Password, request.Password);
	        #endregion

	        #region Validation(s)
	        if (!emailResult.IsValid)
	        {
		        foreach (var error in emailResult.Errors)
		        {
			        errors.Add(error.ErrorMessage);
		        }
	        }
	        
	        if (!passwordValidator)
	        {
		        errors.Add("Credenciales inválidas.");
	        }

	        #endregion

	        if (errors.Count == 0)
	        {
		        bool isAdmin = await _userValidationService.IsUserAdminAsync(user!.Id);
	
		        var issuer = Configuration["Jwt:Issuer"];
		        var audience = Configuration["Jwt:Audience"];
		        var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"] ?? string.Empty);
		        
		        var claims = new List<Claim>()
		        {
			        new Claim("Id", user.Id.ToString()),
			        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
			        new Claim(JwtRegisteredClaimNames.Email, user.Email),
			        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		        };
        
		        if (isAdmin)
			        claims.Add(new Claim(ClaimTypes.Role, "admin"));
		        
		        var tokenDescriptor = new SecurityTokenDescriptor
		        {
			        Subject = new ClaimsIdentity(claims),
			        Expires = DateTime.UtcNow.AddHours(5),
			        Issuer = issuer,
			        Audience = audience,
			        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
		        };
        
		        var tokenHandler = new JwtSecurityTokenHandler();
		        var token = tokenHandler.CreateToken(tokenDescriptor);
		        var stringToken = tokenHandler.WriteToken(token);

		        return Ok(stringToken);
	        }
        

	        return BadRequest(new { errors = errors });
    }
}