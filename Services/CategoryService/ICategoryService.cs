using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using financing_api.Dtos.Category;

namespace financing_api.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<GetCategoryDto>> GetCategories();
        Task<ServiceResponse<GetCategoryDto>> AddCategory(AddCategoryDto category);
        Task<ServiceResponse<GetCategoryDto>> RefreshCategories();
    }
}