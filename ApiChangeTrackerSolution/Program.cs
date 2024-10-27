public class Program
{
    public static async Task Main(string[] args)
    {
        var swaggerUrl = "https://myapp.com/docs/api-docs.json";
        var exporter = new SwaggerExporter();
        await exporter.ExportSwaggerJsonAsync(swaggerUrl);
    }
}
