using System;
using System.Collections.Generic;
using System.Configuration;
using System.Messaging;
using System.Reflection;
using Common.Logging;
using Messages;
using Services.Ioc;

namespace DocViewerWinService
{
    public class MessageHandlerFacade : MessageRegister
    {
        private readonly ILog LOGGER = LogManager.GetCurrentClassLogger();
      
        private Assembly MessageTypeLoadingCache { get; set; }
        private readonly string MESSAGEASSEMBLYNAME = ConfigurationManager.AppSettings["DocViewer.MessageAssembly"];
        private const char SEPARATOR = ',';

     

        public void Handle(Message message)
        {
            var msgType = GetMessageType(message);
            var handlerType = GetHandlerType(msgType);
            var handler = ServiceActivator.Get(handlerType);

            message.Formatter = new XmlMessageFormatter(new[] { msgType });
            var body = message.Body;
            try
            {
                LOGGER.DebugFormat("Handler ({0}) handle message ({1})", msgType, handlerType);

                handlerType.GetMethod("Handle").Invoke(handler, new[] { body });
            }
            catch (Exception err)
            {
                LOGGER.Error("Could not invoke handle for handler: " + handlerType, err);
                throw;
            }
        }

        private Type GetHandlerType(Type msgType)
        {
            Type handlerType;
            if (_registeredHandlers.ContainsKey(msgType))
            {
                handlerType = _registeredHandlers[msgType];
            }
            else
            {
                handlerType = FindHandlerTypeInCurrentAssembly(msgType);
            }
            if (handlerType == null)
            {
                throw new ArgumentException("Message and Handler doesn't registered for message: " + msgType);
            }
            return handlerType;
        }

        private Type FindHandlerTypeInCurrentAssembly(Type msgType)
        {
            foreach (var handlerType in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var inter in handlerType.GetInterfaces())
                {
                    foreach (var arg in inter.GetGenericArguments())
                    {
                        if (arg == msgType)
                        {
                            Register(msgType, handlerType);
                            return handlerType;
                        }
                    }
                }
            }
            return null;
        }

        private Type GetMessageType(Message message)
        {
            Type msgType = null;

            if (MessageTypeLoadingCache != null)
            {
                msgType = MessageTypeLoadingCache.GetType(message.Label);
                if (msgType != null)
                {
                    return msgType;
                }
            }
            string[] assemblyNames = MESSAGEASSEMBLYNAME.Split(SEPARATOR);
            foreach (var assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                {
                    throw new NullReferenceException(string.Format("Not find the assembly:[{0}]", assemblyName));
                }
                msgType = assembly.GetType(message.Label);

                if (msgType != null)
                {
                    MessageTypeLoadingCache = assembly;
                    return msgType;
                }
            }

            if (msgType == null)
            {
                throw new ArgumentException(string.Format("Failed to resolve message type for message label [{0}]",
                                                          message.Label));
            }
            return msgType;
        }
    }

    public class MessageRegisteredTo
    {
        private readonly Type _msgType;
        private readonly MessageRegister _register;

        public MessageRegisteredTo(Type msgType, MessageRegister register)
        {
            this._msgType = msgType;
            this._register = register;
        }

        public void ToHandler<msgHandlerType>()
        {
            _register.Register(_msgType, typeof(msgHandlerType));
        }
    }

    public class MessageRegister
    {
        protected readonly Dictionary<Type, Type> _registeredHandlers = new Dictionary<Type, Type>();

        public MessageRegisteredTo Register<TMsgType>() where TMsgType : MessageBase
        {
            return new MessageRegisteredTo(typeof(TMsgType), this);
        }

        public void Register(Type msgType, Type msgHandlerType)
        {
            if (!_registeredHandlers.ContainsKey(msgType))
            {
                _registeredHandlers.Add(msgType, msgHandlerType);
            }
        }
    }
}