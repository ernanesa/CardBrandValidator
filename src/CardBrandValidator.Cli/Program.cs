using CardBrandValidator.Core.Domain;
using CardBrandValidator.Core.Services;
using Spectre.Console;

AnsiConsole.Clear();
RenderHeader();

bool keepRunning = true;
while (keepRunning)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold yellow]Escolha uma operação:[/]")
            .PageSize(10)
            .AddChoices([
                "🔍 Validar número de cartão digitado",
                "🧪 Bateria de testes das 11 bandeiras (4Devs + Elo)",
                "📊 Informações técnicas sobre a arquitetura",
                "🚪 Sair"
            ]));

    switch (choice)
    {
        case "🔍 Validar número de cartão digitado":
            ValidateCustomCard();
            break;

        case "🧪 Bateria de testes das 11 bandeiras (4Devs + Elo)":
            RunBrandShowcase();
            break;

        case "📊 Informações técnicas sobre a arquitetura":
            ShowTechnicalDetails();
            break;

        case "🚪 Sair":
            keepRunning = false;
            AnsiConsole.MarkupLine("[bold green]Obrigado por utilizar o CardBrandValidator! Até logo.[/]");
            break;
    }

    if (keepRunning)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[grey]Pressione qualquer tecla para retornar ao menu principal...[/]");
        Console.ReadKey(intercept: true);
        AnsiConsole.Clear();
        RenderHeader();
    }
}

static void RenderHeader()
{
    var rule = new Rule("[bold cyan]💳 CardBrandValidator .NET 10[/]")
    {
        Style = Style.Parse("cyan dim")
    };
    AnsiConsole.Write(rule);
    AnsiConsole.MarkupLine("[grey]Desafio DIO / Bootcamp TIVIT .NET & GitHub Copilot[/]");
    AnsiConsole.MarkupLine("[dim]Validador de Alta Performance • Zero Allocation Span • 100% Test Coverage[/]\n");
}

static void ValidateCustomCard()
{
    var input = AnsiConsole.Ask<string>("[bold white]Digite o número do cartão (pode conter espaços ou traços):[/]");
    var result = CardValidator.Validate(input);

    RenderCardResult(result);
}

static void RunBrandShowcase()
{
    AnsiConsole.MarkupLine("\n[bold cyan]Executando validação sobre o catálogo completo do 4Devs + Elo:[/]\n");

    (string BrandLabel, string CardNumber)[] showcase =
    [
        ("MasterCard (16 dígitos)", "5502 0983 2234 1104"),
        ("Visa (16 dígitos)", "4532 0151 1283 0366"),
        ("Visa (13 dígitos)", "4123 4567 8901 1"),
        ("American Express (15 dígitos)", "3782 822463 10005"),
        ("Diners Club (14 dígitos)", "3600 000000 0016"),
        ("Discover (16 dígitos)", "6011 0009 9013 9424"),
        ("JCB (16 dígitos)", "3528 0000 0000 0007"),
        ("HiperCard (16 dígitos)", "6062 8226 0000 0003"),
        ("Aura (16 dígitos)", "5000 0000 0000 0009"),
        ("enRoute (15 dígitos, s/ Luhn)", "2014 0000000 0000"),
        ("Voyager (15 dígitos, s/ Luhn)", "8699 0000000 0000"),
        ("Elo Nacional (16 dígitos)", "5067 2200 0000 0003")
    ];

    var table = new Table()
        .Border(TableBorder.Rounded)
        .AddColumn("[bold]Bandeira Testada[/]")
        .AddColumn("[bold]Número Formatado[/]")
        .AddColumn("[bold]Bandeira Detectada[/]")
        .AddColumn("[bold]Luhn Status[/]")
        .AddColumn("[bold]Veredito[/]");

    foreach (var item in showcase)
    {
        var res = CardValidator.Validate(item.CardNumber);

        string luhnStatus = res.IsLuhnValid ? "[green]✓ Válido[/]" : "[red]✗ Inválido[/]";
        string veredito = res.IsValid ? "[bold green]APROVADO[/]" : "[bold red]REJEITADO[/]";

        table.AddRow(
            $"[white]{item.BrandLabel}[/]",
            $"[cyan]{res.FormattedNumber}[/]",
            $"[yellow]{res.BrandName}[/]",
            luhnStatus,
            veredito
        );
    }

    AnsiConsole.Write(table);
}

static void RenderCardResult(CardValidationResult result)
{
    string borderColor = result.IsValid ? "green" : "red";
    string brandColor = result.Brand switch
    {
        CardBrand.Visa => "blue",
        CardBrand.MasterCard => "red",
        CardBrand.Amex => "cyan",
        CardBrand.Elo => "yellow",
        CardBrand.Hipercard => "maroon",
        CardBrand.Discover => "orange1",
        CardBrand.Diners => "aqua",
        CardBrand.Jcb => "green",
        _ => "grey"
    };

    var cardContent = new Markup(
        $"[bold {brandColor}]BANDEIRA:[/] [bold white]{result.BrandName.ToUpperInvariant()}[/]\n\n" +
        $"[bold white]{(string.IsNullOrEmpty(result.FormattedNumber) ? "----------------" : result.FormattedNumber)}[/]\n" +
        $"[grey]Mascarado (PCI-DSS): {result.MaskedNumber}[/]\n\n" +
        $"[dim]TITULAR:[/] [white]DEV DIO / TIVIT[/]    [dim]VALIDADE:[/] [white]12/30[/]\n" +
        $"[dim]STATUS LUHN:[/] {(result.IsLuhnValid ? "[green]✓ Consistente[/]" : "[red]✗ Inconsistente[/]")}\n" +
        $"[dim]STATUS FINAL:[/] {(result.IsValid ? "[bold green]CARTÃO VÁLIDO[/]" : $"[bold red]INVÁLIDO: {result.ErrorMessage}[/]")}"
    );

    var panel = new Panel(cardContent)
        .Header($"[bold {borderColor}] Cartão de Crédito [/]")
        .BorderColor(result.IsValid ? Color.Green : Color.Red)
        .RoundedBorder()
        .Padding(1, 1);

    AnsiConsole.WriteLine();
    AnsiConsole.Write(panel);
}

static void ShowTechnicalDetails()
{
    var grid = new Grid();
    grid.AddColumn();
    grid.AddRow(new Rule("[bold yellow]Destaques de Engenharia e Boas Práticas[/]"));
    grid.AddRow(new Markup(
        "• [bold green]Zero Heap Allocation:[/] Algoritmo de Luhn e Sanitização processados em [cyan]ReadOnlySpan<char>[/]\n" +
        "• [bold green]Regex Source Generators:[/] Expressões regulares compiladas em tempo de compilação via [cyan][GeneratedRegex][/]\n" +
        "• [bold green]100% Test Coverage:[/] Suíte com 100 testes unitários e de integração com xUnit e FluentAssertions\n" +
        "• [bold green]PCI-DSS by Design:[/] Mascaramento preservando apenas 4 primeiros e 4 últimos dígitos\n" +
        "• [bold green]Prioridade Desambiguada:[/] Elo avaliada antes de Visa e MasterCard para evitar colisões de IINs\n" +
        "• [bold green]Minimal API REST:[/] Exposição em endpoint HTTP com Swagger para integração externa\n"
    ));
    AnsiConsole.Write(new Panel(grid).RoundedBorder());
}
