using APPR_P_2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace APPR_P_2.Tests.ModelTests
{
    public class DonationModelTests
    {
        [Fact]
        public void Donation_ValidData_ShouldPassValidation()
        {
            // Arrange
            var donation = new Donation
            {
                DonorId = "user123",
                DonationType = "financial",
                Amount = 100.50m,
                PaymentMethod = "creditcard",
                Supplies = new List<string> { "water", "food" },
                AdditionalSupplies = "Additional items",
                DonationDate = DateTime.Now,
                IsAnonymous = false,
                Status = "Completed"
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Donation_MissingDonorId_ShouldFailValidation()
        {
            // Arrange
            var donation = new Donation
            {
                // Missing DonorId (required field)
                DonationType = "financial",
                Amount = 100.50m
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("DonorId"));
        }

        [Fact]
        public void Donation_MissingDonationType_ShouldFailValidation()
        {
            // Arrange
            var donation = new Donation
            {
                DonorId = "user123",
                // Missing DonationType (required field)
                Amount = 100.50m
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("DonationType"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Donation_InvalidAmount_ShouldFailValidation(decimal amount)
        {
            // Arrange
            var donation = new Donation
            {
                DonorId = "user123",
                DonationType = "financial",
                Amount = amount,
                PaymentMethod = "creditcard"
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Amount"));
        }

        [Fact]
        public void Donation_ValidAmount_ShouldPassValidation()
        {
            // Arrange
            var donation = new Donation
            {
                DonorId = "user123",
                DonationType = "financial",
                Amount = 0.01m, // Minimum valid amount
                PaymentMethod = "creditcard"
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Donation_SuppliesDonation_WithoutAmount_ShouldPassValidation()
        {
            // Arrange
            var donation = new Donation
            {
                DonorId = "user123",
                DonationType = "supplies",
                // Amount can be null for supplies donation
                Supplies = new List<string> { "water", "food" },
                PaymentMethod = "creditcard"
            };

            var context = new ValidationContext(donation);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(donation, context, results, true);

            // Assert
            Assert.True(isValid);
        }
    }
}