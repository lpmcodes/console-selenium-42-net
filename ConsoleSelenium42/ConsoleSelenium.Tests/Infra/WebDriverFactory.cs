using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleSelenium.Tests.Infra;

/// <summary>
/// Centraliza a criação e configuração do WebDriver usado pelos testes.
/// O Selenium Manager (embutido no Selenium 4) baixa o ChromeDriver compatível automaticamente.
/// Para rodar sem abrir janela, defina a variável de ambiente HEADLESS=true.
/// </summary>
public static class WebDriverFactory
{
    private static readonly TimeSpan TempoCarregamentoPagina = TimeSpan.FromSeconds(30);

    public static IWebDriver CriarChrome()
    {
        var options = new ChromeOptions();
        options.AddArgument("--window-size=1366,900");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");

        if (ExecutarSemInterface())
        {
            options.AddArgument("--headless=new");
        }

        IWebDriver driver = new ChromeDriver(options);
        driver.Manage().Timeouts().PageLoad = TempoCarregamentoPagina;
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero; // usamos apenas esperas explícitas

        return driver;
    }

    private static bool ExecutarSemInterface() =>
        string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "true", StringComparison.OrdinalIgnoreCase);
}
