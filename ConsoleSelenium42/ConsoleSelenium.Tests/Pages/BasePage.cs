using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests.Pages;

/// <summary>
/// Base dos Page Objects: guarda o driver e oferece esperas explícitas reutilizáveis.
/// </summary>
public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver, int timeoutEmSegundos = 15)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutEmSegundos));
        // WebDriverException cobre respostas vazias do Chrome durante a troca de página (ex.: ler Driver.Url).
        Wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(StaleElementReferenceException),
            typeof(ElementClickInterceptedException),
            typeof(WebDriverException));
    }

    public string Titulo => Driver.Title;

    public string Url => Driver.Url;

    protected IWebElement AguardarVisivel(By localizador) =>
        Wait.Until(d =>
        {
            var elemento = d.FindElement(localizador);
            return elemento.Displayed ? elemento : null;
        })!;

    protected IWebElement AguardarClicavel(By localizador) =>
        Wait.Until(d =>
        {
            var elemento = d.FindElement(localizador);
            return elemento.Displayed && elemento.Enabled ? elemento : null;
        })!;

    protected IReadOnlyList<IWebElement> ObterVisiveis(By localizador) =>
        Driver.FindElements(localizador)
              .Where(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text))
              .ToList();

    protected void AguardarDocumentoCompleto() =>
        Wait.Until(d => ((IJavaScriptExecutor)d)
            .ExecuteScript("return document.readyState")?.ToString() == "complete");

    protected void ClicarComFallback(IWebElement elemento)
    {
        try
        {
            elemento.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", elemento);
        }
    }
}
