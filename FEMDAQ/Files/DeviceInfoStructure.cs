using Files.Parser;

namespace Files
{
    public class DeviceInfoStructure
    {
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public InfoBlock InfoBlock { get; private set; }



        public DeviceInfoStructure(string deviceIdentifier, string deviceType, InfoBlock infoBlock)
        {
            DeviceType = deviceType;
            DeviceIdentifier = deviceIdentifier;
            InfoBlock = infoBlock;
        }
    }
}
