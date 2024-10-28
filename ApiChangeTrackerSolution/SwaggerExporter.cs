using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SwaggerExporter
{
    private readonly HttpClient _httpClient;
    private readonly string _apiDocFolderPath;

    public SwaggerExporter()
    {
        var projectRootPath = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
        _apiDocFolderPath = Path.Combine(projectRootPath, "ApiDocJsons");

        Directory.CreateDirectory(_apiDocFolderPath);
        _httpClient = new HttpClient();
    }

    public async Task ExportSwaggerJsonAsync(string swaggerUrl)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(swaggerUrl);

            var currentHash = ComputeSha256Hash(response);

            var lastFileHash = GetLastFileHash(out int lastSerialNumber);

            if (currentHash == lastFileHash)
            {
                Console.WriteLine("No changes detected in the Swagger JSON.");
                return;
            }

            int newSerialNumber = lastSerialNumber + 1;
            string fileName = $"v{newSerialNumber}_{DateTime.Now:yyyyMMdd}.json";
            string filePath = Path.Combine(_apiDocFolderPath, fileName);

            await File.WriteAllTextAsync(filePath, response);
            Console.WriteLine($"Swagger JSON saved successfully: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting Swagger JSON: {ex.Message}");
        }
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private string GetLastFileHash(out int lastSerialNumber)
    {
        lastSerialNumber = 0;

        var files = Directory.GetFiles(_apiDocFolderPath)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.Name)
            .ToList();

        if (!files.Any()) return string.Empty;

        var lastFile = files.First();
        lastSerialNumber = int.Parse(lastFile.Name.Split('_')[0].Substring(1));

        var lastFileContent = File.ReadAllText(lastFile.FullName);
        return ComputeSha256Hash(lastFileContent);
    }
}
