using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO.User;
using Core.Enums.User;
using Core.Models;
using Infrastructure.Authenticate;
using Infrastructure.Repositories;
using Infrastructure.Security;

namespace Services
{
    public interface IUserService
    {
        Task<TokenJWT> AuthenticateAsync(UserAuthenticateRequest request);
        Task<TokenJWT> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> CreateUserAsync(CreateUserRequest request);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync(string? role = null, string? status = null, string? search = null);
        Task<UserResponse?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int userId);
        Task<UserStats> GetUserStatsAsync();
    }

    public sealed class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IJWTManager _jwtManager;

        public UserService(IUnitOfWork unitOfWork, IJWTManager jwtManager)
        {
            _unitOfWork = unitOfWork;
            _userRepo = unitOfWork.Users;
            _jwtManager = jwtManager;
        }

        public async Task<TokenJWT> AuthenticateAsync(UserAuthenticateRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || Encrypt.EncryptPassword(request.Password) != user.Password)
                throw new UnauthorizedAccessException("Email ou senha inválidos.");

            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("Usuário inativo ou pendente.");

            var tokenDto = await _jwtManager.AuthenticateAsync(user, request.RememberMe);
            return tokenDto;
        }

        public async Task<TokenJWT> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ArgumentException("Refresh token é obrigatório.", nameof(request));

            var tokenDto = await _jwtManager.RefreshTokenAsync(request.RefreshToken);
            return tokenDto;
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request)
        {
            // Verificar se email já existe
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email já está em uso.");

            var user = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email.ToLower(),
                Password = Encrypt.EncryptPassword(request.Password),
                Type = request.Type,
                Status = request.Status,
                Phone = request.Phone,
                Avatar = request.Avatar,
                ClientId = request.ClientId,
                EmpresaId = request.EmpresaId
            };

            _userRepo.Add(user);
            var saved = await _unitOfWork.SaveAsync();
            return saved > 0;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(string? role = null, string? status = null, string? search = null)
        {
            var users = await _userRepo.GetAllAsync();
            var query = users.AsQueryable();

            // Filtro por role
            if (!string.IsNullOrWhiteSpace(role))
            {
                if (Enum.TryParse<UserType>(role, true, out var userType))
                {
                    query = query.Where(u => u.Type == userType);
                }
            }

            // Filtro por status
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<UserStatus>(status, true, out var userStatus))
                {
                    query = query.Where(u => u.Status == userStatus);
                }
            }

            // Filtro por busca
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Username.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return query.Select(MapToUserResponse).ToList();
        }

        public async Task<UserResponse?> GetUserByIdAsync(int userId)
        {
            if (userId <= 0) return null;
            
            var user = await _userRepo.GetByIdAsync(userId);
            return user != null ? MapToUserResponse(user) : null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            
            var user = await _userRepo.GetByEmailAsync(email);
            if (user != null)
                user.Password = string.Empty;
            return user;
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            // Verificar se email já existe (se está sendo alterado)
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                var existingUser = await _userRepo.GetByEmailAsync(request.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("Email já está em uso.");
            }

            // Atualizar campos
            if (!string.IsNullOrWhiteSpace(request.Name))
                user.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Username))
                user.Username = request.Username;
            if (!string.IsNullOrWhiteSpace(request.Email))
                user.Email = request.Email.ToLower();
            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = Encrypt.EncryptPassword(request.Password);
            if (request.Type.HasValue)
                user.Type = request.Type.Value;
            if (request.Status.HasValue)
                user.Status = request.Status.Value;
            if (request.Phone != null)
                user.Phone = request.Phone;
            if (request.Avatar != null)
                user.Avatar = request.Avatar;
            if (request.ClientId.HasValue)
                user.ClientId = request.ClientId.Value;
            if (request.EmpresaId.HasValue)
                user.EmpresaId = request.EmpresaId.Value;

            user.UpdatedDate = DateTime.UtcNow;

            _userRepo.Update(user);
            var saved = await _unitOfWork.SaveAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            _userRepo.Delete(user);
            var saved = await _unitOfWork.SaveAsync();
            return saved > 0;
        }

        public async Task<UserStats> GetUserStatsAsync()
        {
            var users = await _userRepo.GetAllAsync();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var stats = new UserStats
            {
                TotalUsers = users.Count,
                TotalAdmins = users.Count(u => u.Type == UserType.Admin),
                TotalClients = users.Count(u => u.Type == UserType.Client),
                ActiveUsers = users.Count(u => u.Status == UserStatus.Active),
                InactiveUsers = users.Count(u => u.Status == UserStatus.Inactive),
                PendingUsers = users.Count(u => u.Status == UserStatus.Pending),
                NewUsersThisMonth = users.Count(u => u.CreatedDate.Month == currentMonth && u.CreatedDate.Year == currentYear),
                VerifiedAdmins = users.Count(u => u.Type == UserType.Admin && u.Status == UserStatus.Active),
                ClientsWithNutritionist = 0, // Implementar conforme regra de negócio
                GrowthPercentage = CalculateGrowthPercentage(users)
            };

            return stats;
        }

        private static UserResponse MapToUserResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Role = user.Type.ToString().ToLower(),
                Status = user.Status.ToString().ToLower(),
                Phone = user.Phone,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedDate,
                UpdatedAt = user.UpdatedDate,
                Type = user.Type,
                StatusEnum = user.Status,
                ClientId = user.ClientId,
                EmpresaId = user.EmpresaId
            };
        }

        private static double CalculateGrowthPercentage(List<User> users)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var currentMonthUsers = users.Count(u => u.CreatedDate.Month == currentMonth && u.CreatedDate.Year == currentYear);
            var lastMonthUsers = users.Count(u => u.CreatedDate.Month == lastMonth && u.CreatedDate.Year == lastMonthYear);

            if (lastMonthUsers == 0) return currentMonthUsers > 0 ? 100 : 0;
            
            return Math.Round(((double)(currentMonthUsers - lastMonthUsers) / lastMonthUsers) * 100, 2);
        }
    }
}
