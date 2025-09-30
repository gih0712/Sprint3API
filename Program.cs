using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;

namespace Sprint3API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configura CORS para permitir requisições de qualquer origem 
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()   // Permite qualquer origem
                      .AllowAnyMethod()   // Permite qualquer método (GET, POST, PUT, DELETE)
                      .AllowAnyHeader();  // Permite qualquer cabeçalho
            });
        });

        // Configura Swagger 
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Mottu API", 
                Version = "v1", 
                Description = "API para gerenciamento de motos no pátio da Mottu, com suporte a classificação por cores e alertas."
            });
            c.ExampleFilters(); 
        });
        builder.Services.AddSwaggerExamplesFromAssemblies(typeof(Program).Assembly);

        var app = builder.Build();

        // Habilita CORS com a política "AllowAll"
        app.UseCors("AllowAll");

        // Habilita Swagger na interface de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Dados in-memory 
        var motos = new List<Moto>();
        var colaboradores = new List<Colaborador>();
        var alertas = new List<Alerta>();
        var nextMotoId = 1;
        var nextColabId = 1;
        var nextAlertaId = 1;

        // Função para gerar links HATEOAS
        Dictionary<string, Link> GenerateLinks(string resource, int id)
        {
            return new Dictionary<string, Link>
            {
                { "self", new Link { Href = $"/{resource}/{id}", Rel = "self", Method = "GET" } },
                { "update", new Link { Href = $"/{resource}/{id}", Rel = "update", Method = "PUT" } },
                { "delete", new Link { Href = $"/{resource}/{id}", Rel = "delete", Method = "DELETE" } }
            };
        }

        // Endpoints para Motos
        app.MapGet("/motos", (int? page = 1, int? pageSize = 10) =>
        {
            var pagedMotos = motos.Skip(((page ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10);
            var response = pagedMotos.Select(m => new MotoResponse(m, GenerateLinks("motos", m.Id)));
            return Results.Ok(new PagedResponse<IEnumerable<MotoResponse>>(response, motos.Count, page ?? 1, pageSize ?? 10));
        })
        .WithName("GetMotos")
        .WithOpenApi(op => new(op)
        {
            Summary = "Lista motos paginadas",
            Description = "Retorna motos do pátio com paginação. Exemplo: /motos?page=2&pageSize=5",
            Parameters = [
                new() { Name = "page", Description = "Número da página (padrão: 1)", Schema = new() { Type = "integer" } },
                new() { Name = "pageSize", Description = "Itens por página (máx: 50)", Schema = new() { Type = "integer" } }
            ]
        });

        app.MapGet("/motos/{id}", (int id) =>
        {
            var moto = motos.FirstOrDefault(m => m.Id == id);
            if (moto == null) return Results.NotFound("Moto não encontrada");
            return Results.Ok(new MotoResponse(moto, GenerateLinks("motos", id)));
        })
        .WithName("GetMotoById")
        .WithOpenApi(op => new(op) 
        { 
            Summary = "Obtém moto por ID", 
            Description = "Retorna detalhes de uma moto específica com links HATEOAS." 
        });

        app.MapPost("/motos", ([FromBody] CreateMotoRequest request) =>
        {
            if (!request.IsValid()) return Results.BadRequest("Dados inválidos");
            var moto = new Moto 
            { 
                Id = nextMotoId++, 
                Placa = request.Placa, 
                Cor = request.Cor, 
                Status = request.Status, 
                TempoLimite = request.TempoLimite 
            };
            motos.Add(moto);
            return Results.Created($"/motos/{moto.Id}", new MotoResponse(moto, GenerateLinks("motos", moto.Id)));
        })
        .WithName("CreateMoto")
        .WithOpenApi(op => new(op)
        {
            Summary = "Cria uma nova moto",
            Description = "Adiciona uma moto ao pátio. Exemplo: { \"placa\": \"ABC-1234\", \"cor\": \"Verde\", \"status\": \"Pronta\", \"tempoLimite\": 0 }"
        })
        .Produces(201);

        app.MapPut("/motos/{id}", (int id, [FromBody] UpdateMotoRequest request) =>
        {
            var moto = motos.FirstOrDefault(m => m.Id == id);
            if (moto == null) return Results.NotFound("Moto não encontrada");
            if (!request.IsValid()) return Results.BadRequest("Dados inválidos");
            moto.Placa = request.Placa ?? moto.Placa;
            moto.Cor = request.Cor ?? moto.Cor;
            moto.Status = request.Status ?? moto.Status;
            moto.TempoLimite = request.TempoLimite ?? moto.TempoLimite;
            return Results.Ok(new MotoResponse(moto, GenerateLinks("motos", id)));
        })
        .WithName("UpdateMoto")
        .WithOpenApi(op => new(op) 
        { 
            Summary = "Atualiza uma moto", 
            Description = "Atualiza dados de uma moto existente. Campos opcionais." 
        });

        app.MapDelete("/motos/{id}", (int id) =>
        {
            var moto = motos.FirstOrDefault(m => m.Id == id);
            if (moto == null) return Results.NotFound("Moto não encontrada");
            motos.Remove(moto);
            return Results.NoContent();
        })
        .WithName("DeleteMoto")
        .WithOpenApi(op => new(op) 
        { 
            Summary = "Deleta uma moto", 
            Description = "Remove uma moto do pátio." 
        });

        // Endpoints para Colaboradores
        app.MapGet("/colaboradores", (int? page = 1, int? pageSize = 10) =>
        {
            var pagedColabs = colaboradores.Skip(((page ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10);
            var response = pagedColabs.Select(c => new ColaboradorResponse(c, GenerateLinks("colaboradores", c.Id)));
            return Results.Ok(new PagedResponse<IEnumerable<ColaboradorResponse>>(response, colaboradores.Count, page ?? 1, pageSize ?? 10));
        })
        .WithName("GetColaboradores")
        .WithOpenApi(op => new(op)
        {
            Summary = "Lista colaboradores paginados",
            Description = "Retorna colaboradores com paginação."
        });

        app.MapGet("/colaboradores/{id}", (int id) =>
        {
            var colab = colaboradores.FirstOrDefault(c => c.Id == id);
            if (colab == null) return Results.NotFound("Colaborador não encontrado");
            return Results.Ok(new ColaboradorResponse(colab, GenerateLinks("colaboradores", id)));
        })
        .WithName("GetColaboradorById")
        .WithOpenApi(op => new(op) { Summary = "Obtém colaborador por ID" });

        app.MapPost("/colaboradores", ([FromBody] CreateColaboradorRequest request) =>
        {
            if (!request.IsValid()) return Results.BadRequest("Dados inválidos");
            var colab = new Colaborador { Id = nextColabId++, Nome = request.Nome, Cargo = request.Cargo };
            colaboradores.Add(colab);
            return Results.Created($"/colaboradores/{colab.Id}", new ColaboradorResponse(colab, GenerateLinks("colaboradores", colab.Id)));
        })
        .WithName("CreateColaborador")
        .WithOpenApi(op => new(op)
        {
            Summary = "Cria um novo colaborador",
            Description = "Adiciona um colaborador. Exemplo: { \"nome\": \"João\", \"cargo\": \"Mecânico\" }"
        })
        .Produces(201);

        app.MapPut("/colaboradores/{id}", (int id, [FromBody] UpdateColaboradorRequest request) =>
        {
            var colab = colaboradores.FirstOrDefault(c => c.Id == id);
            if (colab == null) return Results.NotFound("Colaborador não encontrado");
            colab.Nome = request.Nome ?? colab.Nome;
            colab.Cargo = request.Cargo ?? colab.Cargo;
            return Results.Ok(new ColaboradorResponse(colab, GenerateLinks("colaboradores", id)));
        })
        .WithName("UpdateColaborador")
        .WithOpenApi(op => new(op) { Summary = "Atualiza um colaborador" });

        app.MapDelete("/colaboradores/{id}", (int id) =>
        {
            var colab = colaboradores.FirstOrDefault(c => c.Id == id);
            if (colab == null) return Results.NotFound("Colaborador não encontrado");
            colaboradores.Remove(colab);
            return Results.NoContent();
        })
        .WithName("DeleteColaborador")
        .WithOpenApi(op => new(op) { Summary = "Deleta um colaborador" });

        // Endpoints para Alertas
        app.MapGet("/alertas", (int? page = 1, int? pageSize = 10) =>
        {
            var pagedAlertas = alertas.Skip(((page ?? 1) - 1) * (pageSize ?? 10)).Take(pageSize ?? 10);
            var response = pagedAlertas.Select(a => new AlertaResponse(a, GenerateLinks("alertas", a.Id)));
            return Results.Ok(new PagedResponse<IEnumerable<AlertaResponse>>(response, alertas.Count, page ?? 1, pageSize ?? 10));
        })
        .WithName("GetAlertas")
        .WithOpenApi(op => new(op)
        {
            Summary = "Lista alertas paginados",
            Description = "Retorna alertas (ex: tempo excedido) com paginação."
        });

        app.MapGet("/alertas/{id}", (int id) =>
        {
            var alerta = alertas.FirstOrDefault(a => a.Id == id);
            if (alerta == null) return Results.NotFound("Alerta não encontrado");
            return Results.Ok(new AlertaResponse(alerta, GenerateLinks("alertas", id)));
        })
        .WithName("GetAlertaById")
        .WithOpenApi(op => new(op) { Summary = "Obtém alerta por ID" });

        app.MapPost("/alertas", ([FromBody] CreateAlertaRequest request) =>
        {
            if (!request.IsValid() || !motos.Any(m => m.Id == request.MotoId)) 
                return Results.BadRequest("Dados inválidos ou moto não encontrada");
            var alerta = new Alerta 
            { 
                Id = nextAlertaId++, 
                Descricao = request.Descricao, 
                MotoId = request.MotoId, 
                DataAlerta = DateTime.Now 
            };
            alertas.Add(alerta);
            return Results.Created($"/alertas/{alerta.Id}", new AlertaResponse(alerta, GenerateLinks("alertas", alerta.Id)));
        })
        .WithName("CreateAlerta")
        .WithOpenApi(op => new(op)
        {
            Summary = "Cria um novo alerta",
            Description = "Adiciona um alerta ligado a uma moto. Exemplo: { \"descricao\": \"Tempo excedido\", \"motoId\": 1 }"
        })
        .Produces(201);

        app.MapPut("/alertas/{id}", (int id, [FromBody] UpdateAlertaRequest request) =>
        {
            var alerta = alertas.FirstOrDefault(a => a.Id == id);
            if (alerta == null) return Results.NotFound("Alerta não encontrado");
            if (request.MotoId.HasValue && !motos.Any(m => m.Id == request.MotoId.Value))
                return Results.BadRequest("Moto não encontrada");
            alerta.Descricao = request.Descricao ?? alerta.Descricao;
            alerta.MotoId = request.MotoId ?? alerta.MotoId;
            return Results.Ok(new AlertaResponse(alerta, GenerateLinks("alertas", id)));
        })
        .WithName("UpdateAlerta")
        .WithOpenApi(op => new(op) { Summary = "Atualiza um alerta" });

        app.MapDelete("/alertas/{id}", (int id) =>
        {
            var alerta = alertas.FirstOrDefault(a => a.Id == id);
            if (alerta == null) return Results.NotFound("Alerta não encontrado");
            alertas.Remove(alerta);
            return Results.NoContent();
        })
        .WithName("DeleteAlerta")
        .WithOpenApi(op => new(op) { Summary = "Deleta um alerta" });

        app.Run();
    }
}