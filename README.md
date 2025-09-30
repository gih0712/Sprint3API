# Sprint3API
API para gerenciamento de motos no pátio da Mottu, com CORS e Swagger

# Mottu API

## Integrantes
- Giulia Corrêa Camillo RM554473
- Caroline de Oliveira RM559123

## Justificativa da Arquitetura
Domínio: Gerenciamento de motos no pátio da Mottu, com classificação por cores. Entidades: Motos, Colaboradores, Alertas. Usa .NET 8 Minimal API por simplicidade e performance. Armazenamento in-memory para protótipo. Inclui paginação, HATEOAS, status codes RESTful, e CORS para requisições externas.

## Instruções de Execução
1. Clone o repositório: git clone https://github.com/seu-usuario/Sprint3API.git.
2. Entre na pasta: cd Sprint3API.
3. Restaure dependências: dotnet restore.
4. Execute: dotnet run.
5. Acesse Swagger: http://localhost:50
