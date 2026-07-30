using ONX100.Communication;
using ONX100.Models;

namespace ONX100.Driver;

public class ProjectorDriver
{
    private readonly TcpClientConnection _connection;


    public ProjectorDriver(
        TcpClientConnection connection)
    {
        _connection = connection;
    }


    public async Task Connect()
    {
        await _connection.Connect();
    }


    private void EnsureConnected()
    {
        if (!_connection.IsConnected)
        {
            throw new InvalidOperationException(
                "Device is not connected."
            );
        }
    }


    public async Task<CommandResponse> PowerOn()
    {
        EnsureConnected();

        return await ExecuteCommand("PWR ON");
    }


    public async Task<CommandResponse> PowerOff()
    {
        EnsureConnected();

        return await ExecuteCommand("PWR OFF");
    }


    public async Task<bool> GetPowerState()
    {
        EnsureConnected();

        var response =
            await _connection.SendAndReceive("PWR ?");


        ValidateResponse(response);


        return response.Raw == "PWR ON";
    }


    public async Task<CommandResponse> SetInput(int input)
    {
        if (input < 1 || input > 4)
        {
            return ErrorResponse(
                "ERR 02",
                "Invalid parameter"
            );
        }


        EnsureConnected();

        return await ExecuteCommand(
            $"IN {input}"
        );
    }


    public async Task<int> GetInput()
    {
        EnsureConnected();


        var response =
            await _connection.SendAndReceive("IN ?");


        ValidateResponse(response);


        if (!response.Raw.StartsWith("IN "))
        {
            throw new Exception(
                $"Unexpected response: {response.Raw}"
            );
        }


        return int.Parse(
            response.Raw.Replace("IN ", "")
        );
    }


    public async Task<CommandResponse> SetVolume(int volume)
    {
        if (volume < 0 || volume > 100)
        {
            return ErrorResponse(
                "ERR 02",
                "Invalid parameter"
            );
        }


        EnsureConnected();

        return await ExecuteCommand(
            $"VOL {volume}"
        );
    }


    public async Task<int> GetVolume()
    {
        EnsureConnected();

        var response =
            await _connection.SendAndReceive("VOL ?");


        var value = response.Raw
            .Replace("VOL ", "")
            .Trim();


        return Convert.ToInt32(
            value,
            16
        );
    }


    public async Task<CommandResponse> SetMute(
        bool enabled)
    {
        EnsureConnected();


        var command = enabled
            ? "MUTE ON"
            : "MUTE OFF";


        return await ExecuteCommand(command);
    }


    public async Task<bool> GetMute()
    {
        EnsureConnected();


        var response =
            await _connection.SendAndReceive("MUTE ?");


        ValidateResponse(response);


        return response.Raw == "MUTE ON";
    }


    public async Task<UnitPropertiesStatus> GetStatus()
    {
        EnsureConnected();


        var power =
            await GetPowerState();

        var input =
            await GetInput();

        var volume =
            await GetVolume();

        var mute =
            await GetMute();


        return new UnitPropertiesStatus
        {
            IsPoweredOn = power,
            InputSource = input,
            VolumeLevel = volume,
            IsMuted = mute
        };
    }


    private async Task<CommandResponse> ExecuteCommand(
        string command)
    {
        var response =
            await _connection.SendAndReceive(command);


        if (response.Type == MessageType.Error)
        {
            return ErrorResponse(
                response.Raw,
                GetErrorMessage(response.Raw)
            );
        }


        if (response.Raw == "OK")
        {
            return new CommandResponse
            {
                Success = true,
                Response = response.Raw
            };
        }


        return ErrorResponse(
            response.Raw,
            "Unexpected response"
        );
    }


    private void ValidateResponse(
        DeviceMessage response)
    {
        if (response.Type == MessageType.Error)
        {
            throw new Exception(
                $"Device error: {response.Raw}"
            );
        }
    }


    private CommandResponse ErrorResponse(
        string code,
        string message)
    {
        return new CommandResponse
        {
            Success = false,
            Response = code,
            ErrorCode = code,
            ErrorMessage = message
        };
    }


    private string GetErrorMessage(
        string errorCode)
    {
        return errorCode switch
        {
            "ERR 01" => "Unknown command",
            "ERR 02" => "Invalid parameter",
            "ERR 03" => "Command rejected",
            _ => "Unknown error"
        };
    }
}