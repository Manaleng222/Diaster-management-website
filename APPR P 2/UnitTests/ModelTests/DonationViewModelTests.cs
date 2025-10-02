using APPR_P_2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace APPR_P_2.Tests.ModelTests
{
    public class DonationViewModelTests
    {
        [Fact]
        public void DonationViewModel_ValidData_ShouldPassValidation()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "financial",
                DonationAmount = 100,
                PaymentMethod = "creditcard",
                Supplies = new List<string> { "water" },
                AdditionalSupplies = "Additional items",
                DonateAnonymously = false,
                SubscribeToNewsletter = true
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("", "Doe", "john.doe@example.com")] // Missing first name
        [InlineData("John", "", "john.doe@example.com")] // Missing last name
        [InlineData("John", "Doe", "")] // Missing email
        public void DonationViewModel_MissingRequiredFields_ShouldFailValidation(string firstName, string lastName, string email)
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = firstName,
                DonorLastName = lastName,
                DonorEmail = email,
                DonationType = "financial"
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void DonationViewModel_InvalidEmailFormat_ShouldFailValidation()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "invalid-email",
                DonationType = "financial"
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("DonorEmail"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void DonationViewModel_InvalidDonationAmount_ShouldFailValidation(decimal amount)
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "financial",
                DonationAmount = amount
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("DonationAmount"));
        }

        [Fact]
        public void DonationViewModel_ValidMinimumAmount_ShouldPassValidation()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "financial",
                DonationAmount = 1, // Minimum valid amount
                PaymentMethod = "creditcard"
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void DonationViewModel_SuppliesDonation_WithoutAmount_ShouldPassValidation()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "supplies",
                // DonationAmount can be null for supplies donation
                Supplies = new List<string> { "water", "food" }
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void DonationViewModel_SubscribeToNewsletter_DefaultValue_IsTrue()
        {
            // Arrange & Act
            var viewModel = new DonationViewModel();

            // Assert
            Assert.True(viewModel.SubscribeToNewsletter);
        }

        [Fact]
        public void DonationViewModel_AdditionalSupplies_CanBeEmpty()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "supplies",
                Supplies = new List<string> { "water" },
                AdditionalSupplies = "" // Empty string should be valid
            };

            var context = new ValidationContext(viewModel);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            // Assert
            Assert.True(isValid);
        }
    }
}