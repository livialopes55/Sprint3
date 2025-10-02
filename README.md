#  Mottu API - Challenge FIAP 2025

API RESTful desenvolvida em **.NET 8 (Web API)** para o Challenge da disciplina **Advanced Business Development with .NET**.  
Implementa CRUD completo, paginação, HATEOAS, boas práticas REST e documentação via Swagger/OpenAPI.

---

##  Tecnologias Utilizadas
- ASP.NET Core 8 (Web API)
- Entity Framework Core 8
- SQLite (banco local para persistência)
- Swagger / OpenAPI (documentação)
- API Versioning

---

##  Estrutura de Entidades
A API possui 3 entidades principais, justificando o domínio da aplicação:

- **Filial** → representa as unidades da Mottu.  
- **Pátio** → áreas físicas onde as motos são armazenadas.  
- **Moto** → veículos que podem estar vinculados a um pátio.

Relacionamentos implementados:
- Uma **Filial** possui vários **Pátios**.
- Um **Pátio** possui várias **Motos**.

---

## Endpoints Implementados

### Filiais
- `GET /api/v1/filiais?pageNumber=1&pageSize=10` → Lista paginada de filiais (com HATEOAS).  
- `GET /api/v1/filiais/{id}` → Detalhe de uma filial.  
- `POST /api/v1/filiais` → Criação de filial.  
- `PUT /api/v1/filiais/{id}` → Atualização de filial.  
- `DELETE /api/v1/filiais/{id}` → Exclusão de filial.  

### Pátios
- `GET /api/v1/patios`  
- `GET /api/v1/patios/{id}`  
- `POST /api/v1/patios`  
- `PUT /api/v1/patios/{id}`  
- `DELETE /api/v1/patios/{id}`  

### Motos
- `GET /api/v1/motos`  
- `GET /api/v1/motos/{id}`  
- `POST /api/v1/motos`  
- `PUT /api/v1/motos/{id}`  
- `DELETE /api/v1/motos/{id}`  

---

## Paginação & HATEOAS
Nos endpoints de listagem, é possível utilizar parâmetros:
- `?pageNumber=1&pageSize=10`

Exemplo de resposta com HATEOAS:

```json
{
  "data": [
    {
      "id": 1,
      "nome": "Filial Barueri",
      "endereco": "Rua X, São Paulo"
    }
  ],
  "total": 6,
  "pageNumber": 1,
  "pageSize": 5,
  "links": [
    {
      "rel": "self",
      "href": "http://localhost:5000/api/v1/filiais?pageNumber=1&pageSize=5",
      "method": "GET"
    },
    {
      "rel": "create",
      "href": "http://localhost:5000/api/v1/filiais",
      "method": "POST"
    }
  ]
}
```

---

##  Swagger
A documentação da API é gerada automaticamente e acessível em:

 [http://localhost:5000](http://localhost:5000)  

---

## Como Executar
1. Clone este repositório:
   ```bash
   git clone https://github.com/livialopes55/Sprint3.git
   ```
2. Entre na pasta do projeto:
   ```bash
   cd MottuApi
   ```
3. Restaure os pacotes:
   ```bash
   dotnet restore
   ```
4. Execute a aplicação:
   ```bash
   dotnet run
   ```

---

## Integrantes
- **Lívia Lopes** - RM: 556281  
- **Henrique Pecora** - RM: 556612  
- **Santhiago de Gobbi** - RM: 98420  

