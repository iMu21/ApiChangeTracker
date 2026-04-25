# ApiChangeTracker

A small .NET console tool that polls a Swagger / OpenAPI document URL and writes a new versioned snapshot to disk **only when the content has actually changed**, using a SHA-256 hash of the JSON to detect drift.

Useful for tracking how a remote API surface evolves over time when you don't own the upstream service and want a local audit trail.

## How it works

1. Fetch the configured `swaggerUrl`.
2. Hash the response body.
3. Compare against the hash of the most-recent file in `ApiDocJsons/`.
4. If different, write a new file with the next serial number; otherwise skip.

## Usage

Edit the URL in `Program.cs`:

```csharp
var swaggerUrl = "https://your-api.example.com/swagger/v1/swagger.json";
```

Then:

```bash
dotnet run
```

Snapshots land in the `ApiDocJsons/` folder next to the executable.
