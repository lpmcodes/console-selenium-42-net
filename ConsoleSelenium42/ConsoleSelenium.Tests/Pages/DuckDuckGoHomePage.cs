using OpenQA.Selenium;

namespace ConsoleSelenium.Tests.Pages;

/// <summary>
/// Page Object da página inicial do DuckDuckGo (campo de busca e autocompletar).
/// </summary>
public sealed class DuckDuckGoHomePage : BasePage
{
    public const string UrlBase = "https://duckduckgo.com";

    private static readonly By CampoPesquisa = By.Name("q");

    private static readonly By ItensSugestao = By.CssSelector(
        ".search-suggestions-list li, .sbox-suggestions li, [role='option']");

    public DuckDuckGoHomePage(IWebDriver driver) : base(driver) { }

    public DuckDuckGoHomePage Abrir()
    {
        Driver.Navigate().GoToUrl(UrlBase);
        AguardarVisivel(CampoPesquisa);
        return this;
    }

    public DuckDuckGoHomePage Digitar(string termo)
    {
        var campo = AguardarClicavel(CampoPesquisa);
        campo.Clear();
        campo.SendKeys(termo);
        return this;
    }

    public DuckDuckGoResultsPage Pesquisar(string termo)
    {
        Digitar(termo);
        Driver.FindElement(CampoPesquisa).SendKeys(Keys.Enter);
        return new DuckDuckGoResultsPage(Driver).AguardarCarregar();
    }

    /// <summary>Espera explícita até a lista dinâmica ter pelo menos <paramref name="quantidadeMinima"/> sugestões.</summary>
    public IReadOnlyList<string> AguardarSugestoes(int quantidadeMinima)
    {
        var itens = Wait.Until(_ =>
        {
            var visiveis = ObterVisiveis(ItensSugestao);
            return visiveis.Count >= quantidadeMinima ? visiveis : null;
        })!;

        return itens.Select(TextoDaSugestao).ToList();
    }

    /// <summary>Clica na sugestão da posição informada (começando em 1) e devolve o texto clicado.</summary>
    public string ClicarSugestao(int posicao)
    {
        // Repetimos a busca do elemento a cada tentativa, porque a lista é recriada pelo JavaScript.
        return Wait.Until(_ =>
        {
            var visiveis = ObterVisiveis(ItensSugestao);
            if (visiveis.Count < posicao)
            {
                return null;
            }

            var item = visiveis[posicao - 1];
            var texto = TextoDaSugestao(item);
            item.Click();
            return texto;
        })!;
    }

    private static string TextoDaSugestao(IWebElement item) =>
        item.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                 .FirstOrDefault() ?? string.Empty;
}
