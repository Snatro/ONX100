
using ONX100.Models;

public class DeviceMessageParser
{
    public DeviceMessage Parse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Unknown
            };
        }


        if (message.StartsWith("*"))
        {
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Handshake
            };
        }


        if (message.StartsWith("EVT"))
        {   
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Event
            };
        }


        if (message.StartsWith("ERR"))
        {
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Error
            };
        }

        if (message.Contains("session ended"))
        {
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Disconnect
            };
        }

        if (message.Contains("OK"))
        {
            return new DeviceMessage
            {
                Raw = message,
                Type = MessageType.Acknowledge
            };
        }


        return new DeviceMessage
        {
            Raw = message,
            Type = MessageType.Response
        };
    }
}