using CSharpApp.Core.Dtos.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Authentication.Commands {
    public record LoginCommand(LoginCredentials LoginCredentials): IRequest<AuthResponse>;
}
