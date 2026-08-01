using ONX100.Models;

namespace ONX100.Communication;

public interface ITcpClientConnection
{
    bool IsConnected { get; }

    Task Connect();

    Task SendCommand(string command);

    Task<DeviceMessage> Query(string command);
}