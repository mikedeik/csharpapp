using CSharpApp.Application.Categories.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand> {
        private readonly ICategoriesService _categoriesService;

        public UpdateCategoryCommandHandler(ICategoriesService categoriesService) {
            _categoriesService = categoriesService;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken) {
            await _categoriesService.UpdateCategoryAsync(request.CategoryId, request.UpdatedCategory);
        }
    }

}
