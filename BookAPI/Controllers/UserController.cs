using BookAPI.Helpers;
using BookAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookDetailContext _authContext;

        public UserController(BookDetailContext bookDetailContext)

        {
             _authContext = bookDetailContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if(userObj == null)
            
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x=>x.Username == userObj.Username);

            if (user == null)
                return NotFound(new { Message = "User Not Found" });


            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new {Message = "Password Is Incorrect"});
            }
            user.Token = CreateJwtToken(user);


            return Ok(new
            {
                Token = user.Token,
                Message = "Login Success!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            //Kolla användarnamn
            if (await CheckUsernameExistAsync(userObj.Username))
                return BadRequest(new {Message = "Username already exists"});

            //Kolla Email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email already exists" });

            //Kolla lösenstyrka
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new {Message =  pass});

            userObj.Password = PasswordHasher.HashPassword(userObj.Password); //Krypterar lösenord
            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync(); //save to db
            return Ok(new
            {
                Status = 200,
                Message = "User Registered!"
            }) ;
        }

        private async Task<bool> CheckUsernameExistAsync(string username)
         => await _authContext.Users.AnyAsync(x => x.Username == username);

        private async Task<bool> CheckEmailExistAsync(string email)
         => await _authContext.Users.AnyAsync(x => x.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum password length should be 8"+Environment.NewLine);
           // if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
           //     && Regex.IsMatch(password, "[0-9]")))
           //     sb.Append("Password should Alphanumeric"+ Environment.NewLine);
          // if(!Regex.IsMatch(password, "[<,>,@,!,#,¤,%,&,/,(,),=,+,`,´,{,},£]"))
          //     sb.Append("Password should contain special characters");

           return sb.ToString();
        }

        private string CreateJwtToken(User user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("VERYVERYsecret1234...");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.Username}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };
            var token = JwtTokenHandler.CreateToken(TokenDescriptor);
            return JwtTokenHandler.WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }
    }
}
