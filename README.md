# Prueba_crud
Proyecto desarrollado como parte de una prueba técnica para un puesto de desarrollador .NET Core. Consiste en una API RESTful utilizando ASP.NET Core 8, MySQL y Dapper
Tecnologías usadas:
- ASP.NET Core 8 Web API
- Dapper (micro ORM)
- MySQL (base de datos relacional)
- Postman (documentación y pruebas)
- Hugging Face API (IA)
- FluentValidation (validaciones)
- Middleware personalizado (manejo de errores)

Configuración y ejecución
1. Clonar el repositorio

```bash```
git clone https://github.com/MaribelPilquinao/PruebaTecnica-API.git

2. Configurar la base de datos

Crea una base de datos llamada pruebatecnica en tu servidor MySQL:
CREATE DATABASE pruebatecnica;
Script privados, pero son basicos de clientes, productos y pedidos

3. Configurar la cadena de conexión
Edita el archivo appsettings.Development.json (no incluido en Git).
  Ejemplo:
    {
  "ConnectionStrings": {
    "Default": "server=localhost;port=3306;database=prueba;user=root;password=0000"
      }
    }


Endpoints principales
Productos
GET /api/productos → Listar todos

GET /api/productos/{id} → Obtener por ID

POST /api/productos → Crear producto

PUT /api/productos/{id} → Actualizar

DELETE /api/productos/{id} → Eliminar

Clientes
GET /api/clientes

POST /api/clientes etc.

Pedidos
POST /api/pedidos
 - Valida stock
 - Calcula total
 - Descuenta stock automáticamente
 - Retorna ID del pedido creado

Autor
Maribel Pilquinao
