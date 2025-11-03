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
            // Add test users with ALL required properties
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "user1",
                    UserName = "john@test.com",
                    Email = "john@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "user2",
                    UserName = "jane@test.com",
                    Email = "jane@test.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true
                }
            };

            await _context.Users.AddRangeAsync(users);

            // Add test donations with ALL required properties
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
                    Amount = 0, // Required for supplies donations
                    PaymentMethod = "none", // Required property
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

        // ... rest of the test methods remain similar but ensure all required properties are set
    }
}
