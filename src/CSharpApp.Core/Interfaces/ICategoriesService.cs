using CSharpApp.Core.Dtos.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Interfaces {
    public interface ICategoriesService {
        
        Task<IReadOnlyCollection<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<Category> CreateCategory(CategoryMutateDto newCategory);
        Task UpdateCategoryAsync(int categoryId, CategoryMutateDto updatedCategory);
    }
}
