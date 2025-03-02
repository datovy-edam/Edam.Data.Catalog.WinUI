namespace Edam.UI.Catalog;

// -----------------------------------------------------------------------------

public record AppConfig(
    string? Environment,
    string? DefaultConnectionString,
    string? CatalogServiceBaseUri
);
