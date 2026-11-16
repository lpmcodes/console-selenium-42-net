# console-selenium-42-net

Lista de exercícios — Garantia da Qualidade de Software (Gestão e Qualidade de Software).
Automação de cenários reais no [DuckDuckGo](https://duckduckgo.com) com **Selenium WebDriver**, **MSTest** e **.NET 10**.

## Estrutura

```
ConsoleSelenium42/
├── ConsoleSelenium/              # Projeto Console (principal)
└── ConsoleSelenium.Tests/        # Projeto de testes (MSTest)
    ├── Infra/
    │   ├── WebDriverFactory.cs   # cria e configura o ChromeDriver
    │   └── UrlHelper.cs          # leitura de parâmetros de URL e normalização de texto
    ├── Pages/                    # Page Objects
    │   ├── BasePage.cs
    │   ├── DuckDuckGoHomePage.cs
    │   ├── DuckDuckGoResultsPage.cs
    │   └── PaginaExterna.cs
    └── BuscaDesafioTest.cs       # os 3 cenários de teste
```

## Cenários

| # | Teste | O que valida |
|---|-------|--------------|
| 1 | `Exercicio1_PesquisaBasica_ValidaTermoNoTituloOuUrl` | Busca "Selenium WebDriver C#" com Enter e confere o termo no título/URL |
| 2 | `Exercicio2_Autocompletar_TerceiraSugestaoAbreResultados` | Digita "C# unit testing", espera as sugestões (WebDriverWait), clica na 3ª e confere a busca aberta |
| 3 | `Exercicio3_NavegacaoHistorico_VoltarRestauraResultados` | Busca "Automated Testing", abre o 1º resultado orgânico, valida a URL externa, volta (`Navigate().Back()`) e valida os resultados; extra: `Forward()` |

## Pré-requisitos

- .NET 10 SDK
- Google Chrome instalado (o Selenium Manager baixa o ChromeDriver automaticamente)

## Como executar

```bash
cd ConsoleSelenium42
dotnet restore
dotnet test
```

Rodar um cenário específico:

```bash
dotnet test --filter TestCategory=Exercicio2
```

Rodar sem abrir a janela do navegador (headless):

```bash
# Linux/macOS
HEADLESS=true dotnet test
# Windows PowerShell
$env:HEADLESS="true"; dotnet test
```
