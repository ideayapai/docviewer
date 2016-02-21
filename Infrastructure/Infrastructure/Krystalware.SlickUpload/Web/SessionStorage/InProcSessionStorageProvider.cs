using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using Krystalware.SlickUpload.Configuration;
using System.Reflection;
using System.Threading;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// An <see cref="ISessionStorageProvider" /> that stores upload session state in process.
    /// </summary>
    public class InProcSessionStorageProvider : SessionStorageProviderBase
    {
        static readonly Dictionary<string, UploadSession> _uploadSessions = new Dictionary<string, UploadSession>();
        static readonly Dictionary<string, Dictionary<string, UploadRequest>> _uploadRequests = new Dictionary<string, Dictionary<string, UploadRequest>>();

        protected static Dictionary<string, UploadSession> UploadSessions { get { return _uploadSessions; } }
        protected static Dictionary<string, Dictionary<string, UploadRequest>> UploadRequests { get { return _uploadRequests; } }

#if NET35
        static readonly ReaderWriterLockSlim _sessionsLock = new ReaderWriterLockSlim();
        static readonly ReaderWriterLockSlim _requestsLock = new ReaderWriterLockSlim();

        protected static ReaderWriterLockSlim SessionsLock { get { return _sessionsLock; } }
        protected static ReaderWriterLockSlim RequestsLock { get { return _requestsLock; } }
#else
        static readonly ReaderWriterLock _sessionsLock = new ReaderWriterLock();
        static readonly ReaderWriterLock _requestsLock = new ReaderWriterLock();

        protected static ReaderWriterLock SessionsLock { get { return _sessionsLock; } }
        protected static ReaderWriterLock RequestsLock { get { return _requestsLock; } }
#endif

        /// <summary>
        /// Creates a new instance of the <see cref="InProcSessionStorageProvider" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        public InProcSessionStorageProvider(SessionStorageProviderElement settings)
            : base(settings)
        { }

        /// <inheritdoc />
        public override void SaveSession(UploadSession session, bool isCreate)
        {
            // TODO: uncomment, add check to make sure object is the same
            //if (isCreate)
            //{
                try
                {
#if NET35
                    _sessionsLock.EnterWriteLock();
#else
                    _sessionsLock.AcquireWriterLock(-1);
#endif
                    _uploadSessions[session.UploadSessionId] = session;
                }
                finally
                {
#if NET35
                    _sessionsLock.ExitWriteLock();
#else
                    _sessionsLock.ReleaseWriterLock();
#endif
                }
            //}
        }

        /// <inheritdoc />
        public override UploadSession GetSession(string uploadSessionId)
        {
            try
            {
#if NET35
                _sessionsLock.EnterReadLock();
#else
                _sessionsLock.AcquireReaderLock(-1);
#endif

                UploadSession session;

                if (!_uploadSessions.TryGetValue(uploadSessionId, out session))
                    session = null;

                return session;
            }
            finally
            {
#if NET35
                _sessionsLock.ExitReadLock();
#else
                _sessionsLock.ReleaseReaderLock();
#endif
            }
        }

        /// <inheritdoc />
        public override void RemoveSession(string uploadSessionId)
        {
            try
            {
#if NET35
                _requestsLock.EnterWriteLock();
#else
                _requestsLock.AcquireWriterLock(-1);
#endif

                _uploadRequests.Remove(uploadSessionId);
            }
            finally
            {
#if NET35
                _requestsLock.ExitWriteLock();
#else
                _requestsLock.ReleaseWriterLock();
#endif
            }

            try
            {
#if NET35
                _sessionsLock.EnterWriteLock();
#else
                _sessionsLock.AcquireWriterLock(-1);
#endif

                _uploadSessions.Remove(uploadSessionId);
            }
            finally
            {
#if NET35
                _sessionsLock.ExitWriteLock();
#else
                _sessionsLock.ReleaseWriterLock();
#endif
            }
        }

        /// <inheritdoc />
        public override void SaveRequest(UploadRequest request, bool isCreate)
        {
            // TODO: uncomment, add check to make sure object is the same
            //if (isCreate)
            //{
                try
                {
#if NET35
                    _requestsLock.EnterUpgradeableReadLock();
#else
                    _requestsLock.AcquireReaderLock(-1);
#endif
                    Dictionary<string, UploadRequest> requests;

                    if (!_uploadRequests.TryGetValue(request.UploadSessionId, out requests))
                    {
                        requests = new Dictionary<string, UploadRequest>();

                        requests[request.UploadRequestId] = request;

#if !NET35
                        // TODO: does this break something?
                        LockCookie cookie = new LockCookie();
#endif

                        try
                        {
#if NET35
                            _requestsLock.EnterWriteLock();
#else
                            cookie = _requestsLock.UpgradeToWriterLock(-1);
#endif

                            _uploadRequests[request.UploadSessionId] = requests;
                        }
                        finally
                        {
#if NET35
                            _requestsLock.ExitWriteLock();
#else
                            _requestsLock.DowngradeFromWriterLock(ref cookie);
#endif
                        }
                    }
                    else
                    {
                        lock (requests)
                            requests[request.UploadRequestId] = request;
                    }
                }
                finally
                {
#if NET35
                _requestsLock.ExitUpgradeableReadLock();
#else
                _requestsLock.ReleaseReaderLock();
#endif  
                }
            //}
        }

        /// <inheritdoc />
        public override UploadRequest GetRequest(string uploadSessionId, string uploadRequestId)
        {
            try
            {
#if NET35
                _requestsLock.EnterReadLock();
#else
                _requestsLock.AcquireReaderLock(-1);
#endif

                Dictionary<string, UploadRequest> requests;
                UploadRequest request = null;

                if (_uploadRequests.TryGetValue(uploadSessionId, out requests))
                {
                    lock (requests)
                    {
                        if (!requests.TryGetValue(uploadRequestId, out request))
                            request = null;
                    }
                }

                return request;
            }
            finally
            {
#if NET35
                _requestsLock.ExitReadLock();
#else
                _requestsLock.ReleaseReaderLock();
#endif  
            }
        }

        /*public void RemoveRequest(string uploadSessionId, string uploadRequestId)
        {
            try
            {
                _requestsLock.AcquireReaderLock(-1);

                Dictionary<string, UploadRequest> requests;

                if (_uploadRequests.TryGetValue(uploadSessionId, out requests))
                {
                    lock (requests)
                        requests.Remove(uploadRequestId);
                }
            }
            finally
            {
                _requestsLock.ReleaseReaderLock();
            }
        }*/

        /// <inheritdoc />
        public override IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId)
        {
            try
            {
#if NET35
                _requestsLock.EnterReadLock();
#else
                _requestsLock.AcquireReaderLock(-1);
#endif

                Dictionary<string, UploadRequest> requests;
                List<UploadRequest> returnList = new List<UploadRequest>();

                if (_uploadRequests.TryGetValue(uploadSessionId, out requests))
                {
                    lock (requests)
                        returnList.AddRange(requests.Values);
                }

                return returnList;
            }
            finally
            {
#if NET35
                _requestsLock.ExitReadLock();
#else
                _requestsLock.ReleaseReaderLock();
#endif                
            }
        }

        /// <inheritdoc />
        public override IEnumerable<UploadSession> GetStaleSessions(DateTime staleBefore)
        {
            try
            {
#if NET35
                _sessionsLock.EnterReadLock();
                _requestsLock.EnterReadLock();
#else
                _sessionsLock.AcquireReaderLock(-1);
                _requestsLock.AcquireReaderLock(-1);
#endif

                List<UploadSession> returnList = new List<UploadSession>();

                foreach (UploadSession session in _uploadSessions.Values)
                {
                    DateTime lastUpdated = session.Started;

                    Dictionary<string, UploadRequest> requests;

                    if (_uploadRequests.TryGetValue(session.UploadSessionId, out requests))
                    {
                        foreach (UploadRequest staleRequest in requests.Values)
                        {
                            if (staleRequest.LastUpdated > lastUpdated)
                                lastUpdated = staleRequest.LastUpdated;
                        }
                    }

                    if (lastUpdated < staleBefore)
                        returnList.Add(session);
                }

                return returnList;
            }
            finally
            {
#if NET35
                _sessionsLock.ExitReadLock();
                _requestsLock.ExitReadLock();
#else
                _sessionsLock.ReleaseReaderLock();
                _requestsLock.ReleaseReaderLock();
#endif
            }
        }
    }
}