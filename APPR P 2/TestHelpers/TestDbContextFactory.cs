using APPR_P_2.Data;
using APPR_P_2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
            .Options;

        return new ApplicationDbContext(options);
    }

    public static void InitializeData(ApplicationDbContext context)
    {
        // Add test donations
        context.Donations.AddRange(
            new Donation
            {
                Id = 1,
                DonorId = "user1",
                DonationType = "financial",
                Amount = 100.50m,
                PaymentMethod = "creditcard",
                Supplies = new List<string>(),
                AdditionalSupplies = "",
                DonationDate = DateTime.Now.AddDays(-5),
                IsAnonymous = false,
                Status = "Completed"
            },
            new Donation
            {
                Id = 2,
                DonorId = "user2",
                DonationType = "supplies",
                Amount = 100.50m,
                PaymentMethod = "",
                Supplies = new List<string> { "water", "food" },
                AdditionalSupplies = "Emergency supplies",
                DonationDate = DateTime.Now.AddDays(-3),
                IsAnonymous = true,
                Status = "Completed"
            },
            new Donation
            {
                Id = 3,
                DonorId = "user3",
                DonationType = "both",
                Amount = 50.00m,
                PaymentMethod = "paypal",
                Supplies = new List<string> { "clothing", "blankets" },
                AdditionalSupplies = "Winter clothing",
                DonationDate = DateTime.Now.AddDays(-1),
                IsAnonymous = false,
                Status = "Pending"
            }
        );

        // Add test application users (if needed for relationships)
        context.Users.AddRange(
            new ApplicationUser
            {
                Id = "user1",
                UserName = "john.doe@example.com",
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe"
            },
            new ApplicationUser
            {
                Id = "user2",
                UserName = "jane.smith@example.com",
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith"
            },
            new ApplicationUser
            {
                Id = "user3",
                UserName = "bob.wilson@example.com",
                Email = "bob.wilson@example.com",
                FirstName = "Bob",
                LastName = "Wilson"
            }
        );

        context.SaveChanges();
    }

    public static void Cleanup(ApplicationDbContext context)
    {
        context.Donations.RemoveRange(context.Donations);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
    }

    public static ApplicationDbContext CreateContextWithData()
    {
        var context = CreateContext();
        InitializeData(context);
        return context;
    }
}