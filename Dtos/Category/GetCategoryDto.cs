using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace financing_api.Dtos.Category
{
    public class GetCategoryDto
    {
        public int Id { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}