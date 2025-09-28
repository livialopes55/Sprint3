# Mottu API — Sprint 3 (.NET)

API RESTful em .NET 8 para gestão de **Filiais**, **Pátios** e **Motos**, com **CRUD completo**, **paginação**, **HATEOAS** e **status codes adequados**, documentada com **Swagger**.

## Integrantes
- **Lívia de Oliveira Lopes** — RM 556281

## Objetivo
Prover endpoints REST para o domínio Mottu: **Filiais** possuem **Pátios** e **Pátios** possuem **Motos**. A API expõe operações de cadastro, consulta, atualização e exclusão, com paginação nas listagens e hipermídia (HATEOAS) nos retornos.

## Arquitetura & Tecnologias
- **.NET 8 — ASP.NET Core Web API**
- **Entity Framework Core + SQLite** (banco local criado automaticamente)
- **Swagger / OpenAPI**
- **API Versioning** (rota `/api/v1/...`)
- Camadas simples: **Controllers → DbContext/Models**
- Boas práticas REST:
  - Paginação (`pageNumber`, `pageSize`)
  - HATEOAS (links de `self`, `create`, `update`, `delete`)
  - Status codes: `200`, `201`, `204`, `400`, `404`

## Como executar
Pré-requisito: **.NET 8 SDK**

```bash
dotnet build
dotnet run --project MottuApi
```

A aplicação mostrará no console as URLs. Exemplos comuns:
- Swagger: `http://localhost:5000/swagger/index.html'
- Raiz redireciona para o Swagger (`/ → /swagger`)

> O banco **SQLite** (`mottu.db`) é criado automaticamente na primeira execução e uma carga mínima (seed) é aplicada.

## Testes rápidos (Swagger)
1. Abrir o **Swagger** (`/swagger`).
2. Executar os **GETs paginados** para Filiais/Pátios/Motos (`?pageNumber=1&pageSize=2`).
3. Executar **POST → GET por id → PUT → GET por id → DELETE** para cada entidade.
4. Conferir:
   - **HATEOAS**: em `GET por id` o corpo retorna `{ data, links }`.
   - **Status codes**: `201` no POST, `204` no PUT/DELETE, `404` para ids inexistentes, `400` para payload inválido.

## Endpoints

### Filiais
- `GET    /api/v1/filiais?pageNumber=1&pageSize=10` — lista paginada
- `GET    /api/v1/filiais/{id}` — detalhe (com HATEOAS)
- `POST   /api/v1/filiais` — cria (201 + Location)
- `PUT    /api/v1/filiais/{id}` — atualiza (204)
- `DELETE /api/v1/filiais/{id}` — exclui (204)

### Pátios
- `GET    /api/v1/patios?pageNumber=1&pageSize=10`
- `GET    /api/v1/patios/{id}`
- `POST   /api/v1/patios`
- `PUT    /api/v1/patios/{id}`
- `DELETE /api/v1/patios/{id}`

### Motos
- `GET    /api/v1/motos?pageNumber=1&pageSize=10`
- `GET    /api/v1/motos/{id}`
- `POST   /api/v1/motos`
- `PUT    /api/v1/motos/{id}`
- `DELETE /api/v1/motos/{id}`

## Exemplos de payloads

### Filial — POST
```json
{
  "nome": "Filial Zona Sul",
  "endereco": "Av. das Flores, 123"
}
```

### Filial — PUT
```json
{
  "id": 0,
  "nome": "Filial Zona Sul - Atualizada",
  "endereco": "Rua Nova, 55"
}
```

### Pátio — POST  *(use um `filialId` existente, ex.: 1)*
```json
{
  "descricao": "Pátio B",
  "dimensao": "40x20",
  "filialId": 1
}
```

### Pátio — PUT
```json
{
  "id": 0,
  "descricao": "Pátio B - Atualizado",
  "dimensao": "45x25",
  "filialId": 1
}
```

### Moto — POST  *(use um `patioId` existente, ex.: 1)*
```json
{
  "placa": "XYZ9A88",
  "modelo": "Fazer 250",
  "ano": 2023,
  "status": "Parada",
  "patioId": 1
}
```

### Moto — PUT
```json
{
  "id": 0,
  "placa": "XYZ9A88",
  "modelo": "Fazer 250 ABS",
  "ano": 2024,
  "status": "Em manutenção",
  "patioId": 1
}
```

> **Observação**: a coluna **Placa** é única. Para criar múltiplas motos, altere a placa (ex.: `ABC1D23`, `DEF2G45`).

## Paginação & HATEOAS

### Paginação (exemplo)
`GET /api/v1/filiais?pageNumber=1&pageSize=2` retorna:
```json
{
  "data": [ /* até 2 itens */ ],
  "total": 12,
  "pageNumber": 1,
  "pageSize": 2,
  "links": [
    { "rel": "self", "href": ".../filiais?pageNumber=1&pageSize=2", "method": "GET" },
    { "rel": "create", "href": ".../filiais", "method": "POST" }
  ]
}
```

### HATEOAS (exemplo de detalhe)
`GET /api/v1/filiais/1`:
```json
{
  "data": { "id": 1, "nome": "Filial Centro", "endereco": "Av. Central, 1000" },
  "links": [
    { "rel": "self",   "href": ".../filiais/1", "method": "GET" },
    { "rel": "update", "href": ".../filiais/1", "method": "PUT" },
    { "rel": "delete", "href": ".../filiais/1", "method": "DELETE" }
  ]
}
```

## Status Codes adotados
- `200 OK` — consultas com sucesso
- `201 Created` — criação (com header **Location**)
- `204 No Content` — atualização/exclusão
- `400 Bad Request` — validação falhou (ex.: campos obrigatórios vazios)
- `404 Not Found` — recurso inexistente

## Testes (opcional)
- **xUnit** básico incluído (`MottuApi.Tests` → `dotnet test`).
- **Postman Collection** disponível (CRUD completo + paginação + checagens).

## Notas
- O **Swagger** já está habilitado em `Development`.
- O **banco SQLite** fica no arquivo `mottu.db` na raiz do projeto.
- Semeadura automática cria 1 Filial, 1 Pátio e 1 Moto para facilitar os testes.
