using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using HaveBreak.Common;
using HaveBreak.Shared.Enums;
using HaveBreak.Shared;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;

namespace HaveBreak.API.Controllers.Accounts.Dtos
{
    public class RegisterInputDto : IShouldNormalize, IValidatableObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public byte? Gender { get; set; }

        public void Normalize()
        {
            FirstName = FirstName?.Trim();
            LastName = LastName?.Trim();
            Email = Email?.Trim();
            Password = Password?.Trim();
            PhoneNumber = PhoneNumber?.Trim();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var factory = validationContext.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(CommonResource));
            var results = new List<ValidationResult>();


            var r = new Regex(Constants.PhoneRegularExpression);
            if (!string.IsNullOrEmpty(PhoneNumber) && !r.IsMatch(PhoneNumber))
                results.Add(new ValidationResult(localizer["PhoneNumberInput", nameof(PhoneNumber)],
                    new[] { nameof(PhoneNumber) }));

            r = new Regex(Constants.EmailRegularExpression);
            if (!string.IsNullOrEmpty(Email) && !r.IsMatch(Email))
                results.Add(new ValidationResult(localizer["InvalidEmailFormat", nameof(Email)],
                    new[] { nameof(Email) }));

            if (Gender.HasValue && !Enum.IsDefined(typeof(Gender), Gender.Value))
                results.Add(new ValidationResult(localizer["InvalidEnumValue", nameof(Gender)],
                new[] { nameof(Gender) }));

            return results;
        }
    }
}
