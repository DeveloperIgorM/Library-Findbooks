

<h1 align="center">Devsbook 📚</h1>

![Imagem do projeto](wwwroot/imagem/find.png)

**Library-FindBooks** é um sistema web desenvolvido com **C# ASP.NET MVC** que funciona como uma vitrine de bibliotecas e livros. Usuários podem cadastrar suas bibliotecas pessoais e adicionar livros à plataforma, permitindo uma visualização pública e organizada das obras disponíveis.


## 🚀 Demonstração

> 💻 Em breve será disponibilizado um link para demonstração online.



## 🎯 Objetivo

Criar uma aplicação web funcional com foco em:
- Organização e exibição de bibliotecas e livros;
- Cadastro e gerenciamento de bibliotecas e livros pelo usuário;
- Estruturação em MVC com boas práticas de desenvolvimento;
- Persistência de dados em banco relacional com **SQLite**.

---

## 🔧 Tecnologias Utilizadas

| Tecnologia     | Descrição                                  |
|----------------|----------------------------------------------|
| **C#**         | Linguagem principal do backend              |
| **.NET (ASP.NET MVC)** | Framework web utilizado                  |
| **HTML5**       | Estrutura do front-end                     |
| **CSS3**        | Estilização responsiva                     |
| **JavaScript**  | Funcionalidades interativas no front       |
| **SQLite**      | Banco de dados leve e embutido             |

---

## 📁 Estrutura de Pastas (resumo)

```bash
/Controllers       
/Dto               
/Filtros           
/Migrations        
/Models            
/Services          
/Views             
/wwwroot           
/Properties        
appsettings.json   
Bibliotecas.db     
Program.cs         
NewRepository.csproj
```

## 📦 Funcionalidades

📖 Cadastro de bibliotecas pessoais

📚 Adição de livros à biblioteca

🔍 Listagem de livros por biblioteca

✏️ Edição e exclusão de registros

🗂️ Visualização organizada por categoria


---

## 🛠️ Como executar localmente

### Clone o repositório:
```bash
git clone https://github.com/developerIgorM/Library-FindBooks.git
cd Library-FindBooks
```

### Abra no Visual Studio Code ou Visual Studio.
### Restaure os pacotes e dependências:

```bash
dotnet restore
```

### Execute a aplicação:
```bash
dotnet run
```

### Acesse no navegador:
http://localhost:5000 (ou porta informada no terminal)


## 🗃️ Banco de Dados

#### Banco utilizado: SQLite

#### Arquivo do banco: Bibliotecas.db
 
#### Migrações podem ser gerenciadas com:


```bash
dotnet ef migrations add NomeDaMigracao
dotnet ef database update
```

## 👨🏾‍💻 Autor
### Desenvolvido por Igor Matheus

🌐  [Portfólio](https://igorportfolio.vercel.app/)


💻 [GitHub](https://github.com/DeveloperIgorM)


## 📄 Licença
Distribuído sob a licença MIT.<br/>
Sinta-se livre para usar, estudar, modificar e contribuir!