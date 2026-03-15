using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Mistruna.Core.Base.ComplexTypes;

namespace Mistruna.Core.Microservices.Core.JwtAuth;

public class JwtHttpClient : HttpClient
{
    public JwtHttpClient(string hostUrl) : base(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
    })
    {
        Timeout = TimeSpan.FromMinutes(1.0);
        DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        SetSystemToken();
        BaseAddress = new Uri(hostUrl);
    }

    public JwtHttpClient(string hostUrl, IHttpContextAccessor accessor) : this(hostUrl)
    {
        if (accessor?.HttpContext != null && accessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = accessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authHeader);
            }
        }
    }

    private void SetSystemToken()
    {
        var systemToken = JwtTokenProvider.GenerateSystemToken(SystemUsersEnumeration.SystemMessage);
        if (string.IsNullOrEmpty(systemToken))
        {
            throw new InvalidOperationException("Failed to generate system token.");
        }
        DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", systemToken);
    }
}
