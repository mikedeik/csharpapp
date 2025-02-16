using CSharpApp.Core.Dtos.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories.Commands {
    public record UpdateCategoryCommand(int CategoryId, CategoryMutateDto UpdatedCategory) : IRequest;

}
