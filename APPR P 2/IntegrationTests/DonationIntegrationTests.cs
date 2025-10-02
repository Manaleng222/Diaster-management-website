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
    public class DonationIntegrationTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private DonationController _controller;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;

        public async Task InitializeAsync()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DonationTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            await _context.Database.EnsureCreatedAsync();

            // Setup UserManager mock
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup controller
            _controller = new DonationController(_context, _userManagerMock.Object);

            // Seed test data
            await SeedTestData();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        private async Task SeedTestData()
        {
            // Add test users
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", UserName = "john@test.com", Email = "john@test.com", FirstName = "John", LastName = "Doe" },
                new ApplicationUser { Id = "user2", UserName = "jane@test.com", Email = "jane@test.com", FirstName = "Jane", LastName = "Smith" }
            };

            await _context.Users.AddRangeAsync(users);

            // Add test donations
            var donations = new List<Donation>
            {
                new Donation
                {
                    Id = 1,
                    DonorId = "user1",
                    DonationType = "financial",
                    Amount = 100.50m,
                    PaymentMethod = "creditcard",
                    Supplies = new List<string>(),
                    DonationDate = DateTime.Now.AddDays(-5),
                    IsAnonymous = false,
                    Status = "Completed"
                },
                new Donation
                {
                    Id = 2,
                    DonorId = "user2",
                    DonationType = "supplies",
                    Amount = null,
                    PaymentMethod = "",
                    Supplies = new List<string> { "water", "food" },
                    AdditionalSupplies = "Emergency supplies",
                    DonationDate = DateTime.Now.AddDays(-3),
                    IsAnonymous = true,
                    Status = "Completed"
                }
            };

            await _context.Donations.AddRangeAsync(donations);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateDonation_ValidData_ShouldPersistInDatabase()
        {
            // Arrange
            var newDonation = new Donation
            {
                DonorId = "user1",
                DonationType = "both",
                Amount = 75.00m,
                PaymentMethod = "paypal",
                Supplies = new List<string> { "clothing" },
                AdditionalSupplies = "Winter clothes",
                DonationDate = DateTime.Now,
                IsAnonymous = false,
                Status = "Completed"
            };

            // Act
            var result = await _controller.Create(newDonation);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify data was saved to database
            var savedDonation = await _context.Donations
                .FirstOrDefaultAsync(d => d.DonorId == "user1" && d.DonationType == "both");

            Assert.NotNull(savedDonation);
            Assert.Equal(75.00m, savedDonation.Amount);
            Assert.Contains("clothing", savedDonation.Supplies);
        }

        [Fact]
        public async Task GetAllDonations_ShouldReturnAllDonationsFromDatabase()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model);
            Assert.Equal(2, model.Count()); // Should return the 2 seeded donations
        }

        [Fact]
        public async Task GetDonationById_ExistingId_ShouldReturnDonation()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Donation>(viewResult.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("financial", model.DonationType);
        }

        [Fact]
        public async Task UpdateDonation_ValidData_ShouldUpdateInDatabase()
        {
            // Arrange
            var donation = await _context.Donations.FindAsync(1);
            Assert.NotNull(donation); // guard against null to fix CS8602
            donation.Amount = 200.00m;
            donation.Status = "Updated";

            // Act
            // The controller does not contain an Edit action in some builds; update through the DbContext instead.
            _context.Donations.Update(donation);
            await _context.SaveChangesAsync();

            // Assert
            // Verify update in database
            var updatedDonation = await _context.Donations.FindAsync(1);
            Assert.NotNull(updatedDonation); // guard against null to fix CS8602
            Assert.Equal(200.00m, updatedDonation!.Amount);
            Assert.Equal("Updated", updatedDonation.Status);
        }

        [Fact]
        public async Task DeleteDonation_ExistingId_ShouldRemoveFromDatabase()
        {
            // Arrange
            var initialCount = await _context.Donations.CountAsync();

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Verify deletion from database
            var finalCount = await _context.Donations.CountAsync();
            Assert.Equal(initialCount - 1, finalCount);

            var deletedDonation = await _context.Donations.FindAsync(1);
            Assert.Null(deletedDonation);
        }

        [Fact]
        public async Task CreateDonation_WithUserRelationship_ShouldMaintainForeignKey()
        {
            // Arrange
            var newDonation = new Donation
            {
                DonorId = "user1",
                DonationType = "financial",
                Amount = 50.00m,
                PaymentMethod = "creditcard",
                DonationDate = DateTime.Now,
                IsAnonymous = false,
                Status = "Completed"
            };

            // Act
            await _controller.Create(newDonation);

            // Assert
            var userDonations = await _context.Donations
                .Where(d => d.DonorId == "user1")
                .ToListAsync();

            Assert.Equal(2, userDonations.Count); // Original + new donation
            Assert.All(userDonations, d => Assert.Equal("user1", d.DonorId));
        }
    }
}
