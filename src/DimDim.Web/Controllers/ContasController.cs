using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class ContasController(DimDimContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var contas = await db.Contas.Include(c => c.Cliente).OrderBy(c => c.Agencia).ThenBy(c => c.Numero).ToListAsync();
        return View(contas);
    }

    public async Task<IActionResult> Details(int id)
    {
        var conta = await db.Contas.Include(c => c.Cliente).FirstOrDefaultAsync(c => c.Id == id);
        return conta is null ? NotFound() : View(conta);
    }

    public async Task<IActionResult> Create(int? clienteId)
    {
        await CarregarListasAsync();
        return View(new Conta { ClienteId = clienteId ?? 0, Numero = await ProximoNumeroAsync() });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Agencia,Numero,Tipo,Saldo,ClienteId")] Conta conta)
    {
        await ValidarAsync(conta, null);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return View(conta);
        }

        conta.DataAbertura = DateTime.UtcNow;
        db.Contas.Add(conta);
        await db.SaveChangesAsync();
        TempData["Msg"] = $"Conta {conta.Agencia}/{conta.Numero} aberta (ID {conta.Id}).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var conta = await db.Contas.FindAsync(id);
        if (conta is null) return NotFound();
        await CarregarListasAsync();
        return View(conta);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Agencia,Numero,Tipo,Saldo,ClienteId")] Conta form)
    {
        var conta = await db.Contas.FindAsync(id);
        if (conta is null) return NotFound();
        await ValidarAsync(form, id);
        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return View(form);
        }

        conta.Agencia = form.Agencia;
        conta.Numero = form.Numero;
        conta.Tipo = form.Tipo;
        conta.Saldo = form.Saldo;
        conta.ClienteId = form.ClienteId;
        await db.SaveChangesAsync();
        TempData["Msg"] = $"Conta {conta.Agencia}/{conta.Numero} atualizada.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var conta = await db.Contas.Include(c => c.Cliente).FirstOrDefaultAsync(c => c.Id == id);
        return conta is null ? NotFound() : View(conta);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var conta = await db.Contas.FindAsync(id);
        if (conta is not null)
        {
            db.Contas.Remove(conta);
            await db.SaveChangesAsync();
            TempData["Msg"] = $"Conta {conta.Agencia}/{conta.Numero} encerrada.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarAsync(Conta conta, int? id)
    {
        if (!await db.Clientes.AnyAsync(c => c.Id == conta.ClienteId))
            ModelState.AddModelError(nameof(Conta.ClienteId), "Escolha um cliente válido");
        if (!Conta.Tipos.Contains(conta.Tipo))
            ModelState.AddModelError(nameof(Conta.Tipo), "Tipo inválido");
        if (await db.Contas.AnyAsync(c => c.Agencia == conta.Agencia && c.Numero == conta.Numero && c.Id != id))
            ModelState.AddModelError(nameof(Conta.Numero), "Já existe essa conta nessa agência");
    }

    private async Task CarregarListasAsync()
    {
        var clientes = await db.Clientes.OrderBy(c => c.Nome).ToListAsync();
        ViewBag.Clientes = new SelectList(clientes, nameof(Cliente.Id), nameof(Cliente.Nome));
        ViewBag.Tipos = new SelectList(Conta.Tipos);
    }

    private async Task<string> ProximoNumeroAsync()
    {
        var total = await db.Contas.CountAsync();
        return $"{10001 + total}-{Random.Shared.Next(0, 10)}";
    }
}
