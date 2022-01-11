# Teste back-end enContact

Bem-vindo ao teste para desenvolvimento back-end na enContact.

## O teste

Criamos um pequeno projeto onde tentamos simular o que acontece no dia a dia.
Um código onde você poderá encontrar pontos com bugs, necessidade de refatoração, problemas que podem gerar má performance etc.
O projeto consiste em uma API simples para cadastro e consulta de contatos em uma agenda.

Neste teste você poderá mostrar suas habilidades em c# e dotnet, Orientação a objetos, lógica de programação, SQL, refatoração e também suas habilidades de debug para encontrar os problemas.

O foco deste teste é a garantia dos requisitos abaixo estejam funcionais na API:

- [X] Criar, editar, excluir e listar agendas.
- [X] Criar, editar, excluir e listar empresas.
- [X] Importar contatos a partir de um arquivo .csv
  - Fique a vontade para definir o leiaute do arquivo .csv
  - Caso dê erro na importação de um registro, não deve impactar a importação dos demais.
  - É obrigatório ter uma agenda vinculada ao contato.
  - No arquivo, se for informada uma empresa ao contato, ela deve existir previamente no sistema. Caso não seja informado, o contato é registrado sem vinculo com a empresa.
- [X] Pesquisar contatos
  - Deve pesquisar em qualquer campo do contato (incluído o nome da empresa).
  - A pesquisa deve ser paginada (Fique a vontade para utilizar qualquer estratégia).
- [X] Pesquisa de contatos da empresa
  - Poder consultar os contatos de uma agenda e que estejam em uma determinada empresa.

## O repositório

1. Faça o fork do nosso repositório no Github.
2. Clone do projeto.
3. Faça as devidas alterações para atender aos requisitos.
4. Caso seja necessário, enviar junto o arquivo "Database.db" que fica na raiz do projeto.

## O que fazer?

1. Crie/Edite as APIs necessárias para atender os requisitos.
2. Refatore o código da maneira que achar melhor.
3. Fique a vontade para usar qualquer biblioteca externa, caso seja necessário.

## Desafio do desafio

Tem um tempinho a mais? Acha que pode fazer mais? Então aqui vai alguns desafios para seu projeto, que serve como um plus no seu teste!

- Seria uma boa se pontos críticos do código tivessem testes unitários.
- [X] Adicionar autenticação na API seria interessante.
- [X] Poder exportar a agenda completa também seria legal.

## Observação

Senti liberdade para fazer alterações no projeto base e usando a parte de autenticação como base algumas ações deixaram de existir.

Atualizei o projeto para apenas companhias previamente cadastradas possam inserir/alterar informações do sistema, ao se cadastrar pela primeira vez uma chave de API única para aquela companhia é entregue ao cliente, este deve utiliza-lá em todas as outras requicições seja enviando na query da uri (?API_KEY=xxxxx) ou por um header na request (API_KEY: xxxxx).

O sistema por sua vez irá confirmar se a chave API informada existe e se por exemplo a request solicitada seja para visualizar um ID de um contato/agenda, se o ID pertence a companhia da API_KEY.

Deleção e alteração só serão possíveis se o ID a ser modificado pertencer a companhia da API_KEY.

A inserção de novos contatos é obrigatório o envio de um ID de agenda, sendo essa agenda também confirmada se pertence a companhia da API_KEY.

Percebi uma limitação do código (que não sei se era pra ser a idéia ou se poderia ser modificado) na forma feita o projeto só permitia uma agenda por companhia, pois o IdContactBookId estava atrelado diretamente na tabela Companhia, realizei a alteração para atrelar na tabela ContactBook o Id da companhia que pertence, dando a possibilidade das companhias terem infinitas agendas.


O delete de uma companhia causará a deleção de todas suas agendas e respectivamente todos os seus contatos.

A exportação em CSV só exportará os contatos da companhia da API_KEY podendo ser todos os contatos ou apenas os contatos de uma agenda.


É possível obter a listagem de todas as agendas (independente da companhia), porém os dados serão omitidos quando não estiver autenticado, exemplo

```json
{
  "id": 1,
  "name": "con******",
  "total_contacts": 15
}
```


A pesquisa de contatos possui dois end-points, uma pública e uma privada

```bash
/contact/search
/contact/public/search
```

A pesquisa pública poderá ser realizada por qualquer pessoa mesmo sem API_KEY e irá pesquisar contatos de todas as empresas/agendas, porém a exibição dos dados será limitada

```json
{
  "lista": [
    {
      "id": 85,
      "contactBookId": 6,
      "companyId": 2,
      "name": "ann*******",
      "phone": "(15) 12*******",
      "email": "annrnre@u*******",
      "address": "rua e*******"
    }
  ],
  "total": 1
}
```

A privada por sua vez pesquisará apenas contatos e agendas pertecentes a API_KEY informada, sendo a visão completa.

Os termos de pesquisa podem ser tanto usados em conjunto como também sozinhos, são eles:

```bash
ContactName - nome do contato ou parte dele
ContactPhone - telefone do contato ou parte dele
ContactEmail - e-mail do contato ou parte dele
ContactAddress - endereco do contato ou parte dele
CompanyName - nome da empresa que o contato pertence ou parte dele
AgendaId - ID da agenda atrelada ao contato
AgendaName - nome da agenda atrelada ao contato ou parte dele
```


A páginação foi feita de forma similar a como funciona o Linkedin (que usei de inspiração por nenhuma razão), a listagem máxima exibida serão de 10 contatos e a listagem do número total encontrado.

Para a visualização dos próximos 10 o parâmetro start_from deverá ser enviada com o valor 10, 20, 30 ...

Podendo ser um valor no meio também, exemplo 5 que mostrará a partir da quinta linha ate a 15 


A importação deverá seguir o layout:

ContactBookId,Name,Phone,Email,Address


No caso de falha ao ''parsear'' uma das linhas o sistema irá pular para a próxima e a resposta final consiste em um json com o status para cada linha

```json
[
  {
    "contactBookId": 9,
    "companyId": 2,
    "name": "hehe",
    "contactId": 93,
    "address": "rua joao eudorio",
    "phone": "(15) 99119-6799",
    "email": "hehe@terra.com.br",
    "status": "OK"
  },
  {
    "line": "9,kaka,(15) 99339-94",
    "status": "Invalid format contact"
  },
  {
    "contactBookId": 9,
    "companyId": 2,
    "name": "annrnre",
    "contactId": 94,
    "address": "rua eujaja",
    "phone": "(15) 1299-3999",
    "email": "annrnre@uol.com.br",
    "status": "OK"
  }
]
```

A importação poderá enviar o novo contato para agendas diferentes, desde que informadas e que essas sejam pertencentes a companhia da API_KEY.





