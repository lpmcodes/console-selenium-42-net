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
}
