using AutoMapper;
using CountriesAndCities.Authorization;
using CountriesAndCities.DTOs;
using CountriesAndCities.Extensions;
using CountriesAndCities.Models;
using CountriesAndCities.Services;
using CountriesAndCities.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace CountriesAndCities.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;

        public CityController(ICityService cityService, IMapper mapper)
        {
            _cityService = cityService;
            _mapper = mapper;
        }

        [OnlyActiveCity]
        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(int id)
        {
            var city = await _cityService.GetCityAsync(id);
            var user = Request.HttpContext.User;
            if (city == null || (city.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            var cityDto = _mapper.Map<CityDto>(city);
            return Ok(cityDto);
        }


        [HttpGet]
        public async Task<ActionResult<List<CityDto>>> GetAllActiveCities()
        {
            var cities = await _cityService.GetAllActiveCitiesAsync();
            var cityDtos = _mapper.Map<List<CityDto>>(cities);
            return Ok(cityDtos);
        }


        [HttpPost]
        public async Task<ActionResult> CreateCity(CityDto cityDto)
        {
            var city = _mapper.Map<City>(cityDto);
            await _cityService.CreateCityAsync(city);
            return CreatedAtAction(nameof(GetCity), new { id = city.CityId }, cityDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
        {
            var existingCity = await _cityService.GetCityAsync(id);
            var user = Request.HttpContext.User;
            if (existingCity == null || (existingCity.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            _mapper.Map(cityDto, existingCity);

            await _cityService.UpdateCityAsync(existingCity);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCitySpecificFields(int id,Dictionary<string, object> itemsToUpdate)
        {
            if (itemsToUpdate == null || itemsToUpdate.Count == 0)
            {
                throw new InvalidDataException("No field(s) selected to update.");
            }

            var existingCity = await _cityService.GetCityAsync(id);
            var user = Request.HttpContext.User;
            if (existingCity == null || (existingCity.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            await _cityService.UpdateCitySpecificFieldsAsync(existingCity, itemsToUpdate);

            return NoContent();
        }


        [HttpDelete("{id}/{completeDeletion:bool}")]
        public async Task<IActionResult> DeleteCity(int id, bool completeDeletion = false)
        {
            var existingCity = await _cityService.GetCityAsync(id);
            var user = Request.HttpContext.User;
            if (existingCity == null || (existingCity.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            await _cityService.DeleteCityAsync(existingCity, completeDeletion);

            return NoContent();
        }

        #region  Admin

        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<List<CityDto>>> GetAllCities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            var cityDtos = _mapper.Map<List<CityDto>>(cities);
            return Ok(cityDtos);
        }

        [Authorize("AdminOnly")]
        [HttpDelete("{completeDeletion:bool}")]
        public async Task<IActionResult> DeleteAllCities(bool completeDeletion = false)
        {
            await _cityService.DeleteAllCitiesAsync(completeDeletion);

            return NoContent();
        }


        [Authorize("AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> RollbackDeletedCityAsync(int id)
        {
            await _cityService.RollbackDeletedCityAsync(id);

            return NoContent();
        }

        [Authorize("AdminOnly")]
        [HttpPut]
        public async Task<IActionResult> RollbackDeletedCitiesAsync()
        {
            await _cityService.RollbackDeletedCitiesAsync();

            return NoContent();
        }
        #endregion
    }
}
