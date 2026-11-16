using ConsoleSelenium.Tests.Infra;
using OpenQA.Selenium;

namespace ConsoleSelenium.Tests.Pages;

/// <summary>
/// Representa qualquer site externo aberto a partir de um resultado de busca.
/// </summary>
public sealed class PaginaExterna : BasePage
{
    public PaginaExterna(IWebDriver driver) : base(driver, timeoutEmSegundos: 30) { }

    public PaginaExterna AguardarCarregar()
    {
        Wait.Until(d => UrlHelper.EhUrlWebValida(d.Url) && !UrlHelper.EhDominio(d.Url, "duckduckgo.com"));
        AguardarDocumentoCompleto();
        return this;
    }
}
