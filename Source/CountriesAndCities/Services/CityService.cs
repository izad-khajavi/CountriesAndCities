using AutoMapper;
using AutoMapper.Internal;
using CountriesAndCities.Data;
using CountriesAndCities.DTOs;
using CountriesAndCities.Models;
using CountriesAndCities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Nodes;
using static CountriesAndCities.ValueObjects.Enums;

namespace CountriesAndCities.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;

        public CityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<List<City>> GetAllActiveCitiesAsync()
        {
            return await _context.Cities.Where(c=>!c.IsDeleted && c.Status == Status.Active).ToListAsync();
        }

        public async Task<int> CreateCityAsync(City city)
        {
            var country = await _context.Countries.FindAsync(city.CountryId);
            if (country == null || country.Status != Status.Active)
            {
                throw new InvalidOperationException("Cannot register a city for an inactive country.");
            }

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city.CityId;
        }

        public async Task UpdateCityAsync(City city)
        {
            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCitySpecificFieldsAsync(int id, Dictionary<string, object> itemsToUpdate)
        {
            var city = await _context.Cities.FindAsync(id);
            await UpdateCitySpecificFieldsAsync(city, itemsToUpdate);
        }

        public async Task UpdateCitySpecificFieldsAsync(City city, Dictionary<string, object> itemsToUpdate)
        {
            foreach (var item in itemsToUpdate)
            {
                PropertyInfo propertyInfo = city.GetType().GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    if ((item.Value == null || item.Value == DBNull.Value) && propertyInfo.PropertyType.IsNullableType())
                    {
                        propertyInfo.SetValue(city, item.Value);
                    }
                    else
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(city, Convert.ToInt32(item.Value.ToString()));
                        }
                        else
                        {
                            propertyInfo.SetValue(city, Convert.ChangeType(item.Value.ToString(), propertyInfo.PropertyType), null);
                        }
                    }

                    _context.Entry(city).Property(propertyInfo.Name).IsModified = true;

                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCityAsync(int id, bool completeDeletion = false)
        {
            var city = await _context.Cities.FindAsync(id);
            await DeleteCityAsync(city, completeDeletion);
        } 

        public async Task DeleteCityAsync(City city, bool completeDeletion = false)
        {
            if (city != null)
            {
                if (completeDeletion)
                {
                    _context.Cities.Remove(city);
                }
                else
                {
                    city.IsDeleted = true;
                    _context.Entry(city).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
        }


        #region Admin

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task DeleteAllCitiesAsync(bool completeDeletion = false)
        {
            if (!completeDeletion)
            {
                _context.Cities.ExecuteUpdate(c => c.SetProperty(n => n.IsDeleted, n => true)
                                                        .SetProperty(n => n.Status, n => Status.InActive));
            }
            else
            {
                _context.Cities.ExecuteDelete();
            }
            await _context.SaveChangesAsync();
        }


        public async Task RollbackDeletedCityAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                city.IsDeleted = false;
                city.Status = Status.Active;
                _context.Entry(city).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RollbackDeletedCitiesAsync()
        {
            _context.Cities.Where(a=>a.IsDeleted)?.ExecuteUpdate(c => c.SetProperty(n => n.IsDeleted, n => false)
                                                       .SetProperty(n => n.Status, n => Status.Active));
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
