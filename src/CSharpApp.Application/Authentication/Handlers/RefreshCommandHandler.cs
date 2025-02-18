using CSharpApp.Application.Authentication.Commands;
using CSharpApp.Core.Dtos.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Authentication.Handlers {
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponse> {

        private readonly IAuthenticationService _authenticationService;

        public RefreshCommandHandler(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken token) {

            return await _authenticationService.Refresh(request.RefreshDto.RefreshToken);
        }

    }
}
