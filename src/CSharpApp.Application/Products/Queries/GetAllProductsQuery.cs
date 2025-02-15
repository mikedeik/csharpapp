using CSharpApp.Core.Dtos.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Products.Queries {
    public record GetAllProductsQuery(): IRequest<IReadOnlyCollection<Product>>;
}
