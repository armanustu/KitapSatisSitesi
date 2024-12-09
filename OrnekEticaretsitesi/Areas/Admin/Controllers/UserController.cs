using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrnekEticaretsitesi.Areas.Admin.Models;
using OrnekEticaretsitesi.Data;

namespace OrnekEticaretsitesi.Areas.Admin.Controllers
{


    [Area("Admin")]
   
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //UserRoles ve Roles tablosu applicationdbContexte yok???????


            var users = _context.ApplicationUsers.ToList();
          
            var role = _context.Roles.ToList();//AspNet.Roles
            var userRol = _context.UserRoles.ToList();//AspNetUserRoles
            foreach (var item in users)
            {
                var roleId = userRol.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
                

            }

                return View(users);//Rol adı nerden gelir?????? users roladı yok???rol adını eşleştirmeyle ulaşıyoruz 
        }


        // GET: Admin/User/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.ApplicationUsers == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ApplicationUsers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




    }
}
