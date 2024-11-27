using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

#region TODO
// TODO: Criar Conexão - Estabelecer uma conexão com o RabbitMQ usando ConnectionFactory e CreateConnectionAsync().
// A conexão é responsável por estabelecer o link com o servidor RabbitMQ.

// TODO: Criar Canal - Utilizar CreateChannelAsync() para criar um canal de comunicação a partir da conexão estabelecida.
// O canal é essencial para interagir com o RabbitMQ, possibilitando declarar filas e consumir mensagens.

// TODO: Declarar Fila - Declarar a fila "hello" com QueueDeclareAsync() para garantir que a fila exista antes do consumo das mensagens.
// Isso assegura que as mensagens a serem consumidas tenham uma fila configurada.

// TODO: Criar Consumidor - Criar um consumidor assíncrono com AsyncEventingBasicConsumer para escutar a fila e processar as mensagens recebidas.
// Ao receber uma mensagem, desserializá-la para o objeto Aluno e tratá-la.

// TODO: Consumir Mensagem - Utilizar o canal para iniciar o consumo das mensagens da fila "hello" com BasicConsumeAsync().
// Definir autoAck como true para confirmar automaticamente as mensagens após serem recebidas.
#endregion

namespace Consumers
{
    #region Sem comentário 
    public class Program
    {
        static async Task Main(string[] args)
        {
            //Local
            var factory = new ConnectionFactory { HostName = "localhost" };

            //AWS
            //var factory = new ConnectionFactory
            //{
            //    Uri = new Uri("amqps://"),
            //    Port = 5671,
            //    VirtualHost = "/",
            //    UserName = "",
            //    Password = ""
            //};

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "hello",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false);

            Console.WriteLine("[*] Aguardadno mensagens.");

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
    #endregion

    #region Com comentário 
    //public class Program
    //{
    //    static async Task Main(string[] args)
    //    {
    //        // TODO: PASSO 1: Criar Conexão - Estabelece uma conexão com o servidor RabbitMQ usando a factory.
    //        // A conexão é essencial para que a aplicação se comunique com o RabbitMQ.
    //        var factory = new ConnectionFactory { HostName = "localhost" };
    //        using var connection = await factory.CreateConnectionAsync();

    //        // TODO: PASSO 2: Criar Canal - Utiliza a conexão estabelecida para criar um canal de comunicação com o RabbitMQ.
    //        // O canal é necessário para interagir com o RabbitMQ e realizar operações como declarar filas e consumir mensagens.
    //        using var channel = await connection.CreateChannelAsync();

    //        // TODO: PASSO 3: Declarar Fila - Declara a fila "hello" para garantir que ela exista antes de consumir as mensagens.
    //        // durable: false - A fila não será persistida no disco, sendo removida se o servidor reiniciar.
    //        // exclusive: false - A fila não é exclusiva para esta conexão.
    //        // autoDelete: false - A fila não será removida automaticamente quando não houver consumidores.
    //        await channel.QueueDeclareAsync(queue: "hello",
    //                                        durable: false,
    //                                        exclusive: false,
    //                                        autoDelete: false);

    //        Console.WriteLine("[*] Aguardando mensagens.");

    //        // TODO: PASSO 4: Criar Consumidor - Cria um consumidor assíncrono que será responsável por tratar as mensagens recebidas da fila.
    //        // Ao receber uma mensagem, o evento 'ReceivedAsync' é acionado, desserializando a mensagem para um objeto do tipo 'Aluno'.
    //        var consumer = new AsyncEventingBasicConsumer(channel);
    //        consumer.ReceivedAsync += (model, ea) =>
    //        {
    //            // Converte o corpo da mensagem de byte array para string usando UTF-8.
    //            var corpo = ea.Body.ToArray();
    //            var mensagem = Encoding.UTF8.GetString(corpo);

    //            // Desserializa a mensagem JSON para um objeto do tipo 'Aluno'.
    //            var aluno = JsonSerializer.Deserialize<Aluno>(mensagem);

    //            // Exibe no console o nome do aluno recebido.
    //            Console.WriteLine($"[x] Recebido: {aluno.Nome}");

    //            return Task.CompletedTask; // Finaliza a tarefa para indicar que o processamento foi concluído.
    //        };

    //        // TODO: PASSO 5: Consumir Mensagem - Define que o consumidor consumirá as mensagens da fila "hello".
    //        // autoAck: true - As mensagens serão confirmadas automaticamente após serem recebidas pelo consumidor.
    //        await channel.BasicConsumeAsync(queue: "hello",
    //                                        autoAck: true,
    //                                        consumer: consumer);

    //        Console.WriteLine("Aperte [ENTER] para sair.");
    //        Console.ReadLine();
    //    }
    //}

    //// Classe Aluno - Define as propriedades Nome e Email, usadas para desserializar as mensagens recebidas.
    //public class Aluno
    //{
    //    public string Nome { get; set; }
    //    public string Email { get; set; }
    //}
    #endregion
}
