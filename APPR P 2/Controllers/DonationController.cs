
using APPR_P_2.Data;
using APPR_P_2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APPR_P_2.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor with dependency injection
        public DonationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

     

        // GET: Donation/ThankYou - Changed parameter from Guid to int
        public async Task<IActionResult> ThankYou(int id)
        {
            if (_context != null)
            {
                var donation = await _context.Donations.FindAsync(id);
                if (donation != null)
                {
                    ViewBag.DonationId = id;
                    return View(donation);
                }
            }

            ViewBag.DonationId = id;
            return View();
        }

        // GET: Donation Index (for testing)
        public async Task<IActionResult> Index()
        {
            if (_context == null)
                return View(new List<Donation>());

            var donations = await _context.Donations.ToListAsync();
            return View(donations);
        }

        // GET: Donation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation)
        {
            if (_context != null && ModelState.IsValid)
            {
                _context.Add(donation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donation);
        }

        // GET: Donation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context == null)
            {
                return NotFound();
            }

            var donation = await _context.Donations
                .FirstOrDefaultAsync(m => m.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: Donation/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Donation/Donate
        public IActionResult Donate()
        {
            // Populate dropdown lists
            ViewBag.DonationTypes = new[]
            {
                new SelectListItem { Value = "financial", Text = "Financial Donation" },
                new SelectListItem { Value = "supplies", Text = "Supplies Donation" },
                new SelectListItem { Value = "both", Text = "Both Financial and Supplies" }
            };

            ViewBag.PaymentMethods = new[]
            {
                new SelectListItem { Value = "creditcard", Text = "Credit Card" },
                new SelectListItem { Value = "paypal", Text = "PayPal" },
                new SelectListItem { Value = "banktransfer", Text = "Bank Transfer" }
            };

            ViewBag.SupplyItems = new[]
            {
                new SelectListItem { Value = "water", Text = "Bottled Water" },
                new SelectListItem { Value = "food", Text = "Non-perishable Food" },
                new SelectListItem { Value = "blankets", Text = "Blankets" },
                new SelectListItem { Value = "meds", Text = "Medical Supplies" },
                new SelectListItem { Value = "hygiene", Text = "Hygiene Kits" },
                new SelectListItem { Value = "clothing", Text = "Clothing" }
            };

            return View();
        }

        // POST: Donation/ProcessDonation - Fixed to return Task<IActionResult>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessDonation(DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context != null)
                {
                    // Convert ViewModel to Entity Model
                    var donation = new Donation
                    {
                        DonorId = "anonymous",
                        DonationType = model.DonationType,
                        Amount = model.DonationAmount,
                        PaymentMethod = model.PaymentMethod,
                        Supplies = model.Supplies ?? new List<string>(),
                        AdditionalSupplies = model.AdditionalSupplies ?? string.Empty,
                        DonationDate = DateTime.Now,
                        IsAnonymous = model.DonateAnonymously,
                        Status = "Completed"
                    };

                    _context.Add(donation);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ThankYou", new { id = donation.Id });
                }
                else
                {
                    // Fallback for when context is not available
                    return RedirectToAction("ThankYou", new { id = 1 });
                }
            }

            // Repopulate dropdown lists if validation fails
            ViewBag.DonationTypes = new[]
            {
                new SelectListItem { Value = "financial", Text = "Financial Donation" },
                new SelectListItem { Value = "supplies", Text = "Supplies Donation" },
                new SelectListItem { Value = "both", Text = "Both Financial and Supplies" }
            };

            ViewBag.PaymentMethods = new[]
            {
                new SelectListItem { Value = "creditcard", Text = "Credit Card" },
                new SelectListItem { Value = "paypal", Text = "PayPal" },
                new SelectListItem { Value = "banktransfer", Text = "Bank Transfer" }
            };

            ViewBag.SupplyItems = new[]
            {
                new SelectListItem { Value = "water", Text = "Bottled Water" },
                new SelectListItem { Value = "food", Text = "Non-perishable Food" },
                new SelectListItem { Value = "blankets", Text = "Blankets" },
                new SelectListItem { Value = "meds", Text = "Medical Supplies" },
                new SelectListItem { Value = "hygiene", Text = "Hygiene Kits" },
                new SelectListItem { Value = "clothing", Text = "Clothing" }
            };

            return View("Donate", model);
        }
    }
}



