using System.Net;

namespace ConsoleSelenium.Tests.Infra;

/// <summary>
/// Utilitários para ler e comparar URLs e textos de busca.
/// </summary>
public static class UrlHelper
{
    public static string ObterParametro(string url, string nome)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(uri.Query))
        {
            return string.Empty;
        }

        foreach (var par in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var partes = par.Split('=', 2);
            if (partes[0].Equals(nome, StringComparison.OrdinalIgnoreCase))
            {
                return partes.Length > 1 ? WebUtility.UrlDecode(partes[1]) : string.Empty;
            }
        }

        return string.Empty;
    }

    public static bool EhDominio(string url, string dominio) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && uri.Host.EndsWith(dominio, StringComparison.OrdinalIgnoreCase);

    public static bool EhUrlWebValida(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>Normaliza um texto para comparação: minúsculas e espaços simples.</summary>
    public static string Normalizar(string texto) =>
        string.Join(' ', texto.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
}
