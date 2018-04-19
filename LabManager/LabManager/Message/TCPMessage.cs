namespace LabManager.Message
{
    public class TCPMessage : LabManager.Message.Message
    {
        public Code ServerOpened_OK = new Code("服务器打开成功");
        public Code InsufficientPermission = new Code("权限不够");

        public Code ConnectToServer_OK = new Code("连接服务器成功");
        public Code ConnectToServer_Failed = new Code("连接服务器失败");
        public Code ConnectToServer_Waitting = new Code("正在连接服务器");

        public Code ClientConnect_OK = new Code("客户端连接成功");
        public Code ClientConnect_Failed = new Code("客户端连接失败");
        public Code ClientConnect_Waitting = new Code("客户端正在连接");

    }
}
