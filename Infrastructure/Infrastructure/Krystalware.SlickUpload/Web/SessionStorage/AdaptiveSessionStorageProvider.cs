using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using Krystalware.SlickUpload.Configuration;
using System.Web.Configuration;
using System.Web.SessionState;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// An <see cref="ISessionStorageProvider" /> that delegates to <see cref="InProcSessionStorageProvider" /> if session state mode is Off or InProc, and <see cref="SessionStateSessionStorageProvider" /> otherwise.
    /// </summary>
    public class AdaptiveSessionStorageProvider : SessionStorageProviderBase
    {
        ISessionStorageProvider _provider;

        /// <summary>
        /// Creates a new instance of the <see cref="AdaptiveSessionStorageProvider" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        public AdaptiveSessionStorageProvider(SessionStorageProviderElement settings)
            : base(settings)
        {
            // TODO: error handle?
            SessionStateSection section = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

            if (section.Mode == SessionStateMode.Off || section.Mode == SessionStateMode.InProc)
                _provider = new InProcSessionStorageProvider(settings);
            else
                _provider = new SessionStateSessionStorageProvider(settings);
        }

        /// <inheritdoc />
        public override void SaveSession(UploadSession session, bool isCreate)
        {
            _provider.SaveSession(session, isCreate);
        }

        /// <inheritdoc />
        public override UploadSession GetSession(string uploadSessionId)
        {
            return _provider.GetSession(uploadSessionId);
        }

        /// <inheritdoc />
        public override void RemoveSession(string uploadSessionId)
        {
            _provider.RemoveSession(uploadSessionId);
        }

        /// <inheritdoc />
        public override void SaveRequest(UploadRequest request, bool isCreate)
        {
            _provider.SaveRequest(request, isCreate);
        }

        /// <inheritdoc />
        public override UploadRequest GetRequest(string uploadSessionId, string uploadRequestId)
        {
            return _provider.GetRequest(uploadSessionId, uploadRequestId);
        }

        //public override void RemoveRequest(string uploadSessionId, string uploadRequestId)
        //{
        //    _provider.RemoveRequest(uploadSessionId, uploadRequestId);
        //}

        /// <inheritdoc />
        public override IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId)
        {
            return _provider.GetRequestsForSession(uploadSessionId);
        }

        /// <inheritdoc />
        public override IEnumerable<UploadSession> GetStaleSessions(DateTime staleAfter)
        {
            return _provider.GetStaleSessions(staleAfter);
        }

        internal ISessionStorageProvider InternalProvider
        {
            get { return _provider; }
        }
    }
}
