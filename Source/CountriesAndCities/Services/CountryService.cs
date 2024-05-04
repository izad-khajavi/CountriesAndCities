using AutoMapper;
using AutoMapper.Internal;
using CountriesAndCities.Data;
using CountriesAndCities.DTOs;
using CountriesAndCities.Models;
using CountriesAndCities.ValueObjects;
using Microsoft.AspNetCore.Identity;
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
    public class CountryService : ICountryService
    {
        private readonly ApplicationDbContext _context;

        public CountryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Country> GetCountryAsync(int id)
        {
            return await _context.Countries.FindAsync(id);
        }

        public async Task<List<Country>> GetAllActiveCountriesAsync()
        {
            return await _context.Countries.Where(a => !a.IsDeleted && a.Status == Status.Active).ToListAsync();
        }

        public object? GetAllActiveCountriesAndRelatedCities()
        {
            var query = _context.Countries.Select(cnt => new
            {
                CountryId = cnt.CountryId,
                CountryName = cnt.Name,
                CountryPersianName = cnt.PersianName,
                CountryCode = cnt.CountryCode,
                CountryStatus = cnt.Status,
                CountryIsDeleted = cnt.IsDeleted,

                Cities = cnt.Cities.Select(ct => new
                {
                    CityId = ct.CityId,
                    CityName = ct.Name,
                    CityPersianName = ct.PersianName,
                    CityCode = ct.CityCode,
                    CityType = ct.Type,
                    CityStatus =  ct.Status,
                    CityIsDeleted = ct.IsDeleted
                }).Where(ct => !ct.CityIsDeleted && ct.CityStatus == Status.Active)
            }).Where(cnt => !cnt.CountryIsDeleted && cnt.CountryStatus == Status.Active);

            return query?.ToList();

        }


        public async Task<int> CreateCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country.CountryId;
        }

        public async Task UpdateCountryAsync(Country country)
        {
            _context.Entry(country).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCountrySpecificFieldsAsync(int id, Dictionary<string, object> itemsToUpdate)
        {
            var country = await _context.Countries.FindAsync(id);
            await UpdateCountrySpecificFieldsAsync(country, itemsToUpdate);
        }

        public async Task UpdateCountrySpecificFieldsAsync(Country country, Dictionary<string, object> itemsToUpdate)
        {

            foreach (var item in itemsToUpdate)
            {
                PropertyInfo propertyInfo = country.GetType().GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    if ((item.Value == null || item.Value == DBNull.Value) && propertyInfo.PropertyType.IsNullableType())
                    {
                        propertyInfo.SetValue(country, item.Value);
                    }
                    else
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(country, Convert.ToInt32(item.Value.ToString()));

                        }
                        else
                        {
                            propertyInfo.SetValue(country, Convert.ChangeType(item.Value.ToString(), propertyInfo.PropertyType), null);
                        }
                    }

                    _context.Entry(country).Property(propertyInfo.Name).IsModified = true;

                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCountryAsync(int id, bool completeDeletion = false)
        {
            var country = await _context.Countries.FindAsync(id);
            await DeleteCountryAsync(country, completeDeletion);
        }

        public async Task DeleteCountryAsync(Country country, bool completeDeletion = false)
        {
            if (country != null)
            {
                if (completeDeletion)
                {
                    _context.Countries.Remove(country);
                }
                else
                {
                    country.IsDeleted = true;
                    country.Status = Status.InActive;
                    _context.Entry(country).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
        }


        #region Admin
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        public object? GetAllCountriesAndRelatedCities()
        {
            var query = _context.Countries.Select(cnt => new
            {
                CountryId = cnt.CountryId,
                CountryName = cnt.Name,
                CountryPersianName = cnt.PersianName,
                CountryCode = cnt.CountryCode,
                CountryStatus = cnt.Status,
                CountryIsDeleted = cnt.IsDeleted,

                Cities = cnt.Cities.Select(ct => new
                {
                    CityId = ct.CityId,
                    CityName = ct.Name,
                    CityPersianName = ct.PersianName,
                    CityCode = ct.CityCode,
                    CityType = ct.Type,
                    CityStatus = ct.Status,
                    CityIsDeleted = ct.IsDeleted
                })
            });

            return query?.ToList();

        }

        public async Task DeleteAllCountriesAsync(bool completeDeletion = false)
        {
            if (!completeDeletion)
            {
                _context.Countries.ExecuteUpdate(c => c.SetProperty(n => n.IsDeleted, n => true)
                                                            .SetProperty(n => n.Status, n => Status.InActive));
            }
            else
            {
                _context.Countries.ExecuteDelete();
            }
            await _context.SaveChangesAsync();
        }

        public async Task RollbackDeletedCountryAsync(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                country.IsDeleted = false;
                country.Status = Status.Active;
                _context.Entry(country).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RollbackDeletedCountriesAsync()
        {
            _context.Countries.Where(a=>a.IsDeleted)?.ExecuteUpdate(c => c.SetProperty(n => n.IsDeleted, n => false)
                                                       .SetProperty(n => n.Status, n => Status.Active));
            await _context.SaveChangesAsync();
        }


        #endregion
    }
}

