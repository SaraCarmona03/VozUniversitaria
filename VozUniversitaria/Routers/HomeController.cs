using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VozUniversitaria.Models;

namespace VozUniversitaria.Routers;

/// <summary>Atiende las páginas principales y las respuestas de error de la aplicación.</summary>
public class HomeController : Controller
{
    /// <summary>Muestra el inicio, disponible solo para usuarios autenticados.</summary>
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>Muestra la página informativa de privacidad.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Muestra la página de error sin almacenar la respuesta en caché.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
