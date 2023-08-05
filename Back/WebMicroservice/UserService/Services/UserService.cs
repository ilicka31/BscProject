using AutoMapper;
using Common.Exceptions;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.DTOs;
using UserService.Models;
using UserService.Repository;
using UserService.Services.EmailService;


namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IConfigurationSection _secretKey;
        private readonly IConfigurationSection _googleClientId;

        public UserService(IMapper mapper, IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _secretKey = configuration.GetSection("SecretKey");
            _emailService = emailService;
            _googleClientId = configuration.GetSection("GoogleClientId");
        }

        public async Task<UserDTO> ActivateUser(long id, bool activate)
        {
            var user = await _userRepository.GetById(id);

            if (user == null) { throw new NotFoundException("User doesn't exist."); }
            if (activate)
            {
                user.Approved = true;

                var emailAddress = user.Email;
                var message = new Message(new string[] { $"{emailAddress}" }, "Profile activation", "Your profile is now active! WebShop App.");
                await _emailService.SendEmail(message);
            }
            else
            {
                user.Denied = true;
                var emailAddress = user.Email;
                var message = new Message(new string[] { $"{emailAddress}" }, "Profile activation", "Your profile activation has been rejected. WebShop App.");
                await _emailService.SendEmail(message);
            }

            _userRepository.Update(user);
            
            return _mapper.Map<UserDTO>(user);
        }

        public Task ChangePassword(long id, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDTO> ExternalLogin(string token)
        {

            ExternalUserDTO externalUser = await VerifyGoogleToken(token);
            if (externalUser == null) { throw new ConflictException("Invalid user google token."); }

            var us = await _userRepository.GetAll();
            List<Models.User> users = us.ToList();
            Models.User user = users.Find(u => u.Email.Equals(externalUser.Email));

            if (user == null)
            {
                user = new Models.User()
                {
                    Name = externalUser.Name,
                    Username = externalUser.UserName,
                    Email = externalUser.Email,
                    ProfilePictureUrl = new byte[0],
                    Password = "",
                    Address = "",
                    BirthDate = DateTime.Now,
                    Type = UserType.BUYER,
                    Approved = true
                };

                await _userRepository.Create(user);

            }
            List<Claim> userClaims = new List<Claim>();

            userClaims.Add(new Claim(ClaimTypes.Role, "BUYER"));

            userClaims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
            var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:44304",
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signinCredentials
                );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDTO()
            {
                Token = tokenString,
                Role = user.Type.ToString(),
                UserId = user.Id
            };
        }

        public async Task<List<UserDTO>> GetAllUnactivatedSellers()
        {
            var us = await _userRepository.GetAll();
            List<Models.User> list = us.ToList();  
            return _mapper.Map<List<UserDTO>>(list.Where(u => u.Approved == false && u.Type == UserType.SELLER && u.Denied == false).ToList());

        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            return _mapper.Map<List<UserDTO>>(await _userRepository.GetAll());
        }

        public async Task<List<UserDTO>> GetSellers()
        {
            var us = await _userRepository.GetAll();
            List<Models.User> list = us.ToList(); 
            return _mapper.Map<List<UserDTO>>(list.Where(u => u.Type == UserType.SELLER).ToList());

        }

        public async Task<UserDTO> GetUser(long id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) { throw new NotFoundException("User doesn't exist."); }
            return _mapper.Map<UserDTO>(user);
        }

        public long GetUserIdFromToken(ClaimsPrincipal user)
        {
            long id;
            long.TryParse(user.Identity.Name, out id);
            return id;
        }

        public async Task<UserImageDTO> GetUserImage(long id)
        {
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException("User doesn't exist.");

            byte[] imageBytes = await _userRepository.GetUserImage(user.Id);

            UserImageDTO userImage = new UserImageDTO()
            {
                ImageBytes = imageBytes
            };
            return userImage;
        }

        public async Task<TokenDTO> Login(UserLoginDTO user)
        {
            var us = await _userRepository.GetAll();
            List<Models.User> list = us.ToList();
            Models.User user1 = list.Find(u => u.Email == user.Email);

            if (user1 == null) { throw new NotFoundException($"User with Email: {user.Email} doesn't exist."); }
            
            if (BCrypt.Net.BCrypt.Verify(user.Password, user1.Password))
            {


                  List<Claim> userClaims = new List<Claim>();

                if (user1.Type.ToString() == "ADMIN")
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, "ADMIN"));
                }
                if (user1.Type.ToString() == "SELLER")
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, "SELLER"));
                }
                if (user1.Type.ToString() == "BUYER")
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, "BUYER"));
                }
                userClaims.Add(new Claim(ClaimTypes.Name, user1.Id.ToString()));

                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
                var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:44304",
                    claims: userClaims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: signinCredentials
                    );
                string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                  return new TokenDTO()
                {
                    Token =tokenString,
                    Role = user1.Type.ToString(),
                    UserId = user1.Id
                };
            }

            else throw new ConflictException("Password doesn't match.");

        }

        public async Task<UserDTO> Register(UserRegisterDTO user)
        {
            var us = await _userRepository.GetAll();
            List<Models.User> list = us.ToList();
            Models.User user1 = list.Find(u => u.Email == user.Email);
            if (user1 != null) { throw new ConflictException($"User with Email: {user.Email} already exists."); }

            Models.User newUser = _mapper.Map<Models.User>(user);
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            if (newUser.Type.ToString() == "SELLER")
            {
                newUser.Approved = false;
            }
            else
            {
                newUser.Approved = true;
            }
            newUser.ProfilePictureUrl = new byte[0];
            newUser.Denied = false;
            await _userRepository.Create(newUser);

            return _mapper.Map<UserDTO>(newUser);
        }

        public async Task<UserUpdateDTO> UpdateUser(long id, UserUpdateDTO user)
        {
            Models.User u = await _userRepository.GetById(id);
            if (u == null) { throw new NotFoundException("User doesn't exist."); }

            var password = u.Password;
            var type = u.Type;
            var profilePicUrl = u.ProfilePictureUrl;
            var approved = u.Approved;
            var us = await _userRepository.GetAll();
            List<Models.User> list = us.ToList();
            Models.User user1 = list.Find(u => u.Email == user.Email && u.Id != id);
            if (user1 != null) { throw new ConflictException($"User with Email: {user.Email} already exists."); }

            u = _mapper.Map<Models.User>(user);
            u.Password = password;
            u.Type = type;
            u.ProfilePictureUrl = profilePicUrl;
            u.Id = id;
            u.Approved = approved;
            _userRepository.Update(u);
          
            return _mapper.Map<UserUpdateDTO>(u);
        }

        public async Task UploadImage(long id, IFormFile file)
        {
            var user = await _userRepository.GetById(id) ?? throw new NotFoundException("User doesn't exist.");

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();

                user.ProfilePictureUrl = fileBytes;
                _userRepository.Update(user);
            }
            
        }
        private async Task<ExternalUserDTO> VerifyGoogleToken(string externalLoginToken)
        {
            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _googleClientId.Value }
                };

                var googleUserInfo = await GoogleJsonWebSignature.ValidateAsync(externalLoginToken, validationSettings);

                ExternalUserDTO externalUser = new ExternalUserDTO()
                {
                    UserName = googleUserInfo.Email.Split("@")[0],
                    Name = googleUserInfo.Name,
                    Email = googleUserInfo.Email
                };

                return externalUser;
            }
            catch
            {
                return null;
            }
        }
    }
  
}
