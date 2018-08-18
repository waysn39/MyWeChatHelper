using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.NET.HTTP;
using WeChat.NET.Objects;

namespace WeChat.NET.LuckMoney
{
    class LuckMessage
    {
        private LuckType type;
        private string sendTo;
        private string sendFrom;
        private string content;
        private int superAdmin = -1;

        public LuckMessage(LuckType luckType,string sendTo, string sendFrom,string content) {
            this.type = luckType;
            this.sendTo = sendTo;
            this.sendFrom = sendFrom;
            this.content = content;
        }

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

        private void SendText(string message)
        {
            WXService.GetWXService().SendMsg(message, UserManager.GetInstance().GetCurrentUser().UserName, RealSendTo(), 1);
        }

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

        private string RealSendTo()
        {
            return IsFormMySelf() ? sendTo : sendFrom;
        }

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

    }

    public enum LuckType {
        RobLK = 0,
        GetLK = 1,
        None =2
    }

}
