namespace OsuApiClient.Configurations.Interfaces;

/// <summary>
/// Interfaces configuration values for the osu! API client
/// </summary>
public interface IOsuClientConfiguration
{
    /// <summary>
    /// Gets or sets the client id
    /// </summary>
    long ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret
    /// </summary>
    string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the client redirect url
    /// </summary>
    string RedirectUrl { get; set; }
}
