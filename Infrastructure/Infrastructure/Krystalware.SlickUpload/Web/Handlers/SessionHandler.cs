using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that reads and writes to session state.
    /// </summary>
    public class SessionHandler : IHttpHandler, IRequiresSessionState
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string command = context.Request.QueryString["command"];

                if (string.IsNullOrEmpty(command))
                    throw new HttpException(500, "command parameter is required.");

                object data = null;
                string dataString = context.Request.Form["data"];

                if (!string.IsNullOrEmpty(dataString))
                    data = GetStringDeserialized(dataString);

                object responseData = ExecuteCommand(command, data);

                if (responseData != null)
                    context.Response.Write(GetSerializedString(responseData));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal static object ExecuteCommand(string command, object data)
        {
            HttpSessionState state = HttpContext.Current.Session;

            switch (command.ToLower().Trim())
            {
                case "savesession":
                    SaveSession(state, (UploadSession)data);

                    break;
                case "getsession":
                    return GetSession(state, (string)data);
                case "removesession":
                    RemoveSession(state, (string)data);

                    break;
                case "saverequest":
                    SaveRequest(state, (UploadRequest)data);

                    break;
                case "getrequest":
                    string[] keys = (string[])data;

                    return GetRequest(state, keys[0], keys[1]);
                //case "removerequest":
                //    keys = (string[])data;

                //    state.Remove(GetUploadRequestKey(keys[0], keys[1]));

                //    break;
                case "getrequestsforsession":
                    return GetRequestsForSession(state, (string)data);
                /*case "getstalesessions":
                    return GetStaleSessions(state, (DateTime)data);*/
                case "writetosessionstate":
                    object[] dataArray = (object[])data;

                    WriteToSessionState(state, (string)dataArray[0], dataArray[1]);

                    break;
                /*case "cleanupsessionifstale":
                    CleanUpSessionIfStale(state, (string)data);

                    break;*/
            }

            return null;
        }

        /*private static void CleanUpSessionIfStale(HttpSessionState state, string uploadSessionId)
        {
            UploadSession session = GetSession(state, uploadSessionId);

            if (session != null)
            {
                IEnumerable<UploadRequest> requests = GetRequestsForSession(state, uploadSessionId);

                DateTime lastUpdated = DateTime.MinValue;

                foreach (UploadRequest request in requests)
                {
                    if (request.LastUpdated > lastUpdated)
                        lastUpdated = request.LastUpdated;
                }

                if (lastUpdated.AddSeconds(SlickUploadContext.Config.SessionStorageProvider.StaleTimeout) < DateTime.Now)
                {
                    // TODO: optimize this?
                    SlickUploadContext.RollbackSession(session);
                }
            }
        }*/

        static Dictionary<string, string> GetSessionRequestsList(HttpSessionState state, string uploadSessionId, bool createIfMissing)
        {
            Dictionary<string, string> requests = state[GetUploadSessionRequestsKey(uploadSessionId)] as SerializableDictionary<string, string>;

            if (requests == null && createIfMissing)
            {
                requests = new SerializableDictionary<string, string>();

                state[GetUploadSessionRequestsKey(uploadSessionId)] = requests;
            }

            return requests;
        }
        
        static string GetUploadSessionKey(string uploadSessionId)
        {
            return "kw_UploadSession_" + uploadSessionId;
        }

        static string GetUploadSessionRequestsKey(string uploadSessionId)
        {
            return GetUploadSessionKey(uploadSessionId) + "_Requests";
        }

        static void SaveSession(HttpSessionState state, UploadSession session)
        {
            state[GetUploadSessionKey(session.UploadSessionId)] = GetSerializedString(session);
        }

        static UploadSession GetSession(HttpSessionState state, string uploadSessionId)
        {
            string data = state[GetUploadSessionKey(uploadSessionId)] as string;

            if (!string.IsNullOrEmpty(data))
                return GetStringDeserialized(data) as UploadSession;
            else
                return null;
        }

        static void RemoveSession(HttpSessionState state, string uploadSessionId)
        {
            state.Remove(GetUploadSessionKey(uploadSessionId));
        }

        static void SaveRequest(HttpSessionState state, UploadRequest request)
        {
            Dictionary<string, string> saveRequests = GetSessionRequestsList(state, request.UploadSessionId, true);

            saveRequests[request.UploadRequestId] = GetSerializedString(request);

        }

        static UploadRequest GetRequest(HttpSessionState state, string uploadSessionId, string uploadRequestId)
        {
            Dictionary<string, string> getRequests = GetSessionRequestsList(state, uploadSessionId, false);

            if (getRequests != null && getRequests.ContainsKey(uploadRequestId))
                return (UploadRequest)GetStringDeserialized(getRequests[uploadRequestId]);
            else
                return null;
        }

        static IEnumerable<UploadRequest> GetRequestsForSession(HttpSessionState state, string uploadSessionId)
        {
            Dictionary<string, string> getRequestsForSession = GetSessionRequestsList(state, uploadSessionId, false);

            if (getRequestsForSession != null)
            {
                List<UploadRequest> requests = new List<UploadRequest>();

                foreach (string request in getRequestsForSession.Values)
                    requests.Add((UploadRequest)GetStringDeserialized(request));

                return requests;
            }
            else
                return new UploadRequest[] { };
        }

        /*static IEnumerable<UploadSession> GetStaleSessions(HttpSessionState state, DateTime staleBefore)
        {
            List<UploadSession> uploadSessionList = new List<UploadSession>();

            foreach (string key in state.Keys)
            {
                if (key.StartsWith("kw_UploadSession_"))
                {
                    UploadSession session = (UploadSession)state[key];
                    DateTime lastUpdated = session.Started;

                    Dictionary<string, UploadRequest> requestsForStaleSession = GetSessionRequestsList(state, session.UploadSessionId, false);

                    if (requestsForStaleSession != null && requestsForStaleSession.Count > 0)
                    {
                        foreach (UploadRequest staleRequest in requestsForStaleSession.Values)
                        {
                            if (staleRequest.LastUpdated > lastUpdated)
                                lastUpdated = staleRequest.LastUpdated;
                        }
                    }

                    if (lastUpdated < staleBefore)
                        uploadSessionList.Add(session);
                }
            }

            return uploadSessionList;
        }*/

        static void WriteToSessionState(HttpSessionState state, string key, object value)
        {
            state[key] = value;
        }

        internal static string GetSerializedString(object data)
        {
            string dataString;

            ObjectStateFormatter formatter = new ObjectStateFormatter();

            if (data is UploadSession)
                dataString = "session-" + ((UploadSession)data).Serialize();
            else if (data is UploadRequest)
                dataString = "request-" + ((UploadRequest)data).Serialize();
            else if (data is IEnumerable<UploadSession>)
            {
                List<string> serializedStrings = new List<string>();

                foreach (UploadSession session in (IEnumerable<UploadSession>)data)
                    serializedStrings.Add(session.Serialize());

                dataString = "sessionlist-" + formatter.Serialize(serializedStrings.ToArray());
            }
            else if (data is IEnumerable<UploadRequest>)
            {
                List<string> serializedStrings = new List<string>();

                foreach (UploadRequest request in (IEnumerable<UploadRequest>)data)
                    serializedStrings.Add(request.Serialize());

                dataString = "requestlist-" + formatter.Serialize(serializedStrings.ToArray());
            }
            else
                dataString = formatter.Serialize(data);

            // TODO: encrypt
            return dataString;
        }

        internal static object GetStringDeserialized(string value)
        {
            value = value.Replace(' ', '+');

            // TODO: decrypt

            ObjectStateFormatter formatter = new ObjectStateFormatter();

            if (string.IsNullOrEmpty(value))
                return null;
            else if (value.StartsWith("session-"))
                return UploadSession.Deserialize(value.Substring("session-".Length));
            else if (value.StartsWith("request-"))
                return UploadRequest.Deserialize(value.Substring("request-".Length));
            else if (value.StartsWith("sessionlist-"))
            {
                string[] uploadSessionStrings = (string[])formatter.Deserialize(value.Substring("sessionlist-".Length));

                List<UploadSession> sessions = new List<UploadSession>();

                foreach (string sessionString in uploadSessionStrings)
                    sessions.Add(UploadSession.Deserialize(sessionString));

                return sessions;
            }
            else if (value.StartsWith("requestlist-"))
            {
                string[] uploadRequestStrings = (string[])formatter.Deserialize(value.Substring("requestlist-".Length));

                List<UploadRequest> requests = new List<UploadRequest>();

                foreach (string requestString in uploadRequestStrings)
                    requests.Add(UploadRequest.Deserialize(requestString));

                return requests;
            }
            else
                return formatter.Deserialize(value);
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
