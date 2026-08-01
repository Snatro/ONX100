
namespace ONX100.Models
{

    public enum MessageType
    {
        Unknown,
        Handshake,
        Event,
        Disconnect,
        Response,
        Acknowledge,
        Error
    }


    public class DeviceMessage
    {
        public string Raw { get; set; } = "";

        public MessageType Type { get; set; }
    }

}