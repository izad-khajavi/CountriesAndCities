using CountriesAndCities.DTOs;
using CountriesAndCities.Models;
using CountriesAndCities.ValueObjects;

namespace CountriesAndCities.Services
{
    public interface ICityService
    {
        Task<City> GetCityAsync(int id);
        Task<List<City>> GetAllActiveCitiesAsync();
        Task<List<City>> GetAllCitiesAsync();

        Task<int> CreateCityAsync(City city);
       
        Task UpdateCityAsync(City city);
        Task UpdateCitySpecificFieldsAsync(int id, Dictionary<string, object> itemsToUpdate);
        Task UpdateCitySpecificFieldsAsync(City city, Dictionary<string, object> itemsToUpdate);
        
        Task DeleteCityAsync(int id, bool completeDeletion = false);
        Task DeleteCityAsync(City city, bool completeDeletion = false);
        Task DeleteAllCitiesAsync(bool completeDeletion = false);

        Task RollbackDeletedCityAsync(int id);
        Task RollbackDeletedCitiesAsync();
    }
}
