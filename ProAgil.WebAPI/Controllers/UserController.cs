﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Proagil.WebAPI.Dtos;
using ProAgil.Domain.Identity;

namespace Proagil.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém usuário
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpGet("get-user")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var user = await _userManager.FindByNameAsync("inoccard");
                var userToReturn = _mapper.Map<UserDto>(user);
                userToReturn.PasswordHash = null;
                return Ok(userToReturn);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível obter usuário: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Login
        /// Não pode retornar BadRequest
        /// Não necessita de autenticação [AllowAnonymous]
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userLogin.UserName);
                var result = await _userManager.CheckPasswordAsync(user, userLogin.PasswordHash);

                if (result)
                {
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);
                    userToReturn.PasswordHash = null;
                    return Ok(new
                    {
                        token = GenerateJwToken(appUser).Result,
                        user = userToReturn
                    });
                }

                return Unauthorized("Não foi possível fazer login");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível fazer login: {e.Message}");
            }
        }

        /// <summary>
        /// Cria novo usuário
        /// Não necessita de autenticação [AllowAnonymous]
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                var userToResult = _mapper.Map<UserDto>(user);

                if (result.Succeeded){
                    //userToResult.PasswordHash = null;
                    return Created("user", userToResult);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Não foi possível criar usuário: {e.Message}");
            }
        }

        #region private Methods
        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwToken(User user)
        {
            var claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}