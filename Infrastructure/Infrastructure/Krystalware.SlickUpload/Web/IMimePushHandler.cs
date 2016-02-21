using System;
using System.Collections.Specialized;

namespace Krystalware.SlickUpload.Web
{
	/// <summary>
	/// Summary description for IMimePushHandler.
	/// </summary>
	internal interface IMimePushHandler
	{
		void BeginPart(NameValueCollection headers);
        void PartData(ref byte[] data, int start, int length);
		void EndPart(bool isLast, bool isComplete);
	}
}