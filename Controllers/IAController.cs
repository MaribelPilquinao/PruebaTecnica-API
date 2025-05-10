using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Prueba_crud.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


[ApiController]
[Route("api/ia")]
public class IAController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public IAController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    [HttpPost("recomendar-con-ia")]
    public async Task<IActionResult> RecomendarConIA([FromBody] RecomendacionLibreRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        // extraer categoría y presupuesto desde la IA
        var prompt = $"Del siguiente texto: '{request.Descripcion}', extrae solo dos datos: " +
             $"la categoría (como una sola palabra) y el presupuesto en soles (si lo hay). " +
             $"Devuélvelo solo como JSON válido sin explicaciones, así: " +
             $"{{\"categoria\": \"...\", \"presupuesto\": 0 }}. Si no hay presupuesto, pon 999999.";


        var json = JsonSerializer.Serialize(new { input = prompt });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:5005/api/ai/response", content);
        if (!response.IsSuccessStatusCode)
            return StatusCode(500, "Error al comunicarse con el servicio de IA (extracción de datos).");

        var iaResponse = await response.Content.ReadAsStringAsync();
        var iaData = JsonSerializer.Deserialize<JsonElement>(iaResponse);
        var rawAnswer = iaData.GetProperty("answer");

        string categoria;
        decimal presupuesto;

        try
        {
            //texto plano de la IA
            var rawText = rawAnswer.ToString();

            // busca json 
            var match = Regex.Match(rawText, @"\{.*?\}", RegexOptions.Singleline);
            if (!match.Success)
                return BadRequest(new { mensaje = "La IA no devolvió un JSON válido." });

            var jsonSolo = match.Value;

            var parsed = JsonDocument.Parse(jsonSolo).RootElement;

            categoria = parsed.GetProperty("categoria").GetString() ?? string.Empty;

            var sinonimos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                 { "laptop", "laptops" },
                 { "laptops", "laptops" },
                 { "portátil", "laptops" },
                 { "audífonos", "perifericos" },
                 { "audifonos", "perifericos" },
                 { "auriculares", "perifericos" },
                 { "ratón", "perifericos" },
                 { "mouse", "perifericos" },
                 { "teclado", "perifericos" }
            };
            // normalizacion de categoría para q busque por sinonimos solo para pruebas
            if (sinonimos.TryGetValue(categoria, out var categoriaNormalizada))
            {
                categoria = categoriaNormalizada;
            }
            else
            {
                // si no encuentra la categoria
                return BadRequest(new { mensaje = $"La categoría '{categoria}' no es válida." });
            }


            var rawPres = parsed.GetProperty("presupuesto");

            if (rawPres.ValueKind == JsonValueKind.Number)
            {
                presupuesto = rawPres.GetDecimal();
            }
            else if (rawPres.ValueKind == JsonValueKind.String && decimal.TryParse(rawPres.GetString(), out var parsedPres))
            {
                presupuesto = parsedPres;
            }
            else
            {
                presupuesto = 999999;
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = "Error al procesar la respuesta de la IA.", detalle = ex.Message });
        }

        // busca productos desde la base de datos
        var productos = new List<string>();

        using (var connection = new MySqlConnection(_config.GetConnectionString("Default")))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SELECT Nombre FROM Productos WHERE Categoria = @cat AND Precio <= @pres", connection);
            command.Parameters.AddWithValue("@cat", categoria);
            command.Parameters.AddWithValue("@pres", presupuesto);

            using var reader = await command.ExecuteReaderAsync();
            int nombreIndex = reader.GetOrdinal("Nombre");
            while (await reader.ReadAsync())
            {
                productos.Add(reader.GetString(nombreIndex));
            }
        }

        if (productos.Count == 0)
            return NotFound(new { mensaje = "No se encontraron productos para esa categoría y presupuesto." });

        // consulta a la IA cual recomienda
        var recomendacionPrompt = $"Tengo estos productos: {string.Join(", ", productos)}. ¿Cuál recomendarías para: {request.Descripcion}? Solo responde con los nombres.";
        var recJson = JsonSerializer.Serialize(new { input = recomendacionPrompt });
        var recContent = new StringContent(recJson, Encoding.UTF8, "application/json");

        var finalResponse = await client.PostAsync("http://localhost:5005/api/ai/response", recContent);
        if (!finalResponse.IsSuccessStatusCode)
            return StatusCode(500, "Error al comunicarse con el servicio de IA (recomendación).");

        var finalResult = await finalResponse.Content.ReadAsStringAsync();
        return Content(finalResult, "application/json");
    }
}
