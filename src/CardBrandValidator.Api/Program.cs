using CardBrandValidator.Core.Domain;
using CardBrandValidator.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CardBrandValidator API",
        Version = "v1",
        Description = "API REST de Alta Performance para Validação e Reconhecimento de Bandeiras de Cartão de Crédito. Desenvolvido para o Bootcamp TIVIT .NET & GitHub Copilot na DIO."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardBrandValidator API v1");
    c.RoutePrefix = string.Empty; // Inicia o Swagger direto na raiz /
});

app.MapPost("/api/cards/validate", (CardBrandValidator.Api.ValidateCardRequest request) =>
{
    var result = CardValidator.Validate(request.CardNumber);
    return result.IsValid ? Results.Ok(result) : Results.BadRequest(result);
})
.WithName("ValidateCard")
.WithSummary("Valida o número de um cartão de crédito, identifica a bandeira e verifica o Algoritmo de Luhn.")
.Produces<CardValidationResult>(StatusCodes.Status200OK)
.Produces<CardValidationResult>(StatusCodes.Status400BadRequest);

app.MapGet("/api/brands", () =>
{
    var brands = BrandDetector.GetAllSupportedBrands();
    return Results.Ok(brands);
})
.WithName("GetSupportedBrands")
.WithSummary("Retorna o catálogo de todas as 11 bandeiras suportadas e suas regras.");

app.Run();

namespace CardBrandValidator.Api
{
    public record ValidateCardRequest(string? CardNumber);
}
