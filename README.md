
# RabbitMQ Consumer Example

Este repositório demonstra como criar um consumidor simples usando RabbitMQ em .NET. O exemplo implementa um processo para conectar-se ao RabbitMQ, consumir mensagens de uma fila e desserializar os dados recebidos em um objeto.

## Tecnologias Utilizadas

- .NET 6+
- RabbitMQ.Client
- System.Text.Json

---

## Funcionalidades

1. **Conexão com RabbitMQ**: Estabelece uma conexão com o servidor RabbitMQ.
2. **Criação de Canal**: Cria um canal de comunicação para interagir com o RabbitMQ.
3. **Declaração de Fila**: Garante que a fila exista antes de consumir mensagens.
4. **Criação de Consumidor**: Processa mensagens recebidas da fila.
5. **Desserialização**: Converte as mensagens JSON recebidas para objetos do tipo `Aluno`.

---

## Estrutura do Projeto

- `Program`: Classe principal que implementa o consumidor.
- `Aluno`: Classe modelo para representar os dados recebidos.

---

## Configuração Local

Certifique-se de que o RabbitMQ esteja em execução localmente. Use as configurações padrão:

```csharp
var factory = new ConnectionFactory { HostName = "localhost" };
```

---

## Configuração AWS

Caso utilize um ambiente em nuvem, atualize as credenciais:

```csharp
var factory = new ConnectionFactory
{
    Uri = new Uri("amqps://"),
    Port = 5671,
    VirtualHost = "/",
    UserName = "<seu_usuario>",
    Password = "<sua_senha>"
};
```

---

## Como Executar

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/rabbitmq-consumer.git
   cd rabbitmq-consumer
   ```

2. Instale as dependências do RabbitMQ no seu ambiente local ou utilize uma instância na nuvem.

3. Compile e execute o projeto:
   ```bash
   dotnet build
   dotnet run
   ```

4. Envie mensagens para a fila `hello` no RabbitMQ.

5. Observe a saída do consumidor exibindo as mensagens recebidas e desserializadas.

---

## Código de Exemplo

### Consumidor

```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false);

        Console.WriteLine("[*] Aguardando mensagens.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var corpo = ea.Body.ToArray();
            var mensagem = Encoding.UTF8.GetString(corpo);
            var aluno = JsonSerializer.Deserialize<Aluno>(mensagem);
            Console.WriteLine($"[x] Recebido: {aluno.Nome}");

            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: "hello",
                                        autoAck: true,
                                        consumer: consumer);

        Console.WriteLine("Aperte [ENTER] para sair.");
        Console.ReadLine();
    }
}

public class Aluno
{
    public string Nome { get; set; }
    public string Email { get; set; }
}
```

---

## TODO List

- [ ] Adicionar reconexão automática em caso de falhas na conexão com RabbitMQ.
- [ ] Implementar lógica de tratamento de mensagens com `autoAck` desativado.
- [ ] Adicionar suporte a filas duráveis.

---

## Contribuição

Sinta-se à vontade para contribuir com melhorias ou novas funcionalidades. Para isso:

1. Faça um fork do repositório.
2. Crie uma nova branch com a feature:
   ```bash
   git checkout -b feature/nova-funcionalidade
   ```
3. Envie um Pull Request.

---

