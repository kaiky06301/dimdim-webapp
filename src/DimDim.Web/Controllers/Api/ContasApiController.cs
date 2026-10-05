using DimDim.Web.Data;
using DimDim.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DimDim.Web.Controllers.Api;

public record ContaRequest(string Agencia, string Numero, string Tipo, decimal Saldo, int ClienteId);
public record ContaResponse(int Id, string Agencia, string Numero, string Tipo, decimal Saldo, DateTime DataAbertura, int ClienteId, string? ClienteNome);

/// <summary>Mesmo CRUD da tela de Contas, exposto em JSON (GET, POST, PUT, DELETE).</summary>
[ApiController]
[Route("api/contas")]
public class ContasApiController(DimDimContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ContaResponse>> Listar() =>
        await db.Contas.OrderBy(c => c.Id)
            .Select(c => new ContaResponse(c.Id, c.Agencia, c.Numero, c.Tipo, c.Saldo, c.DataAbertura, c.ClienteId, c.Cliente!.Nome))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContaResponse>> Buscar(int id)
    {
        var c = await db.Contas.Where(x => x.Id == id)
            .Select(c => new ContaResponse(c.Id, c.Agencia, c.Numero, c.Tipo, c.Saldo, c.DataAbertura, c.ClienteId, c.Cliente!.Nome))
            .FirstOrDefaultAsync();
        return c is null ? NotFound() : c;
    }

    [HttpPost]
    public async Task<ActionResult<ContaResponse>> Criar(ContaRequest req)
    {
        var erro = await ValidarAsync(req, null);
        if (erro is not null) return BadRequest(new { erro });

        var conta = new Conta { Agencia = req.Agencia, Numero = req.Numero, Tipo = req.Tipo, Saldo = req.Saldo, ClienteId = req.ClienteId };
        if (!TryValidateModel(conta)) return ValidationProblem(ModelState);
        db.Contas.Add(conta);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Buscar), new { id = conta.Id }, (await Buscar(conta.Id)).Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ContaRequest req)
    {
        var conta = await db.Contas.FindAsync(id);
        if (conta is null) return NotFound();
        var erro = await ValidarAsync(req, id);
        if (erro is not null) return BadRequest(new { erro });

        conta.Agencia = req.Agencia;
        conta.Numero = req.Numero;
        conta.Tipo = req.Tipo;
        conta.Saldo = req.Saldo;
        conta.ClienteId = req.ClienteId;
        if (!TryValidateModel(conta)) return ValidationProblem(ModelState);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var conta = await db.Contas.FindAsync(id);
        if (conta is null) return NotFound();
        db.Contas.Remove(conta);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> ValidarAsync(ContaRequest req, int? id)
    {
        if (!await db.Clientes.AnyAsync(c => c.Id == req.ClienteId)) return "Cliente não encontrado";
        if (!Conta.Tipos.Contains(req.Tipo)) return "Tipo deve ser CORRENTE, POUPANCA ou SALARIO";
        if (await db.Contas.AnyAsync(c => c.Agencia == req.Agencia && c.Numero == req.Numero && c.Id != id))
            return "Já existe essa conta nessa agência";
        return null;
    }
}
