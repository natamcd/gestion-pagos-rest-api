# Gestion Pagos REST API

API REST completa para gestión de pagos desarrollada con .NET 8, PostgreSQL y Clean Architecture, lista para deployar en Google Cloud Run.

## 🏗️ Arquitectura

El proyecto sigue Clean Architecture con 3 capas:

```
GestionPagos/
├── GestionPagos.Core/              # Entidades y contratos
│   ├── Entities/                   # Modelos de dominio
│   └── Interfaces/                 # Interfaces de servicios
├── GestionPagos.Infrastructure/    # Implementación de datos
│   ├── Data/
│   │   ├── AppDbContext.cs        # EF Core DbContext
│   │   └── Migrations/            # Migraciones de base de datos
│   └── DependencyInjection.cs     # Configuración de servicios
└── GestionPagos.Api/               # Capa de presentación
    ├── Controllers/                # Endpoints REST
    └── Program.cs                  # Configuración de la app
```

## 🗄️ Modelo de Datos

### Entidades

- **Usuario**: Gestión de usuarios del sistema
- **Transaccion**: Registro de transacciones financieras
- **Elemento**: Categorías de transacciones
- **Naturaleza**: Tipo de transacción (Ingreso/Egreso)

## 🚀 Desarrollo Local

### Prerrequisitos

- .NET 8 SDK
- PostgreSQL 14+
- Docker (opcional)

### Configuración

1. **Clonar el repositorio**
```bash
git clone https://github.com/natamcd/gestion-pagos-rest-api.git
cd gestion-pagos-rest-api
```

2. **Configurar la base de datos**

Editar `GestionPagos.Api/appsettings.json` con tu configuración local:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GestionPagos;Username=postgres;Password=TU_PASSWORD"
  }
}
```

3. **Restaurar paquetes**
```bash
dotnet restore
```

4. **Aplicar migraciones**
```bash
dotnet ef database update --project GestionPagos.Infrastructure --startup-project GestionPagos.Api
```

5. **Ejecutar la aplicación**
```bash
dotnet run --project GestionPagos.Api
```

La API estará disponible en: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

## 📋 Endpoints REST

### Usuarios
- `GET /api/usuarios` - Listar todos los usuarios
- `GET /api/usuarios/{id}` - Obtener usuario por ID
- `POST /api/usuarios` - Crear nuevo usuario
- `PUT /api/usuarios/{id}` - Actualizar usuario
- `DELETE /api/usuarios/{id}` - Eliminar usuario

### Transacciones
- `GET /api/transacciones` - Listar todas las transacciones
- `GET /api/transacciones/{id}` - Obtener transacción por ID
- `GET /api/transacciones/usuario/{usuarioId}` - Transacciones por usuario
- `POST /api/transacciones` - Crear nueva transacción
- `PUT /api/transacciones/{id}` - Actualizar transacción
- `DELETE /api/transacciones/{id}` - Eliminar transacción

### Elementos (Categorías)
- `GET /api/elementos` - Listar todos los elementos
- `GET /api/elementos/{id}` - Obtener elemento por ID
- `POST /api/elementos` - Crear nuevo elemento
- `PUT /api/elementos/{id}` - Actualizar elemento
- `DELETE /api/elementos/{id}` - Eliminar elemento

### Naturalezas (Tipos)
- `GET /api/naturalezas` - Listar todas las naturalezas
- `GET /api/naturalezas/{id}` - Obtener naturaleza por ID
- `POST /api/naturalezas` - Crear nueva naturaleza
- `PUT /api/naturalezas/{id}` - Actualizar naturaleza
- `DELETE /api/naturalezas/{id}` - Eliminar naturaleza

### Health Check
- `GET /health` - Estado de la aplicación

## 🐳 Docker

### Construir imagen
```bash
docker build -t gestion-pagos-api .
```

### Ejecutar con Docker
```bash
docker run -p 8080:8080 \
  -e DB_HOST=tu_host \
  -e DB_NAME=GestionPagos \
  -e DB_USER=postgres \
  -e DB_PASSWORD=tu_password \
  gestion-pagos-api
```

## ☁️ Deploy en Google Cloud Run

### Prerrequisitos en Google Cloud

1. **Crear base de datos PostgreSQL en Cloud SQL**
```bash
gcloud sql instances create gestion-pagos-db \
  --database-version=POSTGRES_14 \
  --tier=db-f1-micro \
  --region=us-central1
```

2. **Crear usuario y base de datos**
```bash
gcloud sql users create gp_app \
  --instance=gestion-pagos-db \
  --password=SECURE_PASSWORD

gcloud sql databases create GestionPagos \
  --instance=gestion-pagos-db
```

3. **Crear VPC Connector**
```bash
gcloud compute networks vpc-access connectors create gp-connector \
  --region=us-central1 \
  --range=10.8.0.0/28
```

4. **Configurar Secret Manager**
```bash
echo -n "SECURE_PASSWORD" | gcloud secrets create DB_PASSWORD --data-file=-
```

### Deploy Manual

```bash
# Construir y pushear imagen
gcloud builds submit --tag gcr.io/YOUR_PROJECT_ID/gestion-pagos-rest-api

# Deploy a Cloud Run
gcloud run deploy gestion-pagos-rest-api \
  --image gcr.io/YOUR_PROJECT_ID/gestion-pagos-rest-api \
  --region us-central1 \
  --platform managed \
  --vpc-connector gp-connector \
  --vpc-egress private-ranges-only \
  --set-env-vars DB_HOST=10.70.0.5,DB_NAME=GestionPagos,DB_USER=gp_app \
  --update-secrets DB_PASSWORD=DB_PASSWORD:latest \
  --allow-unauthenticated \
  --max-instances 10 \
  --memory 512Mi \
  --cpu 1 \
  --port 8080
```

### Deploy Automático con Cloud Build

Configurar el trigger en Cloud Build y ejecutar:

```bash
gcloud builds submit --config cloudbuild.yaml
```

**Nota:** Actualizar `cloudbuild.yaml` con tu PROJECT_ID de Google Cloud.

## 🔧 Migraciones

### Crear nueva migración
```bash
dotnet ef migrations add NombreMigracion \
  --project GestionPagos.Infrastructure \
  --startup-project GestionPagos.Api \
  --output-dir Data/Migrations
```

### Aplicar migraciones
```bash
dotnet ef database update \
  --project GestionPagos.Infrastructure \
  --startup-project GestionPagos.Api
```

### Revertir migración
```bash
dotnet ef migrations remove \
  --project GestionPagos.Infrastructure \
  --startup-project GestionPagos.Api
```

## 🔐 Seguridad

- ✅ Contraseñas almacenadas con hash
- ✅ CORS configurado
- ✅ Secrets manejados por Secret Manager en Cloud
- ✅ Conexión a base de datos via VPC privada
- ⚠️ **Producción**: Actualizar política CORS para dominios específicos

## 📦 Stack Tecnológico

- **Framework**: ASP.NET Core 8.0
- **Base de Datos**: PostgreSQL con EF Core 8
- **ORM**: Entity Framework Core 8.0
- **Documentación**: Swagger/OpenAPI
- **Contenedor**: Docker
- **Cloud**: Google Cloud Run + Cloud SQL
- **CI/CD**: Google Cloud Build

## 🧪 Testing

```bash
# Ejecutar tests (cuando se implementen)
dotnet test
```

## 📝 Notas Importantes

1. **Detección de Entorno**: La aplicación detecta automáticamente si se ejecuta en Cloud Run mediante la variable `K_SERVICE`
2. **Migraciones Automáticas**: En Cloud Run, las migraciones se aplican automáticamente al iniciar
3. **Logging**: Los logs se envían a Console para integración con Cloud Logging
4. **Health Check**: El endpoint `/health` permite monitoreo de la aplicación

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 👥 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request
