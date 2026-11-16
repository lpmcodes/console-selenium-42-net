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

    // Exercício 3: Navegação por Histórico e Botões de Ação
    [TestMethod]
    [TestCategory("Exercicio3")]
    public void Exercicio3_NavegacaoHistorico_VoltarRestauraResultados()
    {
        const string termo = "Automated Testing";

        // 1. Busca inicial
        var resultados = new DuckDuckGoHomePage(_driver)
            .Abrir()
            .Pesquisar(termo)
            .AguardarResultados();

        // 2. Clica no primeiro resultado orgânico
        resultados.ClicarPrimeiroResultadoOrganico();

        // 3. Valida a página externa pela URL atual
        new PaginaExterna(_driver).AguardarCarregar();
        var urlExterna = _driver.Url;

        Assert.IsTrue(UrlHelper.EhUrlWebValida(urlExterna), $"URL externa inválida: '{urlExterna}'");
        Assert.IsFalse(UrlHelper.EhDominio(urlExterna, "duckduckgo.com"),
            $"O navegador deveria ter saído do DuckDuckGo. URL atual: '{urlExterna}'");

        // 4. Volta para a página de resultados
        _driver.Navigate().Back();

        // 5. Valida que os resultados foram restaurados
        var restaurada = new DuckDuckGoResultsPage(_driver)
            .AguardarCarregar()
            .AguardarResultados();

        Assert.IsTrue(UrlHelper.EhDominio(restaurada.Url, "duckduckgo.com"),
            $"Deveria ter voltado ao DuckDuckGo. URL atual: '{restaurada.Url}'");
        Assert.AreEqual(UrlHelper.Normalizar(termo), UrlHelper.Normalizar(restaurada.TermoPesquisado),
            "O termo da busca não foi preservado ao voltar.");
        Assert.IsGreaterThan(0, restaurada.QuantidadeResultados, "A lista de resultados não foi restaurada.");

        // Extra: botão Avançar leva de volta ao site externo
        _driver.Navigate().Forward();
        new PaginaExterna(_driver).AguardarCarregar();

        Assert.IsFalse(UrlHelper.EhDominio(_driver.Url, "duckduckgo.com"),
            "O botão Avançar deveria reabrir a página externa.");
    }
}
