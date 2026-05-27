# 🐾 Liga dos Pets

Plataforma web para adoção responsável de animais. Conecta animais que precisam de um lar a famílias dispostas a amar, oferecendo uma experiência segura e transparente para todos os envolvidos.

---

## 🚀 Funcionalidades

### Para visitantes e usuários
- 🏠 **Página inicial** com estatísticas e animais em destaque.
- 🔍 **Catálogo público** com filtros por espécie, porte, sexo e idade.
- 📋 **Detalhes do animal** com carrossel de fotos, características e agendamento de visita.
- 📝 **Cadastro e login** (ASP.NET Core Identity) com proteção de dados.
- 👤 **Painel do usuário** para gerenciar seus pets cadastrados e acompanhar o status das solicitações.
- 📦 **Solicitação de entrega** com formulário completo (espécie, raça, porte, características e fotos).
- 📅 **Agendamento de visitas** com validação de horários (apenas dias úteis, intervalos de 1h).
- 💬 **Chat em tempo real** com o administrador (SignalR) — botão flutuante e página de mensagens.

### Para administradores
- 📊 **Dashboard administrativo** com métricas (animais disponíveis, adotados, pendentes, usuários, agendamentos) e gráficos.
- 🐶 **CRUD de animais**, espécies, raças, características e usuários.
- 📦 **Gerenciamento de solicitações de entrega** — aprovar ou rejeitar com justificativa.
- 📅 **Gerenciamento de agendamentos** — confirmar, realizar (com opção de adoção) e cancelar.
- 🗂️ **Registro de adoções** e atualização automática do status do animal.
- 💬 **Central de mensagens** para atender usuários em tempo real.

---

## 🧱 Tecnologias

| Camada          | Tecnologias                                                                 |
|-----------------|-----------------------------------------------------------------------------|
| Backend         | ASP.NET Core 10 (MVC), Entity Framework Core, ASP.NET Core Identity          |
| Frontend        | Razor Views, Bootstrap 5, Chart.js, Bootstrap Icons, JavaScript (ES6)       |
| Comunicação     | SignalR (chat em tempo real)                                                |
| Banco de dados  | SQL Server (LocalDB para desenvolvimento)                                    |
| Infraestrutura  | Padrão Repository, Unit of Work, Injeção de Dependência, Clean Architecture |
| Ferramentas     | Swagger (API), `dotnet-ef` (migrations), Visual Studio / VS Code            |

---

## 📁 Estrutura do Projeto

LigaDosPets.sln
├── LigaDosPets.Domain # Entidades e interfaces do domínio
├── LigaDosPets.Application # DTOs, serviços de aplicação, abstrações
├── LigaDosPets.Infrastructure # DbContext, repositórios, migrações, Identity, seed
├── LigaDosPets.Web # Projeto MVC principal (front‑end do usuário)
└── LigaDosPets.API # Projeto API (SignalR, endpoints REST opcionais)


---

## ⚙️ Requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/pt-br/download/dotnet/10.0)
- SQL Server LocalDB (ou outro servidor SQL)
- Visual Studio 2022 / Visual Studio Code Comunity

### 1. Clonar o repositório

2. Configurar a string de conexão
Edite o arquivo LigaDosPets.Web/appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LigaDosPetsDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}

3. Instalar as dependências e compilar

dotnet restore

dotnet build

4. Aplicar migrações e popular o banco
O projeto Web executa automaticamente as migrações e o seeder na inicialização (via Program.cs). Basta rodar:

dotnet ef migrations add InitialCreate --project LigaDosPets.Infrastructure --startup-project LigaDosPets.API

dotnet ef database update --project LigaDosPets.Infrastructure --startup-project LigaDosPets.API

dotnet run --project LigaDosPets.Web

De forma automatica o site ira abrir no navegador, caso isso não aconteça, acesse http://localhost:5000

5. (Opcional) Executar a API separadamente
A API está configurada no projeto LigaDosPets.API e inclui o hub SignalR para chat. Se precisar apenas do back‑end REST, execute:

bash
dotnet run --project LigaDosPets.API
6. Credenciais padrão (criadas pelo seeder)
Papel	E-mail	Senha
Administrador	admin@ligadospets.com	Admin@123
Gerente	gerente@ligadospets.com	Gerente@123
Usuário	usuario@ligadospets.com	Usuario@123
Atenção: Altere essas senhas em produção!

🧪 Testando as funcionalidades
Acesse a Home e explore o catálogo.

Faça login com um usuário comum e cadastre um pet para doação.

Como administrador, acesse a área Admin (menu superior) e gerencie as solicitações.

Teste o chat: como usuário, clique no botão flutuante de chat; como admin, acesse a Central de Mensagens.

Agende uma visita e, no painel admin, realize a visita e confirme a adoção.

📄 Licença
Este projeto é educacional e de código aberto. Sinta‑se livre para usar, modificar e compartilhar.
