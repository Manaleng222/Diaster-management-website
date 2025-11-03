using APPR_P_2.Data;
using APPR_P_2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace APPR_P_2.IntegrationTests
{
    public class DatabaseRelationshipTests : IAsyncLifetime
    {
        private ApplicationDbContext? _context;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"RelationshipTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            await _context.Database.EnsureCreatedAsync();
            await SeedRelationshipData();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        private async Task SeedRelationshipData()
        {
            // Add user with ALL required properties
            var user = new ApplicationUser
            {
                Id = "test-user",
                UserName = "test@relationship.com",
                Email = "test@relationship.com",
                FirstName = "Test", // Add required property
                LastName = "User",  // Add required property
                EmailConfirmed = true
            };
            await _context.Users.AddAsync(user);

            // Add donations with ALL required properties
            var donations = new[]
            {
                new Donation
                {
                    DonorId = "test-user",
                    DonationType = "financial",
                    Amount = 100.00m,
                    PaymentMethod = "creditcard", // Add required property
                    Status = "Completed",
                    DonationDate = DateTime.UtcNow
                },
                new Donation
                {
                    DonorId = "test-user",
                    DonationType = "supplies",
                    Amount = 0, // Add default amount
                    PaymentMethod = "none", // Add required property
                    Supplies = new System.Collections.Generic.List<string> { "food" },
                    Status = "Completed",
                    DonationDate = DateTime.UtcNow
                }
            };

            await _context.Donations.AddRangeAsync(donations);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Donation_UserRelationship_ShouldBeEstablished()
        {
            // Act
            var userWithDonations = await _context.Users
                .Include(u => u.Donations)
                .FirstOrDefaultAsync(u => u.Id == "test-user");

            // Assert
            Assert.NotNull(userWithDonations);
            Assert.Equal(2, userWithDonations.Donations.Count);
            Assert.All(userWithDonations.Donations, d => Assert.Equal("test-user", d.DonorId));
        }

        [Fact]
        public async Task Donation_SuppliesConversion_ShouldWorkCorrectly()
        {
            // Act
            var suppliesDonation = await _context.Donations
                .FirstOrDefaultAsync(d => d.DonationType == "supplies");

            // Assert
            Assert.NotNull(suppliesDonation);
            Assert.Single(suppliesDonation.Supplies);
            Assert.Contains("food", suppliesDonation.Supplies);
        }

        [Fact]
        public async Task CascadeDelete_UserDeleted_ShouldDeleteDonations()
        {
            // Arrange
            var user = await _context.Users.FindAsync("test-user");
            var userDonationsCount = await _context.Donations
                .Where(d => d.DonorId == "test-user")
                .CountAsync();

            Assert.Equal(2, userDonationsCount); // Verify donations exist

            // Act
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Assert
            var remainingDonations = await _context.Donations
                .Where(d => d.DonorId == "test-user")
                .CountAsync();

            Assert.Equal(0, remainingDonations); // All donations should be deleted
        }
    }
}