using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.DTO.Trip
{
        public class RequestTripDto
        {
              [Required]
              public PaymentMethod PaymentMethod { get; set; }
              [Required]
              [JsonPropertyName("PickupCoordinates")]
              public Coordinates PickupCoordinates { get; set; }
              [Required]
              [JsonPropertyName("DistinationCoordinates")]
              public Coordinates DistinationCoordinates { get; set; }
             
            [JsonPropertyName("PickUpName")]

            public string? PickUpName { get; set; }
            [JsonPropertyName("DestinationName")]
            public string? DestinationName { get; set; }
          
        }
}
