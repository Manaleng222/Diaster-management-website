using APPR_P_2.Controllers;
using APPR_P_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace APPR_P_2.Tests.ControllerTests
{
    public class DonationControllerTests
    {
        private readonly DonationController _controller;

        public DonationControllerTests()
        {
            
        }

        [Fact]
        public void Donate_Get_ReturnsViewResult_WithDropdownData()
        {
            // Act
            var result = _controller.Donate();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["DonationTypes"]);
            Assert.NotNull(viewResult.ViewData["PaymentMethods"]);
            Assert.NotNull(viewResult.ViewData["SupplyItems"]);
        }

        [Fact]
        public void Donate_Get_PopulatesDonationTypesDropdown()
        {
            // Act
            var result = _controller.Donate();
            var viewResult = Assert.IsType<ViewResult>(result);
            var donationTypes = viewResult.ViewData["DonationTypes"] as IEnumerable<SelectListItem>;

            // Assert
            Assert.NotNull(donationTypes);
            Assert.Contains(donationTypes, item => item.Value == "financial" && item.Text == "Financial Donation");
            Assert.Contains(donationTypes, item => item.Value == "supplies" && item.Text == "Supplies Donation");
            Assert.Contains(donationTypes, item => item.Value == "both" && item.Text == "Both Financial and Supplies");
        }

        [Fact]
        public void Donate_Get_PopulatesPaymentMethodsDropdown()
        {
            // Act
            var result = _controller.Donate();
            var viewResult = Assert.IsType<ViewResult>(result);
            var paymentMethods = viewResult.ViewData["PaymentMethods"] as IEnumerable<SelectListItem>;

            // Assert
            Assert.NotNull(paymentMethods);
            Assert.Contains(paymentMethods, item => item.Value == "creditcard" && item.Text == "Credit Card");
            Assert.Contains(paymentMethods, item => item.Value == "paypal" && item.Text == "PayPal");
            Assert.Contains(paymentMethods, item => item.Value == "banktransfer" && item.Text == "Bank Transfer");
        }

        [Fact]
        public void Donate_Get_PopulatesSupplyItemsDropdown()
        {
            // Act
            var result = _controller.Donate();
            var viewResult = Assert.IsType<ViewResult>(result);
            var supplyItems = viewResult.ViewData["SupplyItems"] as IEnumerable<SelectListItem>;

            // Assert
            Assert.NotNull(supplyItems);
            Assert.Contains(supplyItems, item => item.Value == "water" && item.Text == "Bottled Water");
            Assert.Contains(supplyItems, item => item.Value == "food" && item.Text == "Non-perishable Food");
            Assert.Contains(supplyItems, item => item.Value == "blankets" && item.Text == "Blankets");
        }

        [Fact]
        public void ProcessDonation_ValidModel_RedirectsToThankYou()
        {
            // Arrange
            var model = new DonationViewModel
            {
                DonorFirstName = "John",
                DonorLastName = "Doe",
                DonorEmail = "john.doe@example.com",
                DonationType = "financial",
                DonationAmount = 100,
                PaymentMethod = "creditcard",
                Supplies = new List<string>(),
                DonateAnonymously = false
            };

            // Act
            var result = _controller.ProcessDonation(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ThankYou", redirectResult.ActionName);
        }

        [Fact]
        public void ProcessDonation_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new DonationViewModel
            {
                // Missing required fields to make model invalid
                DonorFirstName = "", // Required field empty
                DonorLastName = "Doe",
                DonorEmail = "invalid-email" // Invalid email format
            };

            _controller.ModelState.AddModelError("DonorFirstName", "First name is required");
            _controller.ModelState.AddModelError("DonorEmail", "Invalid email address");

            // Act
            var result = _controller.ProcessDonation(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Donate", viewResult.ViewName);
            Assert.Same(model, viewResult.Model);
        }

        [Fact]
        public void ProcessDonation_InvalidModel_RepopulatesDropdowns()
        {
            // Arrange
            var model = new DonationViewModel
            {
                DonorFirstName = "", // Required field empty
                DonorLastName = "Doe",
                DonorEmail = "invalid-email"
            };

            _controller.ModelState.AddModelError("DonorFirstName", "First name is required");

            // Act
            var result = _controller.ProcessDonation(model);
            var viewResult = Assert.IsType<ViewResult>(result);

            // Assert
            Assert.NotNull(viewResult.ViewData["DonationTypes"]);
            Assert.NotNull(viewResult.ViewData["PaymentMethods"]);
            Assert.NotNull(viewResult.ViewData["SupplyItems"]);
        }

        [Fact]
        public void ThankYou_ReturnsViewResult_WithDonationId()
        {
            // Arrange
            var donationId = 123; // Use an int instead of Guid 

            // Act
            var result = _controller.ThankYou(donationId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(donationId, viewResult.ViewData["DonationId"]);
        }

        [Fact]
        public void ThankYou_ReturnsValidView_ForAnyGuid()
        {
            // Arrange
            var donationId = 123;

            // Act
            var result = _controller.ThankYou(donationId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            Assert.Equal(donationId, viewResult.ViewData["DonationId"]);
        }
    }
}