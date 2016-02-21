using System;
using Microsoft.Win32;

namespace CSShellExtContextMenuHandler
{
    public class ShellExtReg
    {
        /// <summary>
        /// Register the context menu handler.
        /// </summary>
        /// <param name="clsid">The CLSID of the component.</param>
        /// <param name="fileType">
        /// The file type that the context menu handler is associated with. For 
        /// example, '*' means all file types; '.txt' means all .txt files. The 
        /// parameter must not be NULL or an empty string. 
        /// </param>
        /// <param name="friendlyName">The friendly name of the component.</param>
        public static void RegisterShellExtContextMenuHandler(Guid clsid,
            string fileType, string friendlyName)
        {
            if (clsid == Guid.Empty)
            {
                throw new ArgumentException("clsid must not be empty");
            }
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("fileType must not be null or empty");
            }

            // If fileType starts with '.', try to read the default value of the 
            // HKCR\<File Type> key which contains the ProgID to which the file type 
            // is linked.
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // If the key exists and its default value is not empty, use 
                        // the ProgID as the file type.
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            // 注册文件右键
            // Create the key HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}.
            string keyName = string.Format(@"{0}\shellex\ContextMenuHandlers\{1}",
                fileType, clsid.ToString("B"));
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(keyName))
            {
                // Set the default value of the key.
                if (key != null && !string.IsNullOrEmpty(friendlyName))
                {
                    key.SetValue(null, friendlyName);
                }
            }

        }

        

        /// <summary>
        /// Unregister the context menu handler.
        /// </summary>
        /// <param name="clsid">The CLSID of the component.</param>
        /// <param name="fileType">
        /// The file type that the context menu handler is associated with. For 
        /// example, '*' means all file types; '.txt' means all .txt files. The 
        /// parameter must not be NULL or an empty string. 
        /// </param>
        public static void UnregisterShellExtContextMenuHandler(Guid clsid,
            string fileType)
        {
            if (clsid == null)
            {
                throw new ArgumentException("clsid must not be null");
            }
            if (string.IsNullOrEmpty(fileType))
            {
                throw new ArgumentException("fileType must not be null or empty");
            }

            // If fileType starts with '.', try to read the default value of the 
            // HKCR\<File Type> key which contains the ProgID to which the file type 
            // is linked.
            if (fileType.StartsWith("."))
            {
                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileType))
                {
                    if (key != null)
                    {
                        // If the key exists and its default value is not empty, use 
                        // the ProgID as the file type.
                        string defaultVal = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(defaultVal))
                        {
                            fileType = defaultVal;
                        }
                    }
                }
            }

            // Remove the key HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}.
            string keyName = string.Format(@"{0}\shellex\ContextMenuHandlers\{1}",
                fileType, clsid.ToString("B"));
            Registry.ClassesRoot.DeleteSubKeyTree(keyName, false);

        }

        public static void RegisterAllFileShellContextMenu(Guid clsid, string friendlyName)
        {
            // 注册所有文件系统右键
            // Create the key HKCR\AllFilesystemObjects\shellex\ContextMenuHandlers.
            string fileSystemKey = string.Format(@"*\shellex\ContextMenuHandlers");
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileSystemKey, true))
            {
                // Set the default value of the key.
                if (key != null && !string.IsNullOrEmpty(friendlyName))
                {
                    var subkey = key.CreateSubKey(friendlyName);
                    subkey.SetValue(null, clsid.ToString("B"));
                }
            }
        }

        public static void UnRegisterAllFileShellContextMenu(string friendlyName)
        {
            string fileSystemKey = string.Format(@"*\shellex\ContextMenuHandlers\{0}",
                friendlyName);
            Registry.ClassesRoot.DeleteSubKeyTree(fileSystemKey, false);
          
        }
    }
}
