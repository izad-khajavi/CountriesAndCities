using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static CountriesAndCities.ValueObjects.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper.Configuration.Annotations;

namespace CountriesAndCities.Models
{
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(PersianName), IsUnique = true)]
    [Index(nameof(CountryCode), IsUnique = true)]
    public class Country
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public int CountryId { get; set; }
        
        [Required]
        public string  Name { get; set; }
        
        public string? PersianName { get; set; }
        
        [Required]
        public string CountryCode { get; set; }

        [DefaultValue(Status.Active)]
        public Status Status { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        [SwaggerSchema(ReadOnly = true)]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        // Navigation property
        
        [JsonIgnore]
        [XmlIgnore]
        [SwaggerSchema(ReadOnly = true)]
        public virtual ICollection<City>? Cities { get; set; }
    }
}
