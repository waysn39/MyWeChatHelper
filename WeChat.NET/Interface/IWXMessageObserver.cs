using WeChat.NET.Objects;

namespace WeChat.NET
{
    /// <summary>
    /// 微信消息接受必须实现的接口
    /// 实现接口后在MessageServer构造函数中注册
    /// </summary>
    public interface IWXMessageObserver
    {
        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="wxMsg">微信消息</param>
        void ReceiveMessage(WXMsg wxMsg);
    }
}