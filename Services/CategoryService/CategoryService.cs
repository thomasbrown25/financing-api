using Microsoft.EntityFrameworkCore;
using financing_api.Data;
using AutoMapper;
using financing_api.Utils;
using financing_api.PlaidInterface;
using financing_api.Dtos.Category;
using financing_api.DbLogger;

namespace financing_api.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IPlaidApi _plaidApi;
        private readonly ILogging _logging;

        public CategoryService(
            DataContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPlaidApi plaidApi,
            ILogging logging
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _plaidApi = plaidApi;
            _logging = logging;
        }

        public async Task<ServiceResponse<GetCategoryDto>> GetCategories()
        {
            var response = new ServiceResponse<GetCategoryDto>();

            try
            {
                response.Data = new GetCategoryDto();

                var dbCategories = await _context.Categories
                                   .OrderBy(c => c.Name)
                                   .ToListAsync();

                response.Data.Categories = dbCategories.Select(c => _mapper.Map<CategoryDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCategoryDto>> AddCategory(AddCategoryDto category)
        {
            var response = new ServiceResponse<GetCategoryDto>();

            try
            {
                Category newCategory = _mapper.Map<Category>(category);

                _context.Categories.Add(newCategory);

                await _context.SaveChangesAsync();

                var dbCategories = await _context.Categories
                                   .OrderBy(c => c.Name)
                                   .ToListAsync();

                response.Data = new GetCategoryDto
                {
                    Categories = dbCategories.Select(c => _mapper.Map<CategoryDto>(c)).ToList()
                };
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCategoryDto>> RefreshCategories()
        {
            var response = new ServiceResponse<GetCategoryDto>();

            try
            {
                response.Data = new GetCategoryDto();
                response.Data.Categories = new List<CategoryDto>();

                var user = Utilities.GetCurrentUser(_context, _httpContextAccessor);

                var result = await _plaidApi.GetTransactionsRequest(user);

                foreach (var transaction in result.Transactions)
                {
                    var category = transaction.Category?[0];
                    var dbCategory = await _context.Categories
                                        .FirstOrDefaultAsync(c => c.Name.ToLower() == category);

                    if (dbCategory is null)
                    {
                        var categoryDto = new CategoryDto();

                        categoryDto.Name = category;

                        response.Data.Categories.Add(categoryDto);
                        Category categoryDb = _mapper.Map<Category>(categoryDto);
                        _context.Categories.Add(categoryDb);
                        await _context.SaveChangesAsync();
                    }
                }

                var dbCategories = await _context.Categories
                                   .OrderBy(c => c.Name)
                                   .ToListAsync();

                response.Data.Categories = dbCategories.Select(c => _mapper.Map<CategoryDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                _logging.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
            return response;
        }
    }
}