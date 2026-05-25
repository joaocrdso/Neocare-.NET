# Setup do Projeto Neocare

## Instalação Rápida

### 1. Pré-requisitos
- .NET 10 SDK
- SQL Server ou LocalDB (instalado automaticamente com Visual Studio 2025)
- MongoDB (opcional, para auditoria)

### 2. Restaurar Pacotes NuGet
```bash
cd Neocare
dotnet restore
cd ..
```

### 3. Configurar Connection Strings

Abra `Neocare/appsettings.json` e configure:

#### Para SQL Server (LocalDB) - Windows
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NeocareDb;Trusted_Connection=true;"
}
```

#### Para SQL Server - Específico
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=NeocareDb;User Id=sa;Password=YourPassword;Encrypt=false;"
}
```

#### Para MongoDB (local)
```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "neocare"
}
```

### 4. Criar Banco de Dados (Migrations)
```bash
cd Neocare
dotnet ef database update --project Neocare.csproj
cd ..
```

### 5. Executar Testes (opcional)
```bash
dotnet test
```

### 6. Rodar a Aplicação
```bash
cd Neocare
dotnet run
```

A aplicação estará disponível em: `https://localhost:5001`

Swagger estará em: `https://localhost:5001/swagger`

## Estrutura de Arquivos Importantes

```
Neocare/
├── API/Controllers/              # Controladores da API
├── Application/                  # Lógica de negócio
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── Domain/                       # Entidades e interfaces de domínio
│   ├── Entities/
│   └── Interfaces/
├── Infrastructure/               # Persistência e configurações
│   ├── Data/
│   ├── Repositories/
│   ├── Persistence/
│   ├── HealthChecks/
│   └── Middleware/
├── Program.cs                    # Configuração e injeção de dependências
└── appsettings.json             # Configurações da aplicação

Neocare.Tests/
├── Unit/                         # Testes unitários
│   ├── Domain/
│   └── Services/
└── Integration/                  # Testes de integração
    └── API/
```

## Comandos Úteis

### Atualizar Migrations
```bash
dotnet ef migrations add NomeDaMigracao --project Neocare.csproj
dotnet ef database update --project Neocare.csproj
```

### Remover Última Migration
```bash
dotnet ef migrations remove --project Neocare.csproj
```

### Reverter Banco para Versão Anterior
```bash
dotnet ef database update NomeDaMigracaoAnterior --project Neocare.csproj
```

### Limpar Banco (Development Only)
```bash
dotnet ef database drop --project Neocare.csproj
dotnet ef database update --project Neocare.csproj
```

### Executar Testes com Saída Verbosa
```bash
dotnet test --verbosity=detailed
```

### Executar Um Teste Específico
```bash
dotnet test --filter "FullyQualifiedName~PatientServiceTests.CreatePatient"
```

## Troubleshooting

### Erro: "Cannot connect to database"
- Verifique se SQL Server está rodando
- Verifique a connection string em `appsettings.json`
- Tente criar o banco manualmente via SQL Server Management Studio

### Erro: "Migration already applied"
- Execute `dotnet ef database drop` para resetar (apenas desenvolvimento)
- Ou especifique a migration correta: `dotnet ef database update NomeDaMigracao`

### Erro: "JWT key not set"
- A chave padrão está em `appsettings.json` no campo `JwtSettings:SecretKey`
- Em produção, use variáveis de ambiente ou Azure Key Vault

### MongoDB não conecta
- MongoDB é opcional apenas para auditoria
- A aplicação funcionará normalmente sem auditoria se MongoDB não estiver disponível
- Para instalar MongoDB localmente, veja: https://docs.mongodb.com/manual/installation/

## Variáveis de Ambiente

Pode-se usar variáveis de ambiente para sobrescrever `appsettings.json`:

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=...;Database=...;"
$env:JwtSettings__SecretKey = "sua-chave-secreta-aqui"

# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"
export JwtSettings__SecretKey="sua-chave-secreta-aqui"
```

## Deployment

Para preparar a aplicação para produção:

1. Defina as variáveis de ambiente necessárias
2. Compile em modo Release: `dotnet build -c Release`
3. Publique: `dotnet publish -c Release -o ./publish`
4. Configure o servidor web (IIS, nginx, etc.)

Mais informações em: https://docs.microsoft.com/en-us/dotnet/core/deploying/
