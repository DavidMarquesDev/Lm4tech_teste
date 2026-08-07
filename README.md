# ProductChallenge

Projeto desenvolvido como **teste técnico para uma vaga de desenvolvimento**.

O objetivo do desafio é avaliar conhecimentos em **C# .NET 8**, **Entity Framework Core**,
**Windows Forms** e **padrões de arquitetura**, por meio da construção de um sistema de
cadastro e gerenciamento de produtos.

## Tecnologias

- C# / .NET 8 (`net8.0-windows`)
- Windows Forms com padrão MVVM
- Entity Framework Core 8 + SQLite
- CommunityToolkit.Mvvm (`ObservableObject`, `AsyncRelayCommand`)
- Microsoft.Extensions.DependencyInjection e Microsoft.Extensions.Logging
- Serilog (sink de arquivo)
- xUnit

## Fases

O desafio é dividido em três fases, cada uma em sua própria branch.

| Fase | Branch | Situação |
|------|--------|----------|
| 1 — Conceitos básicos | `fase1-basico` | Concluída |
| 2 — Padrões avançados (DI, Generics, Repository) | `fase2-avancado` | Concluída |
| 3 — Reflection e complexidade | `fase3-complexo` | Concluída |

## Funcionalidades

Tela única de cadastro com `DataGridView` para listagem e painel de edição para as operações
de inclusão, alteração e exclusão, mais busca por nome e descrição, listagem paginada com
10, 15, 30, 50 ou 100 itens por página e exportação para CSV com escolha de colunas.

### Arquitetura

```
src/
├── ProductChallenge.Domain/          entidade, invariantes, categorias e busca normalizada
│   ├── Metadata/                     atributos lidos por reflexão
│   ├── Validation/                   DynamicValidator e uma regra por atributo
│                                     nenhuma dependência externa
├── ProductChallenge.Application/     contratos e regras de aplicação
│   ├── Abstractions/                 IRepository<T>, IProductRepository, IProductService
│   ├── Services/ProductService.cs
│   ├── Messaging/                    IServiceBus<T> e a notificação de mudança
│   ├── Reporting/                    catálogo de colunas e contratos de exportação
│   ├── ProductInput.cs               o que foi digitado, com as regras em atributos
│   └── ProductDraft.cs               o que foi aprovado, pronto para o serviço
├── ProductChallenge.Infrastructure/  o único projeto que conhece o Entity Framework
│   ├── Persistence/                  AppDbContext, migrations, localização do banco
│   ├── Repositories/                 ProductRepository
│   ├── Messaging/                    barramento em memória e log de auditoria
│   ├── Reporting/                    CsvReportWriter
│   └── DependencyInjection.cs        registro e migração
└── ProductChallenge.Desktop/         Windows Forms
    ├── Views/  ViewModels/
    ├── Common/                       ligação de comandos e apoio a binding
    └── Composition/                  composição da raiz de dependências
tests/
└── ProductChallenge.Tests/
```

As dependências apontam para dentro: `Domain` não referencia ninguém, `Application` declara as
interfaces sem saber como os dados são gravados, `Infrastructure` implementa, e `Desktop` conhece
todos porque é a raiz de composição. `ArchitectureTests` falha se essa regra for quebrada.

### Entidade

`Product` expõe os cinco campos exigidos — `Id`, `Name`, `Price`, `Category` e `StockQuantity` —
mais um `Description` opcional para a ficha técnica. As propriedades têm `set` privado e só são
alteradas por `Create` ou `SetDetails`, que garantem as invariantes: nome obrigatório com até
120 caracteres, descrição de até 1000, preço maior que zero, estoque não negativo e categoria
válida.

`Description` é `string`, não binário: um `byte[]` exigiria seletor de arquivo e preview, e não
teria representação possível na exportação CSV prevista para a Fase 3.

`Id` é `int` para atender à assinatura do repositório genérico exigida na Fase 2
(`GetByIdAsync(int id)` e `DeleteAsync(int id)`).

### Validação

Ocorre em duas camadas com propósitos distintos:

- **Entrada do usuário** — `ProductEditorViewModel.TryBuildDraft()` converte e valida os campos
  num único passo, devolvendo um erro por campo. A View associa cada erro ao controle
  correspondente através de um `ErrorProvider`.
- **Domínio** — as verificações em `Product.SetDetails` são a rede de segurança final. Uma
  exceção ali indica que uma regra deixou de ser aplicada na entrada, não erro do usuário.

Preço e estoque são mantidos como texto no ViewModel para que o painel reflita exatamente o que
foi digitado, inclusive quando o conteúdo ainda não é um número válido. A conversão respeita a
cultura corrente, aceitando o formato `1.234,56`.

### Banco de dados

SQLite com Migrations. O arquivo `products.db` fica no diretório do executável e as migrations
são aplicadas na inicialização, sem necessidade de comando manual.

## Como executar

Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) e Windows.

```bash
dotnet build
dotnet run --project src/ProductChallenge.Desktop
```

Ou abra `ProductChallenge.sln` no Visual Studio 2022 e execute com `F5`.

### Exportação

O botão **Exportar CSV** abre a escolha de colunas, permite reordená-las e grava o arquivo com
BOM e separador da cultura, para abrir direto no Excel em português. A exportação considera o
filtro de busca ativo.

### Log

As mensagens ficam em `logs/produtos-<data>.log`, ao lado do executável. A auditoria registra
qual campo mudou, com o valor anterior e o novo, em vez de apenas dizer que houve alteração:

```
Produto 91 "Arroz integral" atualizado — Preço: 3.549,83 → 1.499,00
Produto 92 "Monitor 27" criado — Preço: 1.899,00; Categoria: Eletrônicos; Estoque: 8
```

Exportações e exceções não tratadas também são registradas; SQL do Entity Framework é omitido
para não afogar o arquivo.

### Dados de demonstração

```bash
dotnet run --project src/ProductChallenge.Desktop -- --seed
```

Gera 100 produtos para avaliar listagem e paginação. Só grava se a tabela estiver vazia, e nunca
roda numa execução normal.

### Migrations

```bash
dotnet tool install --global dotnet-ef --version 8.0.26
dotnet ef migrations add NomeDaMigration   --project src/ProductChallenge.Infrastructure   --startup-project src/ProductChallenge.Infrastructure   --output-dir Persistence/Migrations
```

## Testes

```bash
dotnet test
```

182 testes cobrindo as invariantes do domínio, a normalização de busca, a validação por
atributos, o CRUD completo, os limites da paginação, o diff da auditoria, o catálogo de colunas,
o escape do CSV, o barramento de mensagens e a regra de dependência entre camadas.

Os testes de CRUD usam **SQLite em memória** (`Filename=:memory:` com a conexão mantida aberta),
não o provider InMemory do EF. O provider InMemory não aplica as restrições do mapeamento nem
suporta `ExecuteDelete`, o que faria o teste passar por motivos diferentes dos de produção.

`Product` e `ProductEditorViewModel` são testados sem dublê nenhum, e os ViewModels rodam sem
abrir janela porque não referenciam `System.Windows.Forms`.

## Decisões técnicas

- **`Id` é `int`, não `Guid`** — a Fase 2 exige `IRepository<T>` com `GetByIdAsync(int id)`.
  Adotar `Guid` agora custaria refazer entidade, migration e ViewModels.
- **SQLite em vez do provider InMemory** — o enunciado permite os dois. O InMemory é um dublê
  para testes: não persiste entre execuções nem permite demonstrar Migrations.
- **`IProductRepository` além do `IRepository<T>`** — o repositório genérico cobre o CRUD, mas
  não tem como expressar uma consulta de domínio. Filtro e paginação entram num contrato
  específico em vez de virarem trabalho em memória sobre `GetAllAsync`.
- **`Skip`/`Take` chegam ao SQL** — a página é montada no banco, junto de um `Count` sobre o
  mesmo filtro. Trazer tudo para depois recortar em memória anularia o ganho da paginação.
- **A página fora da faixa é ajustada no repositório** — quem já conta o total é quem sabe qual
  é a última página. Sem isso, filtrar ou excluir na última página deixaria a tela vazia sem
  explicação.
- **`IRepository<T>` transcrito do enunciado sem alterações** — sem `CancellationToken`, e o
  `Task<T>` não anulável resolvido com `KeyNotFoundException` em vez de `null`.
- **`DataAccessException`** — a Infrastructure traduz falhas do Entity Framework para um tipo da
  camada Application. Sem isso o Desktop precisaria referenciar o EF Core só para nomear
  `DbUpdateException`, e a separação de camadas cairia por terra.
- **Repositório e serviço `Transient`, ViewModel e Form `Singleton`** — ambos são sem estado e
  criam um contexto por operação, então nada obsoleto fica retido dentro do consumidor de vida
  longa.
- **Validação em dois mecanismos, cada um no que serve** — converter texto em número não é
  expressável como atributo, então a conversão fica no ViewModel; obrigatoriedade, faixa e
  tamanho ficam em atributos de `ProductInput`, interpretados por `DynamicValidator`. Os dois
  conjuntos de erro chegam à tela pelo mesmo `ErrorProvider`.
- **`ProductInput` e `ProductDraft` são tipos diferentes de propósito** — um representa o que foi
  digitado, com tipos anuláveis para distinguir "não informado" de zero; o outro representa o que
  já passou pela validação, e por isso o serviço recebe valores não anuláveis.
- **`DynamicValidator` com uma regra por atributo** — suportar um atributo novo passa a ser
  acrescentar uma implementação de `IValidationRule`, e não editar o validador.
- **Reflexão com cache** — `DynamicValidator` e `ExportFieldCatalog` resolvem propriedades e
  atributos uma vez por tipo num `ConcurrentDictionary`. Sem isso cada tentativa de salvar
  repetiria a inspeção.
- **`[ExportColumn]` é opt-in** — se a descoberta fosse por todas as propriedades públicas,
  `SearchText` apareceria para o usuário escolher, e ela é coluna derivada.
- **A exportação respeita o filtro em uso** — quem reduziu a lista a doze itens espera receber
  doze, não o catálogo inteiro.
- **O barramento tem dois assinantes** — a tela recarrega e a auditoria registra, sem que o
  serviço que grava conheça nenhum dos dois. É o segundo assinante que comprova o desacoplamento;
  com um só, o padrão seria decoração.
- **A notificação carrega o diff, não só o verbo** — quem compara é o serviço, único ponto que vê
  o antes e o depois; o assinante só formata. Um log que diz apenas "atualizado" não responde à
  pergunta que se faz a ele depois: o que mudou.
- **Log em arquivo, não em console** — num `WinExe` não existe console anexado, e o provider de
  console descartaria tudo em silêncio. As mensagens vão para `logs/produtos-<data>.log`, com
  rotação diária e sete dias retidos, e `Log.CloseAndFlush()` num `finally` garante que as
  últimas linhas — justamente as de um encerramento anormal — cheguem ao disco. O `AddDebug`
  complementa para quem roda pelo Visual Studio.
- **O EF fica fora do arquivo** — `MinimumLevel.Override("Microsoft", Warning)`. Sem isso o log
  afoga em SQL e o rastro da aplicação se perde no meio.
- **Grid somente leitura** — a edição acontece no painel, sob validação. Isso elimina a classe de
  problemas de alterar a coleção enquanto uma célula está em modo de edição.
- **`ExecuteDeleteAsync`** — remove sem materializar a entidade.
- **Busca com atraso de 300 ms** — o `Timer` fica na View, que é onde a decisão de quando
  disparar a consulta pertence; sem ele haveria uma ida ao banco por tecla digitada.
- **Descrição fora do grid** — texto longo em coluna de grade prejudica a leitura da lista. Ela
  aparece no painel de edição e é alcançada pela busca.
- **Coluna `SearchText` para a busca** — o `LIKE` do SQLite só ignora diferença de caixa para
  ASCII e não respeita *collation*, de modo que "eletronico" não encontraria "Eletrônico".
  `Product` mantém nome e descrição em forma normalizada (minúsculas, sem acentos) numa coluna
  indexada, derivada em `SetDetails` e nunca atribuída de fora. O termo digitado passa pela mesma
  normalização, então os dois lados da comparação usam sempre a mesma forma.
- **Categoria sincronizada à mão, sem `DataBindings`** — o WinForms não expõe um evento
  `SelectedItemChanged`, então um binding em `SelectedItem` com `DataSourceUpdateMode.OnPropertyChanged`
  degrada silenciosamente para gravar apenas na perda de foco. Escolher a categoria pelo teclado e
  acionar Salvar por atalho não move o foco, e a escolha se perderia.

### Além do que foi pedido

O enunciado não pede nenhum destes itens; foram incluídos por serem difíceis de justificar como
ausentes num sistema real:

| Item | Motivo |
|------|--------|
| Testes automatizados | Sem eles não haveria rede de segurança para o refactor da Fase 2 |
| `ArchitectureTests` | Sem eles a regra de dependência é só um acordo que ninguém confere quando alguém adiciona um `using` conveniente |
| Campo `Description` | Os cinco campos exigidos continuam presentes; a ficha técnica torna o cadastro utilizável e dá substância à seleção de colunas da exportação prevista na Fase 3 |
| Busca por nome e descrição, insensível a acento e caixa | Uma lista sem filtro deixa de ser navegável já na casa das dezenas de itens, e num cadastro em português exigir o acento correto inviabiliza a busca |
| Evento `OperationFailed` | Uma falha de banco não pode desaparecer em silêncio |
| Barra de status | O usuário precisa saber se a operação concluiu |
| Confirmação ao excluir | Exclusão é irreversível |
| `.editorconfig` com `utf-8-bom` | Evita corromper acentuação ao editar em ferramentas diferentes |
| `global.json` | Fixa a versão do SDK entre máquinas |
| `TreatWarningsAsErrors` no Release | O projeto compila com zero avisos, e foi ele que pegou um `CS0108` real antes da entrega |
| Handlers globais de exceção | Sem eles uma falha não tratada derruba a aplicação sem deixar registro |
| Log em arquivo | A caixa de erro instrui a consultar o log; sem arquivo, seria uma promessa vazia |

### O que eu faria diferente em produção

- `CancellationToken` em todos os métodos assíncronos.
- Exportação em segundo plano com barra de progresso, para volumes muito acima de 100 itens.
- Configuração externa (`appsettings.json`) para a connection string.

## Autor

David Marques
