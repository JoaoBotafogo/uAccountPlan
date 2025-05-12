# uAccountPlan

O objetivo é criar uma API REST para gerenciar o cadastro de **Plano de Contas**.

## Tecnologias Utilizadas

- .NET 8.0
- SQL Server (para persistência de dados)
- Entity Framework Core (para interação com o banco de dados)
- Swagger (para documentação da API)
- xUnit (para testes unitários)

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas, com as seguintes pastas:

- **Application**: Contém a lógica de negócios da aplicação.
- **Domain**: Contém as entidades e regras de domínio.
- **Infrastructure**: Contém a implementação dos repositórios, acesso a dados e outras integrações.
- **WebApi**: Contém os controladores e configurações para expor a API.

## Como Rodar a Solução

### 1. Clonando o Repositório

Clone este repositório para o seu ambiente local:

git clone https://github.com/JoaoBotafogo/uAccountPlan
cd uAccountPlan

### 2. Restaurando as Dependências

Execute o comando abaixo para restaurar as dependências NuGet do projeto:

dotnet restore

### 3. Configuração do Banco de Dados

Antes de rodar a aplicação, você precisa garantir que o banco de dados esteja configurado corretamente.

1. Crie um banco de dados no SQL Server. Você pode usar o SQL Server LocalDB ou uma instância do SQL Server.

2. Abra o arquivo `appsettings.json` e verifique a string de conexão. Altere para apontar para a sua instância do SQL Server, caso necessário.

3. Aplique as migrações para criar as tabelas no banco de dados. Execute o seguinte comando no terminal:

dotnet ef database update

### 4. Rodando a Aplicação

Agora, você pode rodar a aplicação com o comando:

dotnet run

A API estará disponível em http://localhost:5000 (ou a porta configurada em `appsettings.json`).

### 5. Testando a API

A documentação interativa da API será gerada pelo Swagger. Para acessá-la, vá até a URL:

http://localhost:5000/swagger

Aqui você poderá ver todos os endpoints da API e realizar testes diretamente pelo navegador.

## Endpoints Disponíveis

- **GET /api/accountplan**: Retorna todos os planos de contas.
- **POST /api/accountplan**: Cria um novo plano de contas.
- **GET /api/accountplan/{id}**: Retorna um plano de contas específico pelo ID.
- **PUT /api/accountplan/{id}**: Atualiza um plano de contas existente.
- **DELETE /api/accountplan/{id}**: Exclui um plano de contas específico.

## Testes Unitários

Os testes unitários foram implementados utilizando o **xUnit**. Para rodá-los, execute o seguinte comando:

dotnet test

Isso vai executar todos os testes da aplicação.

---

**Autor:** João Alberto
**Data de Criação:** Mai 2025
