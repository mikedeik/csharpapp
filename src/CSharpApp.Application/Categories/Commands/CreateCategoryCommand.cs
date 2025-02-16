using CSharpApp.Core.Dtos.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Commands {
    public class CreateCategoryCommand : IRequest<Category> {
        public CategoryMutateDto Dto { get; }

        public CreateCategoryCommand(CategoryMutateDto dto) {
            Dto = dto;
        }
    }

}
