using CountriesAndCities.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static CountriesAndCities.ValueObjects.Enums;
using System.Security.Claims;

namespace CountriesAndCities.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OnlyActiveCity : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin"))
            {
                return;
            }

            var cityId = Convert.ToInt32(context.HttpContext.Request.RouteValues["id"]);
            var cityService = context.HttpContext.RequestServices.GetRequiredService<ICityService>();
            var city = cityService.GetCityAsync(cityId).Result;

            if (city == null || city.Status != Status.Active)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
