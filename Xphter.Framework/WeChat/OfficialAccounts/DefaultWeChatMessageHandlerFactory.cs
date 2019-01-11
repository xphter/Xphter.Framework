using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xphter.Framework.Collections;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class DefaultWeChatMessageHandlerFactory : IWeChatMessageHandlerFactory {
        public DefaultWeChatMessageHandlerFactory() {
            this.m_messageHandlers = new Dictionary<string, AggregateWeChatMessageHandler>();
            this.m_eventHandlers = new Dictionary<string, AggregateWeChatMessageHandler>();
        }

        private IDictionary<string, AggregateWeChatMessageHandler> m_messageHandlers;
        private IDictionary<string, AggregateWeChatMessageHandler> m_eventHandlers;

        public void RegisterHandlers(params Assembly[] assemblies) {
            if(assemblies == null || assemblies.Length == 0) {
                return;
            }

            foreach(IWeChatMessageHandler item in TypeUtility.LoadInstances<IWeChatMessageHandler>(assemblies)) {
                this.RegisterHandler(item);
            }
        }

        public void RegisterHandler(IWeChatMessageHandler handler) {
            if(handler == null) {
                throw new ArgumentNullException("handler");
            }

            WeChatMessageHandlerAttribute[] messageAttributes = (WeChatMessageHandlerAttribute[]) handler.GetType().GetCustomAttributes(typeof(WeChatMessageHandlerAttribute), true);
            WeChatEventHandlerAttribute[] eventAttributes = (WeChatEventHandlerAttribute[]) handler.GetType().GetCustomAttributes(typeof(WeChatEventHandlerAttribute), true);

            foreach(WeChatMessageHandlerAttribute item in messageAttributes) {
                if(!this.m_messageHandlers.ContainsKey(item.MessageType)) {
                    this.m_messageHandlers[item.MessageType] = new AggregateWeChatMessageHandler();
                }

                this.m_messageHandlers[item.MessageType].AddHanlder(handler);
            }

            foreach(WeChatEventHandlerAttribute item in eventAttributes) {
                if(!this.m_eventHandlers.ContainsKey(item.EventType)) {
                    this.m_eventHandlers[item.EventType] = new AggregateWeChatMessageHandler();
                }

                this.m_eventHandlers[item.EventType].AddHanlder(handler);
            }
        }

        #region IWeChatMessageHandlerFactory Members

        public IWeChatMessageHandler GetHandler(IWeChatMessage message, IOfficialAccountService service) {
            IWeChatMessageHandler handler = null;

            switch(message.MsgType) {
                case WeChatMessageTypes.EVENT:
                    IWeChatEventMessage eventMessage = (IWeChatEventMessage) message;
                    if(this.m_eventHandlers.ContainsKey(eventMessage.Event)) {
                        handler = this.m_eventHandlers[eventMessage.Event];
                    }
                    break;
                default:
                    if(this.m_messageHandlers.ContainsKey(message.MsgType)) {
                        handler = this.m_messageHandlers[message.MsgType];
                    }
                    break;
            }

            return handler;
        }

        #endregion
    }
}
