﻿using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static CountriesAndCities.ValueObjects.Enums;

namespace CountriesAndCities.DTOs
{
    public class CountryDto
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        public int CountryId { get; set; }

        [Required]
        public string Name { get; set; }

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
    }
}