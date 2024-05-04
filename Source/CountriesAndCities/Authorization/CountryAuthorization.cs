using CountriesAndCities.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using static CountriesAndCities.ValueObjects.Enums;

namespace CountriesAndCities.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OnlyActiveCountry : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user =context.HttpContext.User;
            if (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value ==  "Admin"))
            {
                return;
            }

            var countryId = Convert.ToInt32(context.HttpContext.Request.RouteValues["id"]);
            var countryService = context.HttpContext.RequestServices.GetRequiredService<ICountryService>();
            var country = countryService.GetCountryAsync(countryId).Result;
             
            if (country == null || country.Status != Status.Active)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
