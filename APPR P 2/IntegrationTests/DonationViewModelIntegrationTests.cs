using APPR_P_2.Controllers;
using APPR_P_2.Data;
using APPR_P_2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace APPR_P_2.IntegrationTests
{
    public class DonationViewModelIntegrationTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private DonationController _controller;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DonationViewModelTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            await _context.Database.EnsureCreatedAsync();

            var userManagerMock = CreateMockUserManager();
            _controller = new DonationController(_context, userManagerMock.Object);
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        [Fact]
        public async Task ProcessDonation_ValidViewModel_ShouldCreateDonationInDatabase()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "Integration",
                DonorLastName = "Test",
                DonorEmail = "integration@test.com",
                DonationType = "financial",
                DonationAmount = 150.00m,
                PaymentMethod = "creditcard",
                Supplies = new List<string>(),
                DonateAnonymously = false,
                SubscribeToNewsletter = true
            };

            // Act
            var result = await _controller.ProcessDonation(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ThankYou", redirectResult.ActionName);

            // Verify donation was created in database
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.DonationType == "financial" && d.Amount == 150.00m);

            Assert.NotNull(donation);
            Assert.Equal("creditcard", donation.PaymentMethod);
            Assert.Equal("Completed", donation.Status);
        }

        [Fact]
        public async Task ProcessDonation_SuppliesDonation_ShouldSaveSuppliesList()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "Supplies",
                DonorLastName = "Test",
                DonorEmail = "supplies@test.com",
                DonationType = "supplies",
                DonationAmount = null,
                PaymentMethod = "",
                Supplies = new List<string> { "water", "food", "blankets" },
                AdditionalSupplies = "Emergency relief packages",
                DonateAnonymously = true
            };

            // Act
            var result = await _controller.ProcessDonation(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            // Verify supplies were saved correctly
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.DonationType == "supplies" && d.IsAnonymous == true);

            Assert.NotNull(donation);
            Assert.Equal(3, donation.Supplies.Count);
            Assert.Contains("water", donation.Supplies);
            Assert.Contains("food", donation.Supplies);
            Assert.Contains("blankets", donation.Supplies);
            Assert.Equal("Emergency relief packages", donation.AdditionalSupplies);
        }

        [Fact]
        public async Task ProcessDonation_InvalidViewModel_ShouldReturnViewWithErrors()
        {
            // Arrange
            var viewModel = new DonationViewModel
            {
                // Missing required fields
                DonorFirstName = "",
                DonorLastName = "",
                DonorEmail = "invalid-email",
                DonationType = "financial"
            };

            // Manually add model errors to simulate validation failure
            _controller.ModelState.AddModelError("DonorFirstName", "First name is required");
            _controller.ModelState.AddModelError("DonorLastName", "Last name is required");
            _controller.ModelState.AddModelError("DonorEmail", "Invalid email format");

            // Act
            var result = await _controller.ProcessDonation(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Donate", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);

            // Verify no donation was created
            var donationsCount = await _context.Donations.CountAsync();
            Assert.Equal(0, donationsCount); // No donations should be created for invalid data
        }

        [Fact]
        public async Task ThankYou_WithValidDonationId_ShouldReturnViewWithDonation()
        {
            // Arrange - Create a donation first
            var viewModel = new DonationViewModel
            {
                DonorFirstName = "ThankYou",
                DonorLastName = "Test",
                DonorEmail = "thankyou@test.com",
                DonationType = "financial",
                DonationAmount = 100.00m,
                PaymentMethod = "paypal",
                DonateAnonymously = false
            };

            var processResult = await _controller.ProcessDonation(viewModel);
            var redirectResult = Assert.IsType<RedirectToActionResult>(processResult);
            var donationId = (int)redirectResult.RouteValues["id"];

            // Act
            var result = await _controller.ThankYou(donationId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<Donation>(viewResult.Model);
            Assert.Equal(donationId, ((Donation)viewResult.Model).Id);
        }

        [Fact]
        public async Task Index_ShouldReturnListOfDonations()
        {
            // Arrange - Add some test data
            var donation = new Donation
            {
                DonorId = "test-user",
                DonationType = "financial",
                Amount = 50.00m,
                Status = "Completed"
            };
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model);
            Assert.Single(model); // Should contain our test donation
        }

        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }
    }
}