using Prueba_crud.Repositories;
using Prueba_crud.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//servicios de productos
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();

//servicios de clientes
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ClienteService>();

//servicio de pedidos
builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<PedidoService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

//Middleware de errores 404,405 y 500
app.Use(async (context, next) =>
{
    try
    {
        await next();

        if (context.Response.StatusCode == 404)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"mensaje\":\"Ruta no encontrada\"}");
        }
        else if (context.Response.StatusCode == 405)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"mensaje\":\"Método no permitido en esta ruta\"}");
        }
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"mensaje\":\"Error interno del servidor\",\"error\":\"{ex.Message}\"}}");
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
