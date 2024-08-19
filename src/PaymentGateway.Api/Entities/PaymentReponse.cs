using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PaymentGateway.Api.Attributes;
using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Entities
{
    public sealed class PaymentResponse : BaseEntity
    {   
        [Required]
        public PaymentStatus Status { get; set; }

        // Added Column data annotations in the event an EF Core migration is to be run on a schema
        [Required]
        [Column(TypeName = "varchar(4)")]
        [Range(4, 4, ErrorMessage = "{0} must have {1} digits.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "The {0} must contain only numeric characters.")]
        public required string LastFourCardDigits { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "The {0} must be between {1} and {2}.")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Range(1, 2100, ErrorMessage = "The year must be a valid year.")]
        public int ExpiryYear { get; set; }

        // Added Column data annotations in the event an EF Core migration is to be run on a schema
        [Required]
        [IsoCurrencyCode]
        [Column(TypeName = "varchar(3)")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "The {0} must be {1} characters long.")]
        public required string Currency { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} must be a positive integer.")]
        public int Amount { get; set; }
    }
}