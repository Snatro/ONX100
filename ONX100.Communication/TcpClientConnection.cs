using Microsoft.Extensions.Configuration;
using ONX100.Models;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ONX100.Communication;

public class TcpClientConnection
{
    private TcpClient? _client;
    private NetworkStream? _stream;

    private readonly string _ip;
    private readonly int _port;

    private readonly StringBuilder _receiveBuffer = new();

    private readonly DeviceMessageParser _parser;


    public TcpClientConnection(
        IConfiguration configuration,
        DeviceMessageParser parser)
    {
        _ip = configuration["Device:IpAddress"]!;
        _port = int.Parse(
            configuration["Device:Port"]!
        );

        _parser = parser;
    }


    public async Task Connect()
    {
        Disconnect();

        _client = new TcpClient();

        await _client.ConnectAsync(
            _ip,
            _port
        );


        _stream = _client.GetStream();


        Console.WriteLine(
            $"Connected to {_ip}:{_port}"
        );
    }


    public void Disconnect()
    {
        try
        {
            _stream?.Dispose();
            _client?.Close();
        }
        catch
        {
            // Ignore cleanup errors
        }


        _stream = null;
        _client = null;

        _receiveBuffer.Clear();
    }


    private async Task EnsureConnected()
    {
        if (!IsConnected)
        {
            Console.WriteLine(
                "Connection lost. Reconnecting..."
            );

            await Connect();
        }
    }


    private async Task Send(string command)
    {
        await EnsureConnected();

        try
        {
            var bytes = Encoding.ASCII.GetBytes(
                command.Trim() + "\r"
            );


            await _stream!.WriteAsync(bytes);


            Console.WriteLine(
                $"TX: [{command}]"
            );
        }
        catch (IOException)
        {
            Disconnect();
            throw;
        }
    }


    public async Task<DeviceMessage> Receive()
    {
        while (true)
        {
            var rawMessage = await ReadMessage();


            var message =
                _parser.Parse(rawMessage);


            Console.WriteLine(
                $"{message.Type}: {message.Raw}"
            );


            switch (message.Type)
            {
                case MessageType.Event:

                    // Later:
                    // Raise event to driver/frontend
                    Console.WriteLine(
                        $"Device event: {message.Raw}"
                    );

                    continue;


                case MessageType.Handshake:

                    Console.WriteLine(
                        $"Handshake ignored: {message.Raw}"
                    );

                    continue;


                case MessageType.Unknown:

                    Console.WriteLine(
                        $"Unknown message ignored: {message.Raw}"
                    );

                    continue;
            }


            return message;
        }
    }


    public async Task<DeviceMessage> SendAndReceive(
        string command)
    {
        for (int attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                await EnsureConnected();


                await Send(command);


                return await Receive();
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    $"Communication error: {ex.Message}"
                );


                Disconnect();


                if (attempt == 0)
                {
                    await Task.Delay(1000);

                    await Connect();
                }
                else
                {
                    throw;
                }
            }
        }


        throw new Exception(
            "Unable to communicate with device."
        );
    }


    private async Task<string> ReadMessage()
    {
        while (true)
        {
            if (_receiveBuffer
                .ToString()
                .Contains("\r\n"))
            {
                return GetNextMessage();
            }


            var buffer = new byte[1024];


            int bytesRead =
                await _stream!.ReadAsync(buffer);


            if (bytesRead == 0)
            {
                throw new IOException(
                    "Device closed connection."
                );
            }


            var data = Encoding.ASCII
                .GetString(
                    buffer,
                    0,
                    bytesRead
                );


            Console.WriteLine(
                $"RAW RX: {data}"
            );


            _receiveBuffer.Append(data);
        }
    }


    private string GetNextMessage()
    {
        var buffer =
            _receiveBuffer.ToString();


        var index =
            buffer.IndexOf("\r\n");


        var message =
            buffer.Substring(
                0,
                index
            );


        _receiveBuffer.Remove(
            0,
            index + 2
        );


        return message.Trim();
    }


    public bool IsConnected =>
        _client != null &&
        _client.Connected &&
        _stream != null;
}