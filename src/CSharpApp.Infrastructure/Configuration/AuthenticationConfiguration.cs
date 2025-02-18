using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Infrastructure.Configuration {
    public static class AuthenticationConfiguration {

        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration) {

            var authSettings = configuration.GetSection(nameof(AuthenticationSettings)).Get<AuthenticationSettings>();
            if (authSettings == null) {
                throw new InvalidOperationException("AuthenticationSettings not found in configuration.");
            }

            var key = Encoding.UTF8.GetBytes(authSettings.Key);

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection(authSettings.Issuer).Get<string>(),
                    ValidAudience = configuration.GetSection(authSettings.Audience).Get<string>(),
                    IssuerSigningKey = new SymmetricSecurityKey(key)

                };
            });

            return services;
        }

    }
}
