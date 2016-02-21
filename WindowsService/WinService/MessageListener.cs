using System;
using System.Messaging;
using System.Security.Principal;
using System.Threading;
using Common.Logging;
using Messages;

namespace DocViewerWinService
{
    public class MessageListener
    {
        private readonly MessageHandlerFacade handlerFacade;
        private MessageQueue queue;
        private Thread thread;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private static readonly string LocalAdministratorsGroupName =
            new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).
                ToString();

        private static readonly string LocalEveryoneGroupName =
            new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();

        private static readonly string LocalAnonymousLogonName =
            new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();

        public MessageListener(MessageHandlerFacade handlerFacade)
        {
            this.handlerFacade = handlerFacade;
        }

        public void Start()
        {
            thread = new Thread(Initialize_MessageQueue);
            thread.Start();
        }

        private void Initialize_MessageQueue()
        {
            var msmq_address = MessageConfiguration.MessageAddress;

            _logger.DebugFormat("Initializing queue: {0}", msmq_address);

            try
            {
                if (!MessageQueue.Exists(msmq_address))
                {
                    _logger.DebugFormat("Queue: {0} doesn't exists.", msmq_address);
                    _logger.DebugFormat("Creating queue: {0}", msmq_address);
                    CreateQueue(msmq_address);
                }
            }
            catch (Exception err)
            {
                _logger.Error(string.Format("Could not Create queue {0}, process will be stopped.", msmq_address), err);
                thread.Abort();
                Environment.Exit(0);
            }

            queue = new MessageQueue(msmq_address);
            queue.ReceiveCompleted += Queue_ReceiveCompleted;
            queue.BeginReceive();
        }

        private void CreateQueue(string msmq_address)
        {
            var mq = MessageQueue.Create(msmq_address);
            mq.SetPermissions(LocalAdministratorsGroupName, MessageQueueAccessRights.FullControl,
                              AccessControlEntryType.Allow);
            mq.SetPermissions(LocalEveryoneGroupName,
                              MessageQueueAccessRights.WriteMessage | MessageQueueAccessRights.ReceiveMessage,
                              AccessControlEntryType.Allow);
            mq.SetPermissions(LocalAnonymousLogonName, MessageQueueAccessRights.WriteMessage,
                              AccessControlEntryType.Allow);
            _logger.DebugFormat("Queue created: {0}", msmq_address);
        }

        private void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var mq = (MessageQueue)sender;
            try
            {
                var message = mq.EndReceive(e.AsyncResult);
                _logger.DebugFormat("Receive Message with label {0}", message.Label);
                handlerFacade.Handle(message);
            }
            catch (Exception err)
            {
                _logger.ErrorFormat("Handle message with an error. {0}", err.Message);
            }

            mq.BeginReceive();
        }

        public void Stop()
        {
            _logger.Debug("Stopping message queue");

            queue.ReceiveCompleted -= Queue_ReceiveCompleted;
            queue.Dispose();

            thread.Abort();
        }
    }
}