﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UdemyRabbitMQWeb.ExcelCreate.Models;
using UdemyRabbitMQWeb.ExcelCreate.Services;

namespace UdemyRabbitMQWeb.ExcelCreate.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductsController(AppDbContext context, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel() 
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating,
            };

            await _context.UserFiles.AddAsync(userFile);

            await _context.SaveChangesAsync();

            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage() {
                FileId = userFile.Id,
            });

            // rabbitMQ'ya mesaj gönder
            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));


        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userFiles = await _context.UserFiles
                .Where(f => f.UserId == user.Id)
                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return View(userFiles);
        }
    }
}
