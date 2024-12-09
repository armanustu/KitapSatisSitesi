﻿using Microsoft.AspNetCore.Mvc;
using OrnekEticaretsitesi.Data;

namespace OrnekEticaretsitesi.Areas.Admin.ViewComponents
{
    public class CategoryList2 : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        public CategoryList2(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var category = _db.Categories.ToList();
            return View(category);
        }

    }
}