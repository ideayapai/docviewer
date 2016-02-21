using System;
using System.Messaging;
using Common.Logging;

namespace Messages
{
    /// <summary>
    /// 调用消息队列发送
    /// </summary>
    public class MessageBus
    {
        private readonly MessageQueue _messageQueue;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public MessageBus()
        {
            _messageQueue = new MessageQueue(MessageConfiguration.MessageAddress);
        }

        public virtual void Send(MessageBase input)
        {
            _logger.Debug("Send Message.");

            try
            {
                Message message = GetMessage(input);
                _messageQueue.Send(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
           
        }

        private Message GetMessage(MessageBase message)
        {
            return new Message
                       {
                           Formatter = new XmlMessageFormatter(new[] {message.GetType()}),
                           Label = message.GetType().FullName,
                           Body = message
                       };
        }
    }
}
