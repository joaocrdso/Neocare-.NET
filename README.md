# NeoCare - Advanced Business Development with .NET

## 📋 Visão Geral

NeoCare é uma aplicação ASP.NET Core 8 com Razor Pages desenvolvida para gerenciar registros de estresse mental. A aplicação foi evoluída com monitoramento, observabilidade e testes automatizados.

## 🚀 Novas Funcionalidades

### 1. Monitoramento e Observabilidade

#### Health Checks
A aplicação implementa verificações de saúde através de endpoints dedicados:

- **`/health`** - Health check geral da API
- **`/health/ready`** - Health check de prontidão (readiness probe)

**Checks implementados:**
- ✅ Saúde da API
- ✅ Conectividade com banco de dados
- ✅ Disponibilidade de serviços externos

**Exemplo de uso:**
```bash
# Verificar saúde geral
curl http://localhost:5000/health

# Verificar prontidão da aplicação
curl http://localhost:5000/health/ready
```

#### Logging Estruturado com Serilog
O logging estruturado foi configurado para capturar informações detalhadas:

**Níveis de log implementados:**
- Information - Eventos informativos
- Warning - Avisos importantes
- Error - Erros da aplicação

**Características:**
- Correlação de requisições (RequestId automático)
- Saída para console e arquivo
- Formato estruturado (JSON)
- Rotação diária de logs

**Arquivo de logs:** `logs/neocare-YYYY-MM-DD.txt`

**Configuração em `appsettings.json`:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/neocare-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

#### Tracing e Métricas com OpenTelemetry
Implementação completa de distributed tracing e coleta de métricas:

**Instrumentações:**
- ASP.NET Core (requisições HTTP)
- Runtime (.NET)
- Camadas da aplicação

**Métricas coletadas:**
- Tempo de resposta das requisições
- Taxa de erros
- Uso de memória e CPU
- Requisições por segundo

**Exportador:** Console (desenvolvimento) - Configurável para Jaeger, Prometheus, etc.

---

## 🧪 Testes Automatizados

### Estrutura de Testes

```
Neocare.Tests/
├── Unit/
│   ├── Services/
│   │   └── StressEntryServiceTests.cs
│   └── Domain/
│       └── StressEntryEntityTests.cs
└── Integration/
    └── API/
        └── StressEntriesApiTests.cs
```

### Padrão AAA (Arrange-Act-Assert)

Todos os testes seguem o padrão AAA:
- **Arrange** - Preparar dados e mocks
- **Act** - Executar a ação a ser testada
- **Assert** - Validar resultados

### Testes Unitários (20 pts)

#### StressEntryService
- `SearchStressEntries_WithValidParams_ReturnsResults`
- `SearchStressEntries_WithMinStressLevel_FiltersCorrectly`
- `SearchStressEntries_WithSearchTerm_FiltersByDescription`
- `SearchStressEntries_SecondCall_UsesCachedResult`
- `GetByIdAsync_WithValidId_ReturnsStressEntry`
- `GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException`
- `CreateAsync_WithValidData_CreatesStressEntry`
- `UpdateStressEntry_WithValidData_UpdatesSuccessfully`
- `UpdateStressEntry_WithInvalidId_ReturnsNull`
- `DeleteStressEntry_WithValidId_DeletesSuccessfully`
- `DeleteStressEntry_WithInvalidId_ReturnsFalse`

#### StressEntry Entity
- `StressEntry_Creation_InitializesAllProperties`
- `StressEntry_WithDefaultValues_InitializesCorrectly`
- `StressEntry_SymptomsList_IsModifiable`
- `StressEntry_WithValidStressLevel_IsCreated`

**Ferramentas utilizadas:**
- xUnit - Framework de testes
- Moq - Mocking de dependências
- FluentAssertions - Assertions mais legíveis

### Testes de Integração (15 pts)

#### Health Checks Integration
- `HealthCheck_Get_ReturnsHealthy`
- `HealthCheckReady_Get_ReturnsHealthy`

#### GET /api/stress
- `GetStressEntries_WithoutParams_ReturnsSuccessWithPagination`
- `GetStressEntries_WithPagination_ReturnsPaginatedResults`
- `GetStressEntries_WithSortBy_ReturnsSortedResults`

#### POST /api/stress
- `CreateStressEntry_WithValidData_ReturnsCreatedWithLocation`
- `CreateStressEntry_MultipleEntries_AllAreStored`

#### PUT /api/stress/{id}
- `UpdateStressEntry_WithValidData_ReturnsOk`
- `UpdateStressEntry_WithNonExistentId_ReturnsNotFound`

#### DELETE /api/stress/{id}
- `DeleteStressEntry_WithValidId_ReturnsNoContent`
- `DeleteStressEntry_WithNonExistentId_ReturnsNotFound`

#### Error Handling
- `InvalidEndpoint_Returns404`
- `PostWithInvalidJson_ReturnsBadRequest`

**Ferramentas utilizadas:**
- WebApplicationFactory - Teste de integração com host real
- FluentAssertions - Assertions semânticas
- HttpClient - Requisições HTTP reais

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar apenas testes unitários
dotnet test --filter "Category=Unit" 2>/dev/null || dotnet test Neocare.Tests

# Executar apenas testes de integração
dotnet test --filter "Category=Integration" 2>/dev/null || dotnet test Neocare.Tests

# Executar com verbose
dotnet test -v normal

# Executar teste específico
dotnet test --filter "FullyQualifiedName=Neocare.Tests.Unit.Services.StressEntryServiceTests.SearchStressEntries_WithValidParams_ReturnsResults"
```

### Cobertura de Testes

Total de testes implementados: **+25 testes**

- Unit Tests: 15 testes
- Integration Tests: 10+ testes

---

## 📊 Arquitetura e Camadas

```
Neocare (ASP.NET Core 8)
├── Pages/
│   ├── StressEntries.cshtml.cs
│   ├── CreateStressEntry.cshtml.cs
│   └── ...
├── Application/
│   ├── Services/
│   │   └── StressEntryService.cs
│   └── DTOs/
│       ├── StressEntryDto.cs
│       ├── CreateStressEntryDto.cs
│       └── SearchParams.cs
├── Domain/
│   ├── Entities/
│   │   └── StressEntry.cs
│   └── Interfaces/
│       └── IStressEntryRepository.cs
└── Infrastructure/
    ├── Repositories/
    │   └── InMemoryStressEntryRepository.cs
    └── HealthChecks/
        ├── DatabaseHealthCheck.cs
        └── ExternalServiceHealthCheck.cs
```

---

## 🔌 Endpoints da API

### Health Checks
```http
GET /health              # Saúde geral
GET /health/ready        # Prontidão da aplicação
```

### Stress Entries
```http
GET    /api/stress              # Listar com paginação
POST   /api/stress              # Criar novo
PUT    /api/stress/{id}         # Atualizar
DELETE /api/stress/{id}         # Deletar

# Exemplo com parâmetros
GET /api/stress?page=1&pageSize=10&sortBy=level&sortDirection=desc&minStressLevel=5
```

### Swagger/OpenAPI
```
http://localhost:5000/api/docs
```

---

## 📦 Dependências Adicionadas

### Monitoramento e Observabilidade
```xml
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.0" />
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.1.0" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="OpenTelemetry" Version="1.7.0" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.7.0" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.7.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.7.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.7.0" />
```

### Testes
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.2" />
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
```

---

## 🏃 Executando a Aplicação

### Pré-requisitos
- .NET 8 SDK
- Visual Studio 2026 ou VS Code

### Passos

```bash
# 1. Clonar repositório
git clone https://github.com/joaocrdso/Neocare-.NET.git
cd Neocare-.NET

# 2. Restaurar dependências
dotnet restore

# 3. Compilar
dotnet build

# 4. Executar
dotnet run --project Neocare/Neocare.csproj

# 5. Aplicação estará em
# http://localhost:5000 ou https://localhost:5001
```

### Executar Testes
```bash
dotnet test
```

---

## 📝 Configuração de Logging

### Exemplo de uso em código

```csharp
using Serilog;

public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Executando ação importante");
        _logger.LogWarning("Aviso: verifique isto");
        _logger.LogError("Erro: algo deu errado");
    }
}
```

### Visualizar logs

```bash
# Logs do console (durante execução)
dotnet run

# Logs de arquivo
tail -f logs/neocare-2024-01-15.txt  # macOS/Linux
Get-Content logs/neocare-2024-01-15.txt -Tail 100 -Wait  # Windows PowerShell
```

---

## 📊 Observabilidade com OpenTelemetry

### Ativar Tracing
```csharp
// Já configurado em Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
    });
```

### Exportar para sistemas externos

**Jaeger (exemplo):**
```xml
<PackageReference Include="OpenTelemetry.Exporter.Jaeger" Version="1.7.0" />
```

**Prometheus (exemplo):**
```xml
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.7.0" />
```

---

## 🐛 Troubleshooting

### Testes não executam
```bash
# Limpar cache e reconstruir
dotnet clean
dotnet build
dotnet test
```

### Erros de Health Check
- Verificar se banco de dados está acessível
- Confirmar conectividade de rede
- Verificar logs em `logs/neocare-*.txt`

### Logging não funciona
- Verificar permissões de escrita na pasta `logs/`
- Confirmar configuração em `appsettings.json`
- Verificar pasta `bin/` para logs em desenvolvimento

---

## 📚 Recursos Adicionais

- [ASP.NET Core 8 Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Serilog Documentation](https://serilog.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)

---

## 👥 Autores

Desenvolvido como parte do curso "Advanced Business Development with .NET"

---

## 📄 Licença

Este projeto está sob a licença MIT.
