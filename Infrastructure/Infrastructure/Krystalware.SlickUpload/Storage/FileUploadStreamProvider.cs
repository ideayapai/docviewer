using System;
using System.IO;
using System.Reflection;
using System.Web;

using Krystalware.SlickUpload.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Hosting;
using System.Text;

namespace Krystalware.SlickUpload.Storage
{
	/// <summary>
	/// An <see cref="IUploadStreamProvider" /> that writes to files.
	/// </summary>
	public class FileUploadStreamProvider : UploadStreamProviderBase
	{
		/// <summary>
		/// Enumeration of the possible actions to take when trying to upload to a file that exists.
		/// </summary>
		public enum ExistingAction
		{
			/// <summary>
			/// Overwrite the file.
			/// </summary>
			Overwrite,
			/// <summary>
			/// Throw an exception, causing an error and aborting the upload.
			/// </summary>
			Exception,
			/// <summary>
			/// Change the name of the new location by adding a number.
			/// </summary>
			Rename
		}

		/// <summary>
		/// Enumeration of the methods to use when generating a filename.
		/// </summary>
		public enum FileNameMethod
		{
			/// <summary>
			/// Use the name the file was named on the client.
			/// </summary>
			Client,
			/// <summary>
            /// Generate a unique <see cref="Guid" />.
			/// </summary>
			Guid,
            /// <summary>
            /// Generate a unique <see cref="Guid" /> with the client filename's extension appended.
            /// </summary>
            GuidWithExtension
		}

		string _location;
        string _erroredLocation;
		ExistingAction _existingAction;
		FileNameMethod _fileNameMethod;

        /// <summary>
        /// Creates a new instance of the <see cref="FileUploadStreamProvider" /> class with the specified configuration settings.
        /// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        public FileUploadStreamProvider(UploadStreamProviderElement settings)
            : base(settings)
		{
			_location = Settings.Parameters["location"];

			if (_location == null)
			{
				// TODO: fix up
				throw new Exception("location must be specified for SlickUpload file provider");
			}
			else if (!Path.IsPathRooted(_location))
			{
				_location = HostingEnvironment.MapPath(_location);
			}

			string existingActionString = Settings.Parameters["existingAction"];

			if (!string.IsNullOrEmpty(existingActionString))
				_existingAction = (ExistingAction)Enum.Parse(typeof(ExistingAction), existingActionString, true);
			else
				_existingAction = ExistingAction.Exception;

            string fileNameMethodString = Settings.Parameters["fileNameMethod"];

            if (!string.IsNullOrEmpty(fileNameMethodString))
                _fileNameMethod = (FileNameMethod)Enum.Parse(typeof(FileNameMethod), fileNameMethodString, true);
            else
                _fileNameMethod = FileNameMethod.Client;

            _erroredLocation = Settings.Parameters["erroredLocation"];

            if (!string.IsNullOrEmpty(_erroredLocation) && !Path.IsPathRooted(_erroredLocation))
                _location = HostingEnvironment.MapPath(_erroredLocation);
		}

        /// <summary>
        /// Returns the server file name to use for a given <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to generate a file name.</param>
        /// <returns>The generated file name.</returns>
        public virtual string GetServerFileName(UploadedFile file)
        {
            string fileName;

            switch (_fileNameMethod)
            {
                default:
                case FileNameMethod.Client:
                    fileName = GetValidFileName(file.ClientName);

                    break;
                case FileNameMethod.Guid:
                    fileName = Guid.NewGuid().ToString("n");

                    break;
                case FileNameMethod.GuidWithExtension:
                    return Guid.NewGuid().ToString("n") + Path.GetExtension(GetValidFileName(file.ClientName));

            }

            return fileName;
        }

        /// <summary>
        /// Returns a valid file name, with any invalid characters replaced to "-".
        /// </summary>
        /// <param name="fileName">The input file name with potentially invalid characters.</param>
        /// <returns>The valid file name.</returns>
        public virtual string GetValidFileName(string fileName)
        {
            StringBuilder validFileName = new StringBuilder(fileName);

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                validFileName.Replace(invalidChar, '-');

            return validFileName.ToString();
        }

        /// <summary>
        /// Returns the absolute path for a relative file name.
        /// </summary>
        /// <param name="fileName">The relative file name.</param>
        /// <returns>The absolute path.</returns>
        public virtual string ConvertToAbsolutePath(string fileName)
        {
            // If the path is app relative, map it
            if (fileName.StartsWith("~"))
                fileName = HostingEnvironment.MapPath(fileName);

            // If the path isn't absolute, relate it to the location
            if (!Path.IsPathRooted(fileName))
                fileName = Path.Combine(_location, fileName);

            return fileName;
        }

        /// <summary>
        /// Returns an enumerable sequence that represents the renaming sequence for the specified filename.
        /// This sequence will be iterated until it generates a filename that doesn't currently exist to find a valid filename.
        /// </summary>
        /// <param name="fileName">The filename for which to create the rename sequence.</param>
        /// <returns>An enumerable sequence that represents the renaming sequence for the specified.</returns>
        public virtual IEnumerable<string> GetRenameSequence(string fileName)
        {
            yield return fileName;

            string fileNameSection = Path.GetFileName(fileName);
            int extensionStart = fileNameSection.IndexOf('.');
            string path;
            string extension;

            if (extensionStart != -1)
            {
                path = Path.Combine(Path.GetDirectoryName(fileName), fileNameSection.Substring(0, extensionStart));
                extension = fileNameSection.Substring(extensionStart);
            }
            else
            {
                path = fileName;
                extension = string.Empty;
            }

            int i = 0;

            while (true)
            {
                i++;

                fileName = path + "[" + i.ToString() + "]" + extension;

                yield return fileName;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetWriteStream(UploadedFile file)
        {
            string fileName = GetServerFileName(file);

            fileName = ConvertToAbsolutePath(fileName);

            // Attempt to create the directory if it doesn't exist
            string rootPath = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            file.ServerLocation = fileName;

            FileStream s = null;

            try
            {
                switch (_existingAction)
                {
                    case ExistingAction.Exception:
                        // Get the stream and let create throw if it needs to
                        s = new FileStream(fileName, FileMode.CreateNew);//, FileAccess.Write, FileShare.None, 1024 * 4);

                        break;
                    case ExistingAction.Overwrite:
                        // Overwrite if it exists
                        s = new FileStream(fileName, FileMode.Create);//, FileAccess.Write, FileShare.None, 1024 * 4);

                        break;
                    case ExistingAction.Rename:
                        foreach (string renameFileName in GetRenameSequence(fileName))
                        {
                            if (!File.Exists(renameFileName))
                            {
                                try
                                {
                                    s = new FileStream(renameFileName, FileMode.CreateNew);//, FileAccess.Write, FileShare.None, 1024 * 4);

                                    file.ServerLocation = renameFileName;

                                    break;
                                }
                                catch (IOException)
                                {
                                    // Eat... we'll try the next in the sequence
                                }
                            }
                        }

                        break;
                }

                // s.SetLength(SlickUploadContext.CurrentUploadRequest.ContentLength);

                return s;
            }
            catch
            {
                if (s != null)
                {
                    CloseWriteStream(file, s, false);

                    RemoveOutput(file);
                }

                throw;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void RemoveOutput(UploadedFile file)
        {
            // If it's a valid upload
            if (!string.IsNullOrEmpty(file.ServerLocation) && File.Exists(file.ServerLocation))
            {
                if (string.IsNullOrEmpty(_erroredLocation))
                    File.Delete(file.ServerLocation);
                else
                    File.Move(file.ServerLocation, Path.Combine(_erroredLocation, file.UploadRequest.UploadRequestId + "-" + Path.GetFileName(file.ServerLocation)));
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetReadStream(UploadedFile file)
        {
            return File.OpenRead(file.ServerLocation);
        }

        /*
        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete)
        {
            //if (isComplete)
            //    stream.SetLength(SlickUploadContext.CurrentUploadRequest.UploadedFiles[0].ContentLength);

            base.CloseWriteStream(file, stream, isComplete);
        }*/
    }
}