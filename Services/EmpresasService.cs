using Core.DTO;
using Core.DTO.Empresa;
using Core.Models;
using Core.Enums;
using Infrastructure.Repositories;

namespace Services
{
    public interface IEmpresasService
    {
        Task<EmpresaResponse?> CreateEmpresaAsync(CreateEmpresaRequest request);
        Task<EmpresaResponse?> UpdateEmpresaAsync(int id, UpdateEmpresaRequest request);
        Task<bool> DeleteEmpresaAsync(int id);
        Task<EmpresaResponse?> ToggleEmpresaStatusAsync(int id);
        Task<EmpresaResponse?> GetEmpresaByIdAsync(int id);
        Task<Core.DTO.Empresa.EmpresasPagedDTO> GetEmpresasPagedAsync(EmpresaFiltersDTO filters);
        Task<EmpresaStats> GetEmpresaStatsAsync();
    }

    public class EmpresasService : IEmpresasService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmpresasService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EmpresaResponse?> CreateEmpresaAsync(CreateEmpresaRequest request)
        {
            // Verificar se email já existe
            var allEmpresas = await _unitOfWork.Empresas.GetAllAsync();
            var existingByEmail = allEmpresas.FirstOrDefault(e => e.Email == request.Email);
            if (existingByEmail != null)
                return null;

            // Verificar se CNPJ já existe (se fornecido)
            if (!string.IsNullOrEmpty(request.CNPJ))
            {
                var existingByCNPJ = allEmpresas.FirstOrDefault(e => e.CNPJ == request.CNPJ);
                if (existingByCNPJ != null)
                    return null;
            }

            // Verificar se o usuário existe
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                return null;

            var empresa = new Empresas
            {
                Name = request.Name,
                Email = request.Email,
                CNPJ = request.CNPJ,
                Status = request.Status,
                UserId = request.UserId,
                User = user,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _unitOfWork.Empresas.Add(empresa);
            var changes = _unitOfWork.Save();

            if (changes <= 0)
                return null;

            return await GetEmpresaByIdAsync(empresa.Id);
        }

        public async Task<EmpresaResponse?> UpdateEmpresaAsync(int id, UpdateEmpresaRequest request)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);
            if (empresa == null)
                return null;

            // Verificar se email já existe (exceto para a empresa atual)
            if (!string.IsNullOrEmpty(request.Email) && request.Email != empresa.Email)
            {
                var allEmpresas = await _unitOfWork.Empresas.GetAllAsync();
                var existingByEmail = allEmpresas.FirstOrDefault(e => e.Email == request.Email && e.Id != id);
                if (existingByEmail != null)
                    return null;
            }

            // Verificar se CNPJ já existe (exceto para a empresa atual)
            if (!string.IsNullOrEmpty(request.CNPJ) && request.CNPJ != empresa.CNPJ)
            {
                var allEmpresas = await _unitOfWork.Empresas.GetAllAsync();
                var existingByCNPJ = allEmpresas.FirstOrDefault(e => e.CNPJ == request.CNPJ && e.Id != id);
                if (existingByCNPJ != null)
                    return null;
            }

            // Verificar se o novo usuário existe (se fornecido)
            if (request.UserId.HasValue && request.UserId.Value != empresa.UserId)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId.Value);
                if (user == null)
                    return null;
                empresa.User = user;
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(request.Name))
                empresa.Name = request.Name;
            
            if (!string.IsNullOrEmpty(request.Email))
                empresa.Email = request.Email;
            
            if (request.CNPJ != null)
                empresa.CNPJ = request.CNPJ;
            
            if (request.Status.HasValue)
                empresa.Status = request.Status.Value;
            
            if (request.UserId.HasValue)
                empresa.UserId = request.UserId.Value;

            empresa.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Empresas.Update(empresa);
            var changes = _unitOfWork.Save();

            if (changes <= 0)
                return null;

            return await GetEmpresaByIdAsync(id);
        }

        public async Task<bool> DeleteEmpresaAsync(int id)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);
            if (empresa == null)
                return false;

            _unitOfWork.Empresas.Delete(empresa);
            var changes = _unitOfWork.Save();
            return changes > 0;
        }

        public async Task<EmpresaResponse?> ToggleEmpresaStatusAsync(int id)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);
            if (empresa == null)
                return null;

            empresa.Status = empresa.Status == EmpresaStatus.Active 
                ? EmpresaStatus.Inactive 
                : EmpresaStatus.Active;
            
            empresa.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Empresas.Update(empresa);
            var changes = _unitOfWork.Save();

            if (changes <= 0)
                return null;

            return await GetEmpresaByIdAsync(id);
        }

        public async Task<EmpresaResponse?> GetEmpresaByIdAsync(int id)
        {
            var empresa = await _unitOfWork.Empresas.GetByIdAsync(id);
            if (empresa == null)
                return null;

            // Carregar o usuário se não estiver carregado
            if (empresa.User == null)
            {
                empresa.User = await _unitOfWork.Users.GetByIdAsync(empresa.UserId);
            }

            return new EmpresaResponse
            {
                Id = empresa.Id,
                Name = empresa.Name,
                Email = empresa.Email,
                CNPJ = empresa.CNPJ,
                Status = empresa.Status,
                UserId = empresa.UserId,
                UserName = empresa.User?.Name ?? "",
                UserEmail = empresa.User?.Email ?? "",
                CreatedDate = empresa.CreatedDate,
                UpdatedDate = empresa.UpdatedDate
            };
        }

        public async Task<Core.DTO.Empresa.EmpresasPagedDTO> GetEmpresasPagedAsync(EmpresaFiltersDTO filters)
        {
            // Converter filtros para o formato esperado pelo repositório
            var repoFilters = new FiltersDTO
            {
                Name = filters.Search ?? filters.Name,
                pageNumber = filters.Page,
                pageSize = filters.PageSize
            };

            var paged = await _unitOfWork.Empresas.GetAllPagedAsync(repoFilters);
            var results = paged.Results.AsQueryable();

            // Aplicar filtro de status se fornecido
            if (filters.Status.HasValue)
            {
                results = results.Where(e => e.Status == filters.Status.Value);
            }

            // Aplicar busca se fornecida
            if (!string.IsNullOrEmpty(filters.Search))
            {
                results = results.Where(e => 
                    e.Name.Contains(filters.Search) ||
                    e.Email.Contains(filters.Search) ||
                    (e.CNPJ != null && e.CNPJ.Contains(filters.Search)));
            }

            // Aplicar ordenação
            results = ApplySorting(results, filters.SortBy, filters.SortDirection);

            var filteredResults = results.ToList();
            var totalCount = filteredResults.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / filters.PageSize);

            // Aplicar paginação
            var pagedResults = filteredResults
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            // Carregar dados dos usuários para cada empresa
            var empresaResponses = new List<EmpresaResponse>();
            foreach (var empresa in pagedResults)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(empresa.UserId);
                empresaResponses.Add(new EmpresaResponse
                {
                    Id = empresa.Id,
                    Name = empresa.Name,
                    Email = empresa.Email,
                    CNPJ = empresa.CNPJ,
                    Status = empresa.Status,
                    UserId = empresa.UserId,
                    UserName = user?.Name ?? "",
                    UserEmail = user?.Email ?? "",
                    CreatedDate = empresa.CreatedDate,
                    UpdatedDate = empresa.UpdatedDate
                });
            }

            return new Core.DTO.Empresa.EmpresasPagedDTO
            {
                CurrentPage = filters.Page,
                PageSize = filters.PageSize,
                RowCount = totalCount,
                PageCount = totalPages,
                Result = empresaResponses
            };
        }

        public async Task<EmpresaStats> GetEmpresaStatsAsync()
        {
            var empresas = await _unitOfWork.Empresas.GetAllAsync();
            var empresasList = empresas.ToList();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            var totalEmpresas = empresasList.Count;
            var empresasAtivas = empresasList.Count(e => e.Status == EmpresaStatus.Active);
            var empresasInativas = empresasList.Count(e => e.Status == EmpresaStatus.Inactive);
            var novasEmpresasEstesMes = empresasList.Count(e => e.CreatedDate >= startOfMonth);
            var empresasComCNPJ = empresasList.Count(e => !string.IsNullOrEmpty(e.CNPJ));
            var novasEmpresasMesPassado = empresasList.Count(e => 
                e.CreatedDate >= startOfLastMonth && e.CreatedDate < startOfMonth);

            var percentualCrescimento = novasEmpresasMesPassado > 0 
                ? ((decimal)(novasEmpresasEstesMes - novasEmpresasMesPassado) / novasEmpresasMesPassado) * 100
                : novasEmpresasEstesMes > 0 ? 100 : 0;

            var percentualAtivas = totalEmpresas > 0 ? ((decimal)empresasAtivas / totalEmpresas) * 100 : 0;
            var percentualInativas = totalEmpresas > 0 ? ((decimal)empresasInativas / totalEmpresas) * 100 : 0;

            return new EmpresaStats
            {
                TotalEmpresas = totalEmpresas,
                EmpresasAtivas = empresasAtivas,
                EmpresasInativas = empresasInativas,
                NovasEmpresasEstesMes = novasEmpresasEstesMes,
                EmpresasComCNPJ = empresasComCNPJ,
                PercentualCrescimento = Math.Round(percentualCrescimento, 2),
                PercentualAtivas = Math.Round(percentualAtivas, 2),
                PercentualInativas = Math.Round(percentualInativas, 2)
            };
        }

        private static IQueryable<Empresas> ApplySorting(IQueryable<Empresas> query, string? sortBy, string? sortDirection)
        {
            var isDescending = sortDirection?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "cnpj" => isDescending ? query.OrderByDescending(e => e.CNPJ) : query.OrderBy(e => e.CNPJ),
                "status" => isDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
                "createddate" => isDescending ? query.OrderByDescending(e => e.CreatedDate) : query.OrderBy(e => e.CreatedDate),
                "updateddate" => isDescending ? query.OrderByDescending(e => e.UpdatedDate) : query.OrderBy(e => e.UpdatedDate),
                _ => query.OrderBy(e => e.Name)
            };
        }
    }
}
