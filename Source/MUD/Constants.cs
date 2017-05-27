namespace MUD
{
    public enum CommandType
    {
        Combat,
        General,
        Communication,
        Social
    }

    public enum Position
    {
        Dead,
        Unconscious,
        Sleeping,
        Resting,
        Prone,
        Standing,
        Fighting
    }

    public enum ConnectStatus
    {
        Connected,
        AccountManagement
    }

    public static class Constants
    {
        public const int BufferSize = 1024;
    }
}
