# Arquitetura do Neocare

## Visão Geral

Neocare segue a **Clean Architecture** com 4 camadas bem definidas, garantindo separação de responsabilidades, testabilidade e manutenibilidade.

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)              │
│  AuthController | PatientsController | etc.             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Application Layer (Services)               │
│  PatientService | AppointmentService | AuthService     │
│  DTOs | Mappers | Business Logic                        │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Domain Layer (Interfaces)                  │
│  IPatientRepository | IAppointmentRepository            │
│  Entities | Value Objects | Business Rules             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│           Infrastructure Layer (Implementation)         │
│  Repositories | DbContext | Database | External APIs   │
│  Health Checks | Logging | Middleware                   │
└─────────────────────────────────────────────────────────┘
```

## Camadas Detalhadas

### 1. **API Layer** (`/API/Controllers/`)
- **Responsabilidade:** Receber requisições HTTP e retornar respostas
- **Princípios:** REST, HATEOAS, Validação de entrada
- **Controllers:**
  - `AuthController` - Autenticação (register/login)
  - `PatientsController` - CRUD de pacientes
  - `HealthProfessionalsController` - CRUD de profissionais
  - `AppointmentsController` - CRUD de consultas
  - `TreatmentsController` - CRUD de tratamentos

**Exemplo:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;
    // GET, POST, PUT, DELETE com HATEOAS
}
```

### 2. **Application Layer** (`/Application/`)
- **Responsabilidade:** Orquestrar a lógica de negócio
- **Componentes:**
  - `Services/` - Implementação de use cases
  - `Interfaces/` - Contratos de serviços
  - `DTOs/` - Data Transfer Objects (Request/Response)

**Exemplo:**
```csharp
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IAuditLogRepository _auditRepository;
    
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        // Validar dados
        // Criar entidade
        // Persistir
        // Registrar auditoria
    }
}
```

### 3. **Domain Layer** (`/Domain/`)
- **Responsabilidade:** Regras de negócio, entidades, interfaces
- **Componentes:**
  - `Entities/` - Modelos de dados (Patient, Appointment, etc.)
  - `Interfaces/` - Contratos (IRepository<T>, específicas)

**Entidades:**
- `Patient` - Paciente com histórico médico
- `HealthProfessional` - Profissional de saúde com CRM
- `Appointment` - Consulta agendada
- `Treatment` - Tratamento para paciente

**Exemplo:**
```csharp
public class Patient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public ICollection<Appointment> Appointments { get; set; }
}
```

### 4. **Infrastructure Layer** (`/Infrastructure/`)
- **Responsabilidade:** Implementações técnicas (banco de dados, logging, etc.)
- **Componentes:**
  - `Data/` - Entity Framework Context
  - `Repositories/` - Acesso a dados (SQL Server)
  - `Persistence/` - MongoDB Context para auditoria
  - `HealthChecks/` - Verificação de saúde
  - `Middleware/` - Global Exception Handler

## Padrões de Design Implementados

### Repository Pattern
Abstração do acesso a dados através de interfaces:

```csharp
// Interface no Domain
public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email);
    Task<Patient?> GetByCPFAsync(string cpf);
}

// Implementação no Infrastructure
public class PatientRepository : IPatientRepository
{
    private readonly NeocareDbContext _context;
    // Implementação com Entity Framework
}
```

### Dependency Injection
Configuração centralizad no `Program.cs`:

```csharp
// Registrar repositórios
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Registrar serviços
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
```

### DTO Pattern
Separação entre modelos de API e modelos de banco de dados:

```csharp
// Input
public class CreatePatientDto { ... }

// Output
public class PatientDto { ... }

// Entidade (Domain)
public class Patient { ... }
```

### Middleware Pattern
Global Exception Handler centralizado:

```csharp
public class GlobalExceptionHandlerMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex); }
    }
}
```

## Fluxo de uma Requisição

1. **Requisição HTTP** chega ao `Controller`
2. **Controller** valida entrada e chama `Service`
3. **Service** implementa lógica de negócio e chama `Repository`
4. **Repository** acessa banco de dados via `DbContext` (Entity Framework)
5. **DbContext** converte para SQL e executa no **SQL Server**
6. **Repository** retorna dados ao **Service**
7. **Service** mapeia para **DTO** e registra auditoria no **MongoDB**
8. **Service** retorna dados ao **Controller**
9. **Controller** retorna resposta HTTP com **HATEOAS**

```
HTTP Request
     ↓
  Controller (validates input)
     ↓
  Service (applies business logic)
     ↓
  Repository (queries database)
     ↓
  Entity Framework
     ↓
  SQL Server / MongoDB
     ↓
  (reverse flow with mapped data)
     ↓
  HTTP Response + HATEOAS Links
```

## Tecnologias por Camada

| Camada | Tecnologias |
|--------|-------------|
| **API** | ASP.NET Core 10, Controllers, Middleware, JWT |
| **Application** | DTOs, Service Classes, Interfaces |
| **Domain** | Entity Framework Core, Entities |
| **Infrastructure** | SQL Server, MongoDB, Serilog, Health Checks |

## SOLID Principles

### S (Single Responsibility Principle)
- Cada classe tem uma única responsabilidade
- `PatientService` apenas gerencia pacientes
- `AppointmentService` apenas gerencia consultas

### O (Open/Closed Principle)
- Aberto para extensão: novos repositórios implementam `IRepository<T>`
- Fechado para modificação: não muda `NeocareDbContext`

### L (Liskov Substitution Principle)
- `PatientRepository` pode ser substituído por qualquer `IPatientRepository`
- Testes podem usar `Mock<IPatientRepository>`

### I (Interface Segregation Principle)
- `IRepository<T>` - operações genéricas
- `IPatientRepository` - operações específicas de paciente
- Não força implementar métodos desnecessários

### D (Dependency Inversion Principle)
- Controllers dependem de interfaces (`IPatientService`)
- Nunca de implementações concretas (`PatientService`)
- Injeção de dependência no `Program.cs`

## Testes

### Unitários
- Mockam repositórios com `Moq`
- Testam lógica de negócio isolada
- Padrão AAA (Arrange, Act, Assert)

```csharp
[Fact]
public async Task CreatePatient_WithValidData_ShouldCreateSuccessfully()
{
    // Arrange
    var mockRepo = new Mock<IPatientRepository>();
    var service = new PatientService(mockRepo.Object);
    
    // Act
    var result = await service.CreateAsync(...);
    
    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(...);
}
```

### Integração
- Testam fluxo completo: HTTP → Controller → Service → Repository → DB
- Usam `WebApplicationFactory` para simular servidor
- Verificam status codes e payloads reais

```csharp
[Fact]
public async Task CreatePatient_ShouldReturn201()
{
    // Arrange
    var client = _factory.CreateClient();
    var dto = new CreatePatientDto { ... };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/patients", dto);
    
    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

## Observabilidade

### Health Checks
```bash
GET /health
{
  "status": "Healthy",
  "checks": {
    "sqlserver": "Healthy",
    "mongodb": "Healthy"
  }
}
```

### Logging com Serilog
- Console em desenvolvimento
- Arquivo `logs/neocare-.log` com rotação diária
- Contexto enriquecido com metadados

```
[10:30:45 INF] Request started. GET /api/patients
[10:30:45 INF] Executing action PatientsController.GetAll
[10:30:47 INF] Executed action in 150ms
```

### Auditoria em MongoDB
Todas as operações (CREATE, UPDATE, DELETE) são registradas:

```json
{
  "action": "CREATE",
  "entity": "Patient",
  "entityId": "123e4567-e89b-12d3-a456-426614174000",
  "performedBy": "user@email.com",
  "timestamp": "2025-01-15T10:00:00Z"
}
```

## Escalabilidade e Performance

1. **Paginação** - Endpoints retornam dados paginados
2. **Health Checks** - Verificam saúde de dependências
3. **Logging Estruturado** - Serilog com buffering
4. **Async/Await** - Operações não bloqueantes
5. **Repository Pattern** - Facilita trocar de BD
6. **HATEOAS** - Links para navegação sem acoplamento

## Extensibilidade

Para adicionar nova entidade (ex: Prescrição):

1. **Domain:**
   ```csharp
   // Domain/Entities/Prescription.cs
   public class Prescription { ... }
   
   // Domain/Interfaces/IPrescriptionRepository.cs
   public interface IPrescriptionRepository : IRepository<Prescription> { ... }
   ```

2. **Application:**
   ```csharp
   // Application/DTOs/CreatePrescriptionDto.cs
   public class CreatePrescriptionDto { ... }
   
   // Application/Interfaces/IPrescriptionService.cs
   public interface IPrescriptionService { ... }
   
   // Application/Services/PrescriptionService.cs
   public class PrescriptionService : IPrescriptionService { ... }
   ```

3. **Infrastructure:**
   ```csharp
   // Infrastructure/Repositories/PrescriptionRepository.cs
   public class PrescriptionRepository : IPrescriptionRepository { ... }
   ```

4. **API:**
   ```csharp
   // API/Controllers/PrescriptionsController.cs
   [ApiController]
   [Route("api/[controller]")]
   public class PrescriptionsController : ControllerBase { ... }
   ```

5. **Program.cs:**
   ```csharp
   builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
   builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
   ```

---

**Documentação atualizada:** Janeiro de 2025
