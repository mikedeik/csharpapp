using CSharpApp.Application.Categories.Commands;
using CSharpApp.Core.Dtos.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Category> {
        private readonly ICategoriesService _categoriesService;

        public CreateCategoryHandler(ICategoriesService categoriesService) {
            _categoriesService = categoriesService;
        }

        public async Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken) {
            var category = await _categoriesService.CreateCategoryAsync(request.Dto);
            return category;
        }
    }
}
