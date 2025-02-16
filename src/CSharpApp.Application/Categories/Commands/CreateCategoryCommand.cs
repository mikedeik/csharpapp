using CSharpApp.Core.Dtos.Categories;
using CSharpApp.Core.Dtos.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Commands {
    public record CreateCategoryCommand(CategoryMutateDto MutateDto) : IRequest<Category>;

}
