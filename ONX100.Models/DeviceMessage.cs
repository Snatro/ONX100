
namespace ONX100.Models
{

    public enum MessageType
    {
        Response,
        Error,
        Event,
        Handshake,
        Unknown
    }


    public class DeviceMessage
    {
        public string Raw { get; set; } = "";

        public MessageType Type { get; set; }
    }

}