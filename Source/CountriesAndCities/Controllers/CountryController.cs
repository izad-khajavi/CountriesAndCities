using AutoMapper;
using AutoMapper.Configuration.Annotations;
using CountriesAndCities.Authorization;
using CountriesAndCities.DTOs;
using CountriesAndCities.Extensions;
using CountriesAndCities.Models;
using CountriesAndCities.Services;
using CountriesAndCities.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace CountriesAndCities.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountryController(ICountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        [OnlyActiveCountry]
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countryService.GetCountryAsync(id);
            var user = Request.HttpContext.User;
            if (country == null || (country.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(country);
            return Ok(countryDto);
        }


        [HttpGet]
        public async Task<ActionResult<List<CountryDto>>> GetAllActiveCountries()
        {
            var countries = await _countryService.GetAllActiveCountriesAsync();
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return Ok(countryDtos);
        }

        [HttpGet]
        public object GetAllActiveCountriesAndRelatedCities()
        {
            var countriesAndCities =  _countryService.GetAllActiveCountriesAndRelatedCities();
            return Ok(countriesAndCities);
        }


        [HttpPost]
        public async Task<ActionResult> CreateCountry(CountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryService.CreateCountryAsync(country);
            _mapper.Map(country, countryDto);
            return  CreatedAtAction(nameof(GetCountry), new { id = country.CountryId }, countryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, CountryDto countryDto)
        {
            var existingCountry = await _countryService.GetCountryAsync(id);
            var user = Request.HttpContext.User;
            if (existingCountry == null || (existingCountry.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            _mapper.Map(countryDto, existingCountry);

            await _countryService.UpdateCountryAsync(existingCountry);

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountrySpecificFields(int id,Dictionary<string,object> itemsToUpdate)
        {
            if(itemsToUpdate== null || itemsToUpdate.Count ==0)
            {
                throw new InvalidDataException("No field(s) selected to update.");
            }

            var existingCountry = await _countryService.GetCountryAsync(id);
            var user = Request.HttpContext.User;
            if (existingCountry == null || (existingCountry.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            await _countryService.UpdateCountrySpecificFieldsAsync(existingCountry, itemsToUpdate);

            return NoContent();
        }

        [HttpDelete("{id}/{completeDeletion:bool}")]
        public async Task<IActionResult> DeleteCountry(int id, bool completeDeletion = false)
        {
            var existingCountry = await _countryService.GetCountryAsync(id);
            var user = Request.HttpContext.User;
            if (existingCountry == null || (existingCountry.IsDeleted && (user.HasClaim(claim => claim.Type == ClaimTypes.Role && claim.Value != "Admin"))))
            {
                return NotFound();
            }

            await _countryService.DeleteCountryAsync(existingCountry,completeDeletion);

            return NoContent();
        }


        #region Admin

        [Authorize("AdminOnly")]
        [HttpGet]
        public async Task<ActionResult<List<CountryDto>>> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return Ok(countryDtos);
        }

        [Authorize("AdminOnly")]
        [HttpGet]
        public object GetAllCountriesAndRelatedCities()
        {
            var countriesAndCities = _countryService.GetAllCountriesAndRelatedCities();
            return Ok(countriesAndCities);
        }

        [Authorize("AdminOnly")]
        [HttpDelete("{completeDeletion:bool}")]
        public async Task<IActionResult> DeleteAllCountries(bool completeDeletion = false)
        {
            await _countryService.DeleteAllCountriesAsync(completeDeletion);

            return NoContent();
        }

        [Authorize("AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> RollbackDeletedCountryAsync(int id)
        {
            await _countryService.RollbackDeletedCountryAsync(id);

            return NoContent();
        }

        [Authorize("AdminOnly")]
        [HttpPut]
        public async Task<IActionResult> RollbackDeletedCountriesAsync()
        {
            await _countryService.RollbackDeletedCountriesAsync();

            return NoContent();
        }
        #endregion
    }

}
