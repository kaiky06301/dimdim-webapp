using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers;

public class ClientesController(DimDimContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var clientes = await db.Clientes.Include(c => c.Contas).OrderBy(c => c.Nome).ToListAsync();
        return View(clientes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var cliente = await db.Clientes.Include(c => c.Contas).FirstOrDefaultAsync(c => c.Id == id);
        return cliente is null ? NotFound() : View(cliente);
    }

    public IActionResult Create() => View(new Cliente());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Cpf,Email,Telefone")] Cliente cliente)
    {
        if (await db.Clientes.AnyAsync(c => c.Cpf == cliente.Cpf))
            ModelState.AddModelError(nameof(Cliente.Cpf), "Já existe cliente com esse CPF");
        if (!ModelState.IsValid) return View(cliente);

        cliente.DataCadastro = DateTime.UtcNow;
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        TempData["Msg"] = $"Cliente {cliente.Nome} cadastrado (ID {cliente.Id}).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cpf,Email,Telefone")] Cliente form)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();
        if (await db.Clientes.AnyAsync(c => c.Cpf == form.Cpf && c.Id != id))
            ModelState.AddModelError(nameof(Cliente.Cpf), "Já existe cliente com esse CPF");
        if (!ModelState.IsValid) return View(form);

        cliente.Nome = form.Nome;
        cliente.Cpf = form.Cpf;
        cliente.Email = form.Email;
        cliente.Telefone = form.Telefone;
        await db.SaveChangesAsync();
        TempData["Msg"] = $"Cliente {cliente.Nome} atualizado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await db.Clientes.Include(c => c.Contas).FirstOrDefaultAsync(c => c.Id == id);
        return cliente is null ? NotFound() : View(cliente);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is not null)
        {
            db.Clientes.Remove(cliente);
            await db.SaveChangesAsync();
            TempData["Msg"] = $"Cliente {cliente.Nome} excluído (e as contas dele).";
        }
        return RedirectToAction(nameof(Index));
    }
}
