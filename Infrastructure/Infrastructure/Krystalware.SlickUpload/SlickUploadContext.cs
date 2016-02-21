using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Krystalware.SlickUpload.Configuration;
using System.Configuration;
using Krystalware.SlickUpload.Web.SessionStorage;
using System.Web.Configuration;
using Krystalware.SlickUpload.Web;

namespace Krystalware.SlickUpload
{
    /// <summary>
    /// Provides methods for interacting with <see cref="UploadSession" />s and <see cref="UploadRequest" />s.
    /// </summary>
    public static class SlickUploadContext
    {
        // TODO: is this safe?
        static readonly ISessionStorageProvider _provider = Config.SessionStorageProvider.Create();

        /// <summary>
        /// Gets the currently configured <see cref="ISessionStorageProvider" />.
        /// </summary>
        public static ISessionStorageProvider SessionStorageProvider { get { return _provider; } }

        internal static void InsertRequest(UploadRequest request)
        {
            SessionStorageProvider.SaveRequest(request, true);
        }

        internal static void UpdateRequest(UploadRequest request, bool isForceUpdate)
        {
            if (isForceUpdate || (DateTime.Now - request.LastUpdated).TotalSeconds > Config.SessionStorageProvider.UpdateInterval)
            {
                request.LastUpdated = DateTime.Now;

                SessionStorageProvider.SaveRequest(request, false);
            }
        }
        
        /// <summary>
        /// Commits the specified <see cref="UploadSession" />. This completes the session so it won't be automatically
        /// rolled back when the session timeout expires.
        /// </summary>
        /// <param name="session">The <see cref="UploadSession" /> to commit.</param>
        public static void CommitSession(UploadSession session)
        {
            //session.State = UploadState.Complete;

            //SessionStorageProvider.SaveSession(session, false);
            
            EndSession(session);
        }

        /// <summary>
        /// Rolls back the specified <see cref="UploadSession" />. This cleans up the session storage for this session and
        /// requests. This also removes any files that were uploaded during the session.
        /// </summary>
        /// <param name="session">The <see cref="UploadSession" /> to roll back.</param>
        public static void RollbackSession(UploadSession session)
        {
            foreach (UploadRequest request in session.UploadRequests)
                CleanupRequest(request);

            EndSession(session);
        }

        /// <summary>
        /// Updates the specified <see cref="UploadSession" /> in the current session storage provider.
        /// </summary>
        /// <param name="session">The <see cref="UploadSession" /> to update.</param>
        public static void UpdateSession(UploadSession session)
        {
            SessionStorageProvider.SaveSession(session, false);
        }

        internal static void CompleteSession(UploadSession session, UploadErrorType errorType)
        {
            bool hasErrored = (errorType != UploadErrorType.None);

            int successfulFiles = 0;

            if (!hasErrored)
            {
                foreach (UploadRequest request in session.UploadRequests)
                {
                    if (session.CancelledRequests != null && Array.IndexOf<string>(session.CancelledRequests, request.UploadRequestId) != -1)
                        CancelRequest(request);

                    // TODO: we assume if it is uploading that it is almost done and do nothing. Instead, we should block until it becomes not uploading,
                    // or nuke it if it doesn't finish shortly
                    /*if (request.State == UploadState.Uploading)
                        throw new Exception("Request " + request.UploadRequestId + " is still processing.");
                    if (request.State == UploadState.Uploading)
                        CancelRequest(request);*/
                    // TODO: should we consider cancels as not errored? I guess not, because we still consider it an error if everything was cancelled. Maybe revisit when we do AllowPartialError
                    if (request.ErrorType != UploadErrorType.None)
                        hasErrored = true;
                    else
                        successfulFiles += request._uploadedFilesInternal.Count;
                }
            }

            if (hasErrored && errorType == UploadErrorType.None && session.UploadProfile.AllowPartialError && successfulFiles > 0)
                hasErrored = false;

            if (hasErrored && errorType == UploadErrorType.None)
            {
                // Try to find an error
                foreach (UploadRequest request in session.UploadRequests)
                {
                    if (request.ErrorType != UploadErrorType.None)
                    {
                        errorType = request.ErrorType;

                        break;
                    }
                }

                if (errorType == UploadErrorType.None)
                    errorType = UploadErrorType.Other;
            }

            session.State = !hasErrored ? UploadState.Complete : UploadState.Error;
            session.ErrorType = errorType;

            UpdateSession(session);

            // TODO: what happens here if a request is still in progress for some reason. Can we guarantee that never happens?
            foreach (UploadRequest request in session.UploadRequests)
            {
                bool isFailedRequest = (session.FailedRequests != null && Array.IndexOf<string>(session.FailedRequests, request.UploadRequestId) != -1);

                if (hasErrored || request.ErrorType != UploadErrorType.None || isFailedRequest)
                {
                    if (isFailedRequest)
                    {
                        request.State = UploadState.Error;

                        if (request.ErrorType == UploadErrorType.None)
                            request.ErrorType = UploadErrorType.FailedByClient;
                    }

                    CleanupRequest(request);
                }
            }
        }

        internal static void EndSession(UploadSession session)
        {
            SessionStorageProvider.RemoveSession(session.UploadSessionId);

            if (CurrentUploadSessionInternal != null && CurrentUploadSessionInternal.UploadSessionId == session.UploadSessionId)
                CurrentUploadSessionInternal = null;
        }

        internal static void CancelRequest(UploadRequest request)
        {
            try
            {
                if (request.State != UploadState.Uploading)
                    CleanupRequest(request);
            }
            catch (Exception ex)
            {
                // TODO: store this always?
                if (request.Error == null)
                {
                    request.Error = ex;
                    request.ErrorType = UploadErrorType.Other;
                }
            }
            finally
            {
                request.State = UploadState.Error;
                request.ErrorType = UploadErrorType.Cancelled;

                UpdateRequest(request, true);
            }
        }

        internal static void CleanupRequest(UploadRequest request)
        {
            try
            {
                // If this request hasn't had time to autoclose, wait a tick
                if ((DateTime.Now - request.LastUpdated).TotalSeconds < 2)
                    System.Threading.Thread.Sleep(2000);

                foreach (UploadedFile file in new List<UploadedFile>(request._uploadedFilesInternal))
                    request.UploadStreamProvider.RemoveOutput(file);
            }
            catch (Exception ex)
            {
                // TODO: store this always?
                if (request.Error == null)
                {
                    request.Error = new ApplicationException("Error cleaning up request " + request.UploadRequestId + ". Request may still be processing, but should self cleanup when complete.", ex);
                    request.ErrorType = UploadErrorType.Other;

                    UpdateRequest(request, true);
                }
            }
        }

        internal static UploadRequest CurrentUploadRequest
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context != null)
                    return context.Items["CurrentUploadRequest"] as UploadRequest;
                else
                    return null;
            }
            set
            {
                HttpContext context = HttpContext.Current;

                if (context != null)
                {
                    if (context.Items["CurrentUploadRequest"] != null)
                        throw new Exception("CurrentUploadRequest already set for this context. Use SlickUploadContext.CurrentUploadRequest to retrieve the current UploadRequest instance.");

                    context.Items["CurrentUploadRequest"] = value;
                }
            }
        }

        /// <summary>
        /// Gets the current <see cref="UploadSession "/>.
        /// </summary>
        public static UploadSession CurrentUploadSession
        {
            get
            {
                HttpContext context = HttpContext.Current;
                UploadSession session = null;

                if (context != null)
                {
                    session = CurrentUploadSessionInternal;

                    if (session == null)
                    {
                        string uploadSessionId = context.Request["uploadSessionId"];

                        if (string.IsNullOrEmpty(uploadSessionId))
                            uploadSessionId = context.Request["kw_uploadSessionId"];

                        if (!string.IsNullOrEmpty(uploadSessionId))
                            session = SessionStorageProvider.GetSession(uploadSessionId);
                    }

                    if (session != null)
                    {
                        string sourceConnectorId = context.Request["kw_sourceConnectorId"];

                        if (string.IsNullOrEmpty(sourceConnectorId))
                            sourceConnectorId = context.Request["sourceConnectorId"];

                        if (!string.IsNullOrEmpty(sourceConnectorId))
                            session.SourceUploadConnectorId = sourceConnectorId;

                        string uploadErrorType = context.Request["uploadErrorType"];

                        if (string.IsNullOrEmpty(uploadErrorType))
                            uploadErrorType = context.Request["kw_uploadErrorType"];

                        UploadErrorType errorType;

                        try
                        {
                            errorType = (UploadErrorType)Enum.Parse(typeof(UploadErrorType), uploadErrorType, true);
                        }
                        catch
                        {
                            // TODO: perhaps should be other?
                            errorType = UploadErrorType.None;
                        }

                        string failedRequestsString = context.Request["failedRequests"];

                        if (string.IsNullOrEmpty(failedRequestsString))
                            failedRequestsString = context.Request["kw_failedRequests"];

                        if (!string.IsNullOrEmpty(failedRequestsString))
                            session.FailedRequests = failedRequestsString.Split(',');

                        string cancelledRequestsString = context.Request["cancelledRequests"];

                        if (string.IsNullOrEmpty(cancelledRequestsString))
                            cancelledRequestsString = context.Request["kw_cancelledRequests"];

                        if (!string.IsNullOrEmpty(cancelledRequestsString))
                            session.CancelledRequests = cancelledRequestsString.Split(',');

                        CompleteSession(session, errorType);

                        // populate form values
                        // TODO: do this on upload once we implement form data passing on upload
                        // TODO: should we save this for other requests?
                        foreach (UploadedFile file in session.UploadedFiles)
                        {
                            string prefix = file.SourceElement + "_";

                            foreach (string key in context.Request.Form.AllKeys)
                            {
                                if (!string.IsNullOrEmpty(key) && key.StartsWith(prefix))
                                    file.Data[key.Substring(prefix.Length)] = context.Request.Form[key];
                            }
                        }

                        string uploadData = context.Request["kw_uploadData"];

                        if (string.IsNullOrEmpty(uploadData))
                            uploadData = context.Request.Headers["X-SlickUpload-Data"];

                        if (!string.IsNullOrEmpty(uploadData))
                            MimeHelper.ParseQueryStringToDictionary(uploadData, session.Data);

                        CurrentUploadSessionInternal = session;
                    }
                }

                return session;
            }
        }

        internal static UploadSession CurrentUploadSessionInternal
        {
            get
            {
                HttpContext context = HttpContext.Current;
                UploadSession session = null;

                if (context != null)
                    session = context.Items["CurrentUploadSession"] as UploadSession;

                return session;
            }
            set
            {
                HttpContext context = HttpContext.Current;

                if (context != null)
                    context.Items["CurrentUploadSession"]  = value;
            }
        }

        /// <summary>
        /// Gets the current <see cref="SlickUploadSection "/> with the current SlickUpload configuration.
        /// </summary>
        public static SlickUploadSection Config
        {
            get
            {
                // TODO: maybe make this static instead of storing in context?
                HttpContext context = HttpContext.Current;
                SlickUploadSection section = null;
                
                if (context != null)
                    section = HttpContext.Current.Items["SlickUploadSection"] as SlickUploadSection;

                if (section == null)
                {
                    string requestPath = null;

                    if (context != null)
                    {
                        try
                        {
                           requestPath = context.Request.CurrentExecutionFilePath;
                        }
                        catch
                        { }
                    }

                    if (requestPath != null)
                    {
                        section = WebConfigurationManager.GetSection("slickUpload", requestPath) as SlickUploadSection;
                    }
                    else
                    {
                        section = WebConfigurationManager.GetSection("slickUpload") as SlickUploadSection;
                    }                                        

                    if (section == null)
                        section = new SlickUploadSection();

                    if (context != null)
                        context.Items["SlickUploadSection"] = section;
                }

                return section;
            }
        }
    }
}
