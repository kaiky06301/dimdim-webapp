using System.Diagnostics;
using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class HomeController(DimDimContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalClientes = await db.Clientes.CountAsync();
        ViewBag.TotalContas = await db.Contas.CountAsync();
        ViewBag.SaldoTotal = await db.Contas.SumAsync(c => (decimal?)c.Saldo) ?? 0m;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
