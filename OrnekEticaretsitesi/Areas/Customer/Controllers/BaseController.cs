using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrnekEticaretsitesi.Areas.Customer.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

    }
}
