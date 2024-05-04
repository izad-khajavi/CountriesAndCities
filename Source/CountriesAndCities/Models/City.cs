using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static CountriesAndCities.ValueObjects.Enums;

namespace CountriesAndCities.Models
{
    [Index(nameof(CityCode), IsUnique = true)]
    public class City
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public int CityId { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string? PersianName { get; set; }

        [Required]
        public string CityCode { get; set; }
        
        public CityType Type { get; set; }

        [DefaultValue(Status.Active)]
        public Status Status { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        [SwaggerSchema(ReadOnly = true)]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        // Foreign key
        public int CountryId { get; set; }

        // Navigation property
        [JsonIgnore]
        [XmlIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public Country Country { get; set; }
    }
}
