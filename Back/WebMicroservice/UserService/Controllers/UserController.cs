using Common.Entites;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Collections;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using UserService.Database;
using UserService.DTOs;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPublishEndpoint  _publishEndpoint;

        public UserController(IUserService userService, IPublishEndpoint publishEndpoint)
        {
            _userService = userService;
            _publishEndpoint = publishEndpoint;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerDTO)
        {
            UserDTO user = await _userService.Register(registerDTO);
            /* var jsonString = JsonSerializer.Serialize(new UserInfo { Email = user.Email, Id = _userService.GetUserIdFromToken(User), Name = user.Name, UserType = user.type.ToString(), Approved = user.Approved, Address = user.Address });
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
            //exchange: "fanout_exchange", routingKey: "", basicProperties: null, body: body
              _publishEndpoint.BasicPublish(exchange: "fanout_exchange",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: messageBytes);*/
            var pub = new UserInfo { Email = user.Email, Id = user.Id, Name = user.Name, UserType = user.type.ToString(), Approved = user.Approved, Address = user.Address };
           await _publishEndpoint.Publish<UserInfo>(pub);
         //   await _publishEndpoint.Publish<UserInfo>(pub);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
        {
            TokenDTO token = await _userService.Login(loginDTO);
           // await _publishEndpoint.Publish<TokenDTO>(token);
            return Ok(token);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromForm] string googleToken)
        {
            TokenDTO token = await _userService.ExternalLogin(googleToken);
            return Ok(token);
        }

        [HttpGet("user")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> GetUser()
        {
            UserDTO userDTO = await _userService.GetUser(_userService.GetUserIdFromToken(User));
            return Ok(userDTO);
        }

        [HttpGet("activated-sellers")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetActivatedSellers()
        {
            return Ok(await _userService.GetSellers());
        }

        [HttpGet("unactivated-sellers")]
        [Authorize(Roles = "ADMIN")]

        public async Task<IActionResult> GetUnactivatedSellers()
        {
            return Ok(await _userService.GetAllUnactivatedSellers());

        }
        //update
        [HttpPut("update-profile")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDTO user)
        {
            await _userService.UpdateUser(_userService.GetUserIdFromToken(User), user);
            UserDTO user1 = await _userService.GetUser(_userService.GetUserIdFromToken(User));
           /* var jsonString = JsonSerializer.Serialize(new UserInfo { Email = user.Email, Id = _userService.GetUserIdFromToken(User), Name = user.Name, UserType = user1.type.ToString(), Approved = user1.Approved, Address = user.Address });
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);

            _publishEndpoint.BasicPublish(exchange: "fanout_exchange",
                              routingKey: "",
                              basicProperties: null,
                              body: messageBytes);*/
            await _publishEndpoint.Publish<UserInfo>(new UserInfo { Email = user.Email, Id = _userService.GetUserIdFromToken(User), Name= user.Name, UserType = user1.type.ToString(), Approved = user1.Approved, Address = user.Address });
            return Ok();


        }
        //activate
        [HttpPut("activate-user")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ActivateUser([FromBody] UserActivateDTO user)
        {
           await _userService.ActivateUser(user.Id, user.IsActive);
           UserDTO user1 = await _userService.GetUser(user.Id);
          /*  var jsonString = JsonSerializer.Serialize(new UserInfo { Email = user1.Email, Id = _userService.GetUserIdFromToken(User), Name = user1.Name, UserType = user1.type.ToString(), Approved = user1.Approved, Address = user1.Address });
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);

            _publishEndpoint.BasicPublish(exchange: "fanout_exchange",
                              routingKey: "",
                              basicProperties: null,
                              body: messageBytes);*/
           await _publishEndpoint.Publish<UserInfo>(new UserInfo { Email = user1.Email, Id = user1.Id, Name = user1.Name, UserType = user1.type.ToString(), Approved = user1.Approved, Address = user1.Address });

            return Ok();
        }

        [HttpPut("upload-image")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                await _userService.UploadImage(_userService.GetUserIdFromToken(User), file);
                return Ok();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        [HttpGet("get-image")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> GetImage()
        {
            UserImageDTO userDTO = await _userService.GetUserImage(_userService.GetUserIdFromToken(User));
            return Ok(userDTO);
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> ChangePassword([FromForm] UserPasswordDTO userPasswordDTO)
        {
            await _userService.ChangePassword(_userService.GetUserIdFromToken(User), userPasswordDTO.OldPassword, userPasswordDTO.NewPassword);
           // await _publishEndpoint.Publish(new UserPasswordChanged(_userService.GetUserIdFromToken(User), userPasswordDTO.NewPassword));

            return Ok();
        }
    }
}
