using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers.Api;

public record ClienteRequest(string Nome, string Cpf, string Email, string? Telefone);
public record ClienteResponse(int Id, string Nome, string Cpf, string Email, string? Telefone, DateTime DataCadastro, int QtdContas);

/// <summary>Mesmo CRUD da tela de Clientes, exposto em JSON (GET, POST, PUT, DELETE).</summary>
[ApiController]
[Route("api/clientes")]
public class ClientesApiController(DimDimContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ClienteResponse>> Listar() =>
        await db.Clientes.OrderBy(c => c.Id)
            .Select(c => new ClienteResponse(c.Id, c.Nome, c.Cpf, c.Email, c.Telefone, c.DataCadastro, c.Contas.Count))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResponse>> Buscar(int id)
    {
        var c = await db.Clientes.Where(x => x.Id == id)
            .Select(c => new ClienteResponse(c.Id, c.Nome, c.Cpf, c.Email, c.Telefone, c.DataCadastro, c.Contas.Count))
            .FirstOrDefaultAsync();
        return c is null ? NotFound() : c;
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Criar(ClienteRequest req)
    {
        var cliente = new Cliente { Nome = req.Nome, Cpf = req.Cpf, Email = req.Email, Telefone = req.Telefone };
        if (!TryValidateModel(cliente)) return ValidationProblem(ModelState);
        if (await db.Clientes.AnyAsync(c => c.Cpf == req.Cpf)) return Conflict(new { erro = "CPF já cadastrado" });

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Buscar), new { id = cliente.Id },
            new ClienteResponse(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Telefone, cliente.DataCadastro, 0));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ClienteRequest req)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();
        if (await db.Clientes.AnyAsync(c => c.Cpf == req.Cpf && c.Id != id)) return Conflict(new { erro = "CPF já cadastrado" });

        cliente.Nome = req.Nome;
        cliente.Cpf = req.Cpf;
        cliente.Email = req.Email;
        cliente.Telefone = req.Telefone;
        if (!TryValidateModel(cliente)) return ValidationProblem(ModelState);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();
        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
