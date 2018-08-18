using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeChat.NET.LuckMoney;

namespace WeChat.NET.Objects
{
    /// <summary>
    /// 微信消息管理服务
    /// 所有需要接受微信消息的服务需要在构造函数中注册
    /// </summary>
    public class MessageServer
    {
        /// <summary>
        /// 观察者列表
        /// </summary>
        private List<IWXMessageObserver> observers = new List<IWXMessageObserver>();

#region 单一实例
        // 定义一个静态变量来保存类的实例
        private static MessageServer uniqueInstance;

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static MessageServer GetInstance()
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
                        uniqueInstance = new MessageServer();
                    }
                }
            }
            return uniqueInstance;
        }
#endregion

        private MessageServer()
        {
            LuckMessageServer luckMessageServer = new LuckMessageServer();
            this.AddObservers(luckMessageServer);
            
        }

        private void AddObservers(IWXMessageObserver iWXMessageObserver)
        {
            observers.Add(iWXMessageObserver);
        }

        private void RemoveObservers(IWXMessageObserver iWXMessageObserver)
        {
            observers.Remove(iWXMessageObserver);
        }

        /// <summary>
        /// 发送微信消息给其他服务
        /// </summary>
        /// <param name="msg"></param>
        public void notifyMessage(WXMsg msg)
        {
            foreach (IWXMessageObserver observer in observers)
            {
                observer.ReceiveMessage(msg);
            }
        }
    }
}
