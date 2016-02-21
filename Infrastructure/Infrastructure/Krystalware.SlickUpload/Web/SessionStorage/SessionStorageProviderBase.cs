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
    /// Exposes an abstract base class for <see cref="ISessionStorageProvider"/>s.
    /// </summary>
    public abstract class SessionStorageProviderBase : ISessionStorageProvider
    {
        SessionStorageProviderElement _settings;

        /// <summary>
        /// Gets the settings for this session storage provider.
        /// </summary>
        protected SessionStorageProviderElement Settings { get { return _settings; } }

        /// <summary>
        /// Creates a new instance of the <see cref="SessionStorageProviderBase" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        protected SessionStorageProviderBase(SessionStorageProviderElement settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public abstract void SaveSession(UploadSession session, bool isCreate);
        /// <inheritdoc />
        public abstract UploadSession GetSession(string uploadSessionId);
        /// <inheritdoc />
        public abstract void RemoveSession(string uploadSessionId);

        /// <inheritdoc />
        public abstract void SaveRequest(UploadRequest request, bool isCreate);
        /// <inheritdoc />
        public abstract UploadRequest GetRequest(string uploadSessionId, string uploadRequestId);

        /// <inheritdoc />
        public abstract IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId);
        /// <inheritdoc />
        public abstract IEnumerable<UploadSession> GetStaleSessions(DateTime staleBefore);
    }
}
