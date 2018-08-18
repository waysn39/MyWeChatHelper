using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.NET.HTTP;
using WeChat.NET.Objects;

namespace WeChat.NET.LuckMoney
{
    /// <summary>
    /// 红包消息服务
    /// </summary>
    class LuckMessageServer : IWXMessageObserver
    {
        private LuckType type;
        private string sendTo;
        private string sendFrom;
        private string content;
        private int superAdmin = -1;
        public LuckMessageServer() {
        }
        /// <summary>
        /// 主流程
        /// </summary>
        public void SendLuckMessage() {
            if (NeedSendMsg())
            {
                SendText(@"红包助手：即将执行，全过程需要5~30秒。");
                bool isError = false;
                string LKmsg = string.Empty;
                if (type == LuckType.RobLK)
                {
                    SendText(@"红包助手：机器人正在抢红包...");
                    LKmsg = LuckMoneyHelper.RobLuckMoney(content,ref isError);
                }
                if (isError || type == LuckType.GetLK)
                {
                    if (isError)
                    {
                        SendText(@"红包助手：存在异常，尝试更换其他方式，异常信息（" + LKmsg + ")");
                    }
                    SendText(@"红包助手:正在为您搜索网络红包...");
                    LKmsg = LuckMoneyHelper.GetLuckMoney(ref isError);
                }


                if (!isError)
                {
                    SendText(@"红包助手：请点击您发送的红包或下方网址领取您的大红包");
                    SendText(LKmsg);
                }
                else
                {
                    SendText(@"红包助手：请稍后再试。异常信息（" + LKmsg + ")");
                }

            }
        }

        /// <summary>
        /// 向发送者发送文本消息
        /// </summary>
        /// <param name="message">文本</param>
        private void SendText(string message)
        {
            WXService.GetWXService().SendMsg(message, UserManager.GetInstance().GetCurrentUser().UserName, RealSendTo(), 1);
        }

        /// <summary>
        /// 消息是否来自于自己
        /// </summary>
        /// <returns></returns>
        private bool IsFormMySelf() {
            bool result = false;
            if (superAdmin < 0)
            {
                if (sendFrom.Equals(UserManager.GetInstance().GetCurrentUser().UserName))
                {
                    result = true;
                    superAdmin = 1;
                }
                else
                {
                    superAdmin = 0;
                }
            }
            else
            {
                result = superAdmin == 0 ? false : true;
            }
            return result;
        }

        /// <summary>
        /// 最终确定的发送给的人
        /// </summary>
        /// <returns></returns>
        private string RealSendTo()
        {
            return IsFormMySelf() ? sendTo : sendFrom;
        }

        /// <summary>
        /// 权限管理
        /// </summary>
        /// <returns></returns>
        private bool NeedSendMsg() {
            bool result = false;
            if (IsFormMySelf())
                result = true;
            else
            {
                List<object> list = UserManager.GetInstance().GetAllGroup();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is WXUser)
                    {
                        WXUser wX = (WXUser)list[i];
                        if (wX.UserName.Equals(RealSendTo()))
                        {
                            if (wX.NickName.Contains(@"美团，饿了么红包分享群"))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 响应MessageServer的消息
        /// </summary>
        /// <param name="wxMsg"></param>
        public void ReceiveMessage(WXMsg wxMsg)
        {
            LuckType luckType = LuckType.None;
            if ((wxMsg.Type == 49 && wxMsg.Msg.Contains(@"饿了么拼手气")))
                luckType = LuckType.RobLK;
            if (wxMsg.Msg.Contains(@"#红包"))
                luckType = LuckType.GetLK;
            if (luckType != LuckType.None)
            {
                this.type = luckType;
                this.sendTo = wxMsg.To;
                this.sendFrom = wxMsg.From;
                this.content = wxMsg.Msg;
                this.SendLuckMessage();
            }
        }
    }

    /// <summary>
    /// 获取最大红包方式
    /// </summary>
    public enum LuckType {
        /// <summary>
        /// 抢红包
        /// </summary>
        RobLK = 0,
        /// <summary>
        /// 获取红包
        /// </summary>
        GetLK = 1,
        /// <summary>
        /// 无
        /// </summary>
        None =2
    }

}
