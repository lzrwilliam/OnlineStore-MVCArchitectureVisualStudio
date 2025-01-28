using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProiectPAW.ContextModels;
using ProiectPAW.Models;
using ProiectPAWMvc.Logic;
using System.Linq;
using System.Threading.Tasks;

public class CustomRoleProvider : IAuthorizationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomRoleProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var user = _httpContextAccessor.HttpContext.User;

        if (user.Identity.IsAuthenticated)
        {
            var dbContext = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ProdusContext>();
            var userName = user.Identity.Name;
            var utilizator = dbContext.Utilizator.FirstOrDefault(u => u.Nume == userName);

            if (utilizator != null)
            {
                foreach (var requirement in context.PendingRequirements.ToList())
                {
                    if (requirement is RolesAuthorizationRequirement roleRequirement)
                    {
                        if (roleRequirement.AllowedRoles.Contains("Moderator") && utilizator.TipUtilizator == TipUser.Moderator)
                        {
                            context.Succeed(requirement);
                        }
                        else if (roleRequirement.AllowedRoles.Contains("Logat") && utilizator.TipUtilizator == TipUser.Logat)
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}
