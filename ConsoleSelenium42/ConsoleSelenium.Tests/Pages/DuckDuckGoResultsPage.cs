using ConsoleSelenium.Tests.Infra;
using OpenQA.Selenium;

namespace ConsoleSelenium.Tests.Pages;

/// <summary>
/// Page Object da página de resultados do DuckDuckGo.
/// </summary>
public sealed class DuckDuckGoResultsPage : BasePage
{
    private const string Dominio = "duckduckgo.com";

    // Resultados orgânicos (anúncios usam outro data-testid e ficam de fora).
    private static readonly By LinksOrganicos = By.CssSelector(
        "li[data-layout='organic'] a[data-testid='result-title-a'], " +
        "article[data-testid='result'] a[data-testid='result-title-a']");

    public DuckDuckGoResultsPage(IWebDriver driver) : base(driver) { }

    public string TermoPesquisado => UrlHelper.ObterParametro(Driver.Url, "q");

    public int QuantidadeResultados => ObterVisiveis(LinksOrganicos).Count;

    public DuckDuckGoResultsPage AguardarCarregar()
    {
        Wait.Until(d => UrlHelper.EhDominio(d.Url, Dominio) && !string.IsNullOrEmpty(TermoPesquisado));
        AguardarDocumentoCompleto();
        return this;
    }

    public DuckDuckGoResultsPage AguardarResultados()
    {
        Wait.Until(_ => QuantidadeResultados > 0);
        return this;
    }

    /// <summary>Clica no primeiro link orgânico externo e devolve o endereço dele.</summary>
    public string ClicarPrimeiroResultadoOrganico()
    {
        AguardarResultados();

        var link = Wait.Until(_ => ObterVisiveis(LinksOrganicos)
            .FirstOrDefault(l => !UrlHelper.EhDominio(l.GetDomProperty("href") ?? string.Empty, Dominio)))!;

        var href = link.GetDomProperty("href") ?? string.Empty;

        // Garante que o link abra na mesma aba, para o histórico (Voltar/Avançar) funcionar.
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].removeAttribute('target');", link);
        ClicarComFallback(link);

        return href;
    }
}
