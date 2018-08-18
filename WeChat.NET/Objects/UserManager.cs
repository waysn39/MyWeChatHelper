using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.NET.HTTP;

namespace WeChat.NET.Objects
{
    public  class UserManager
    {
        /// <summary>
        /// 当前登录微信用户
        /// </summary>
        private  WXUser CurrentUser;
        /// <summary>
        /// 所有联系人列表
        /// </summary>
        private  List<object> AllFriend = new List<object>();
        /// <summary>
        /// 最近联系人列表
        /// </summary>
        private  List<object> RecentFriend = new List<object>();

        /// <summary>
        /// 组对话列表
        /// </summary>
        private List<object> AllGroup = new List<object>();

        /// <summary>
        /// 微信服务
        /// </summary>
        private WXService Service;

        // 定义一个静态变量来保存类的实例
        private static UserManager uniqueInstance;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static UserManager GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new UserManager();
                    }
                }
            }
            return uniqueInstance;
        }

        // 定义私有构造函数，使外界不能创建该类实例
        private UserManager()
        {
            Service = WXService.GetWXService();
            JObject initResult = Service.GetInitInfo();  //初始化
            init(initResult);

        }

        private void init(JObject initResult) {
            if (initResult != null) {
                CreateMe(initResult);
                CreateRecentUser(initResult);
                CreateAllUser();
            }
        }

        private void CreateMe(JObject initResult) {
            CurrentUser = new WXUser();
            CurrentUser.UserName = initResult["User"]["UserName"].ToString();
            CurrentUser.City = "";
            CurrentUser.HeadImgUrl = initResult["User"]["HeadImgUrl"].ToString();
            CurrentUser.NickName = initResult["User"]["NickName"].ToString();
            CurrentUser.Province = "";
            CurrentUser.PYQuanPin = initResult["User"]["PYQuanPin"].ToString();
            CurrentUser.RemarkName = initResult["User"]["RemarkName"].ToString();
            CurrentUser.RemarkPYQuanPin = initResult["User"]["RemarkPYQuanPin"].ToString();
            CurrentUser.Sex = initResult["User"]["Sex"].ToString();
            CurrentUser.Signature = initResult["User"]["Signature"].ToString();
        }

        private void CreateRecentUser(JObject initResult) {
            foreach (JObject contact in initResult["ContactList"])  //部分好友名单
            {
                WXUser user = new WXUser();
                user.UserName = contact["UserName"].ToString();
                user.City = contact["City"].ToString();
                user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                user.NickName = contact["NickName"].ToString();
                user.Province = contact["Province"].ToString();
                user.PYQuanPin = contact["PYQuanPin"].ToString();
                user.RemarkName = contact["RemarkName"].ToString();
                user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                user.Sex = contact["Sex"].ToString();
                user.Signature = contact["Signature"].ToString();
                RecentFriend.Add(user);
            }
        }

        private void CreateAllUser() {
            JObject contactResult = Service.GetContact(); //通讯录
            if (contactResult != null)
            {
                foreach (JObject contact in contactResult["MemberList"])  //完整好友名单
                {
                    WXUser user = new WXUser();
                    user.UserName = contact["UserName"].ToString();
                    user.City = contact["City"].ToString();
                    user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                    user.NickName = contact["NickName"].ToString();
                    user.Province = contact["Province"].ToString();
                    user.PYQuanPin = contact["PYQuanPin"].ToString();
                    user.RemarkName = contact["RemarkName"].ToString();
                    user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                    user.Sex = contact["Sex"].ToString();
                    user.Signature = contact["Signature"].ToString();

                    AllFriend.Add(user);
                    if (user.UserName.StartsWith(@"@@"))
                    {
                        AllGroup.Add(user);
                    }
                }
            }
        }

        public WXUser GetCurrentUser(){
            return CurrentUser;

        }

        public List<object> GetRecentFriend() {
            return RecentFriend;
        }

        public List<object> GetAllFriend()
        {
            return AllFriend;
        }

        public List<object> GetAllGroup() {
            return AllGroup;
        }

    }
}
