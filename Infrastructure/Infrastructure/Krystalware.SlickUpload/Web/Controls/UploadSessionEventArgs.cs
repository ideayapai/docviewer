using System;
using System.Text;
using System.Collections.ObjectModel;
using Krystalware.SlickUpload.Web.Controls;
using System.Collections.Generic;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Provides data for the UploadComplete event.
    /// </summary>
    public sealed class UploadSessionEventArgs : EventArgs
    {
        UploadSession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadSessionEventArgs" /> class. 
        /// </summary>
        /// <param name="session">The <see cref="UploadSession" /> for this event.</param>
        internal UploadSessionEventArgs(UploadSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the current <see cref="UploadSession" />.
        /// </summary>
        public UploadSession UploadSession
        {
            get
            {
                return _session;
            }
        }

        /// <summary>
        /// Gets the current collection of <see cref="UploadedFile" />s.
        /// </summary>
        public ICollection<UploadedFile> UploadedFiles
        {
            get
            {
                return _session.UploadedFiles;
            }
        }
    }
}
