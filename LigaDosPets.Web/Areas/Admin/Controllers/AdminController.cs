using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public abstract class AdminController : Controller
    {
        // Base para todos os controllers da área admin
    }
}