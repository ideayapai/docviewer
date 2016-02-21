using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// Represents a contract for a session storage provider class that stores and retrieves <see cref="UploadSession" /> and <see cref="UploadRequest" /> objects for uploads in progress.
    /// </summary>
    public interface ISessionStorageProvider
    {
        /// <summary>
        /// Persists the specified <see cref="UploadSession" /> object.
        /// </summary>
        /// <param name="session">The <see cref="UploadSession" /> to save.</param>
        /// <param name="isCreate">A boolean that specifies whether this the initial create for this session or an update.</param>
        void SaveSession(UploadSession session, bool isCreate);
        /// <summary>
        /// Retrieves the <see cref="UploadSession" /> object for the specified upload session id.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id for which to retrieve an <see cref="UploadSession" />.</param>
        /// <returns>The <see cref="UploadSession" /> object retrieved, or null if no matching session id was found.</returns>
        UploadSession GetSession(string uploadSessionId);
        /// <summary>
        /// Removes the <see cref="UploadSession" /> object associated with the specified upload session id, as well as any related <see cref="UploadRequest"/> objects.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id for which to remove an <see cref="UploadSession" />.</param>
        void RemoveSession(string uploadSessionId);

        /// <summary>
        /// Persists the specified <see cref="UploadRequest" /> object.
        /// </summary>
        /// <param name="request">The <see cref="UploadRequest" /> to save.</param>
        /// <param name="isCreate">A boolean that specifies whether this the initial create for this session or an update.</param>
        void SaveRequest(UploadRequest request, bool isCreate);
        /// <summary>
        /// Retrieves the <see cref="UploadRequest" /> object for the specified upload session id and upload request id.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id for which to retrieve an <see cref="UploadRequest" />.</param>
        /// <param name="uploadRequestId">The upload request id for which to retrieve an <see cref="UploadRequest" />.</param>
        /// <returns>The <see cref="UploadRequest" /> object retrieved, or null if no matching session id and request id was found.</returns>
        UploadRequest GetRequest(string uploadSessionId, string uploadRequestId);
        //void RemoveRequest(string uploadSessionId, string uploadRequestId);

        /// <summary>
        /// Returns an enumeration of all <see cref="UploadRequest" />s for the specified upload session id.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id for which to retrieve an enumeration of <see cref="UploadRequest" />s.</param>
        /// <returns>An enumeration of all <see cref="UploadRequest" />s for the specified upload session id.</returns>
        IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId);

        /// <summary>
        /// Returns an enumeration of all <see cref="UploadSession" />s that haven't been updated since the specified <see cref="DateTime" />.
        /// </summary>
        /// <param name="staleBefore">The <see cref="DateTime" /> before which sessions are considered stale.</param>
        /// <returns>An enumeration of all <see cref="UploadSession" />s which are considered stale.</returns>
        IEnumerable<UploadSession> GetStaleSessions(DateTime staleBefore);
    }
}
