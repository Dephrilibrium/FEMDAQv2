namespace Files
{
    public class DeviceInfoStructure
    {
        public string DeviceIdentifier { get; private set; }
        public string DeviceType { get; private set; }
        public object InfoBlock { get; private set; }



        public DeviceInfoStructure(string deviceIdentifier, string deviceType, object infoBlock)
        {
            DeviceType = deviceType;
            DeviceIdentifier = deviceIdentifier;
            InfoBlock = infoBlock;
        }
    }
}
