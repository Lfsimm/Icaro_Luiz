using LUIZ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");



app.MapPost("api/funcionario/cadastrar", ([FromBody] Funcionario funcionario, [FromServices] AppDbContext ctx) => {

    ctx.Funcionarios.Add(funcionario);
    ctx.SaveChanges();
    return Results.Created("", funcionario);

});

app.MapGet("api/funcionario/listar", ([FromServices]   AppDbContext ctx ) =>{

    if (ctx.Funcionarios.Any())
    {
        return Results.Ok(ctx.Funcionarios.ToList());
    }

    return Results.NotFound();

});

app.MapPost("api/folha/cadastrar", ([FromBody] Folha folha, [FromServices] AppDbContext ctx) => {

    var funcionarioid = folha.funcionarioId;
    var funcionario = ctx.Funcionarios.Find(funcionarioid);
    folha.funcionario = funcionario;
    ctx.Folhas.Add(folha);
    ctx.SaveChanges();
    return Results.Created("", folha);

});

app.MapGet("api/folha/listar", ([FromServices]   AppDbContext ctx ) =>{

    if (ctx.Folhas.Any())
    {
        return Results.Ok(ctx.Folhas.ToList());
    }

    return Results.NotFound();

});

app.MapGet("api/folha/buscar/{cpf}/{mes}/{ano}", ([FromRoute]  string cpf, [FromRoute] int mes , [FromRoute] int ano, [FromServices]   AppDbContext ctx ) =>{

    var funcionario = ctx.Funcionarios.Where(p => p.cpf.Contains(cpf)).ToList();
    var Folhas  = ctx.Folhas .Where(f => f.mes == mes && f.ano == ano && f.funcionarioId == funcionario[0].Id);
    return Folhas.Any() ? Results.Ok(Folhas) : Results.NotFound("Nenhuma folha encontrado.");
});

app.Run();
