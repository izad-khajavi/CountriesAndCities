using CountriesAndCities.DTOs;
using CountriesAndCities.Models;
using CountriesAndCities.ValueObjects;
using System.Text.Json.Nodes;

namespace CountriesAndCities.Services
{
    public interface ICountryService
    {
        Task<Country> GetCountryAsync(int id);
        Task<List<Country>> GetAllActiveCountriesAsync();
        Task<List<Country>> GetAllCountriesAsync();
        object GetAllActiveCountriesAndRelatedCities();
        object GetAllCountriesAndRelatedCities();

        Task<int> CreateCountryAsync(Country country);

        Task UpdateCountryAsync(Country country);
        Task UpdateCountrySpecificFieldsAsync(int id, Dictionary<string, object> itemsToUpdate);
        Task UpdateCountrySpecificFieldsAsync(Country country, Dictionary<string, object> itemsToUpdate);

        Task DeleteCountryAsync(int id, bool completeDeletion = false);
        Task DeleteCountryAsync(Country country, bool completeDeletion = false);
        Task DeleteAllCountriesAsync(bool completeDeletion = false);
        
        Task RollbackDeletedCountryAsync(int id);
        Task RollbackDeletedCountriesAsync();
    }
}
