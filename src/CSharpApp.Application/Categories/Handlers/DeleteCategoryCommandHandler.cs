using CSharpApp.Application.Categories.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Handlers {
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand> {

        private readonly ICategoriesService _categoriesService;

        public DeleteCategoryCommandHandler(ICategoriesService categoriesService) {
            _categoriesService = categoriesService;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken) {

            await _categoriesService.DeleteCategoryAsync(request.categoryId);
            
        }
    }
}
