using ConsoleSelenium.Tests.Infra;
using ConsoleSelenium.Tests.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace ConsoleSelenium.Tests;

[TestClass]
[DoNotParallelize] // um navegador por vez evita bloqueio anti-robô do DuckDuckGo
public sealed class BuscaDesafioTest
{
    private IWebDriver _driver = null!;

    [TestInitialize]
    public void Inicializar() => _driver = WebDriverFactory.CriarChrome();

    [TestCleanup]
    public void Finalizar()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    // Exercício 1: A Experiência de Pesquisa Básica e Validação de Título
    [TestMethod]
    [TestCategory("Exercicio1")]
    public void Exercicio1_PesquisaBasica_ValidaTermoNoTituloOuUrl()
    {
        const string termo = "Selenium WebDriver C#";

        var resultados = new DuckDuckGoHomePage(_driver)
            .Abrir()
            .Pesquisar(termo);

        var tituloContemTermo = resultados.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase);
        var urlContemTermo = resultados.TermoPesquisado.Equals(termo, StringComparison.OrdinalIgnoreCase);

        Assert.IsTrue(tituloContemTermo || urlContemTermo,
            $"Esperava o termo '{termo}' no título ou na URL. Título: '{resultados.Titulo}' | URL: '{resultados.Url}'");
    }

    // Exercício 2: Interagindo com o Autocompletar (Suggestions)
    [TestMethod]
    [TestCategory("Exercicio2")]
    public void Exercicio2_Autocompletar_TerceiraSugestaoAbreResultados()
    {
        const string termo = "C# unit testing";
        const int terceiraOpcao = 3;

        var home = new DuckDuckGoHomePage(_driver)
            .Abrir()
            .Digitar(termo); // sem Enter

        var sugestoes = home.AguardarSugestoes(quantidadeMinima: terceiraOpcao);
        Assert.IsGreaterThanOrEqualTo(terceiraOpcao, sugestoes.Count, "A lista de sugestões não exibiu ao menos 3 opções.");

        var sugestaoEscolhida = home.ClicarSugestao(terceiraOpcao);

        var resultados = new DuckDuckGoResultsPage(_driver).AguardarCarregar();

        Assert.AreEqual(
            UrlHelper.Normalizar(sugestaoEscolhida),
            UrlHelper.Normalizar(resultados.TermoPesquisado),
            $"A busca aberta não corresponde à sugestão clicada. URL: '{resultados.Url}'");
    }
}
