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
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/neocare-.txt" } }
    ]
  }
}
```

### 2. Testes Automatizados

#### Como executar os testes
Para executar os testes automatizados, utilize o comando abaixo na raiz do projeto:
```bash
dotnet test
```

Os testes estão organizados em dois projetos:
- **Testes Unitários:** Localizados em `Neocare.Tests.Unit`.
- **Testes de Integração:** Localizados em `Neocare.Tests.Integration`.

#### Organização dos Testes
- **Padrão AAA:** Todos os testes seguem o padrão Arrange, Act, Assert.
- **Nomenclatura:** Os testes seguem o formato `MetodoTestado_Cenario_ResultadoEsperado`.
- **Fixtures:** Utilização de Fixtures e Collection Fixtures para compartilhar contexto entre testes.

### 3. Tracing e Métricas

#### OpenTelemetry
A aplicação utiliza OpenTelemetry para rastreamento distribuído e métricas:
- **Tracing:** Configurado com `AddAspNetCoreInstrumentation` e `AddConsoleExporter`.
- **Métricas:** Incluem `AddRuntimeInstrumentation` e `AddConsoleExporter`.

**Exemplo de configuração:**
```csharp
services.AddOpenTelemetryTracing(builder =>
{
    builder.AddAspNetCoreInstrumentation()
           .AddConsoleExporter();
});

services.AddOpenTelemetryMetrics(metrics =>
{
    metrics.AddAspNetCoreInstrumentation()
           .AddRuntimeInstrumentation()
           .AddConsoleExporter();
});
```

---

## 🛠️ Como Monitorar a Aplicação

1. **Health Checks:**
   - Acesse os endpoints `/health` e `/health/ready` para verificar a saúde da aplicação.

2. **Logs:**
   - Consulte os arquivos de log gerados em `logs/` para informações detalhadas.

3. **Tracing e Métricas:**
   - Utilize ferramentas compatíveis com OpenTelemetry para visualizar os traces e métricas da aplicação.
