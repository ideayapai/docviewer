﻿/********************************** Module Header **********************************\
Module Name:  FileContextMenuExt.cs
Project:      CSShellExtContextMenuHandler
Copyright (c) Microsoft Corporation.

The FileContextMenuExt.cs file defines a context menu handler by implementing the 
IShellExtInit and IContextMenu interfaces.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using CSShellExtContextMenuHandler.Properties;

namespace CSShellExtContextMenuHandler
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B1F1405D-94A1-4692-B72F-FC8CAF8B8700"), ComVisible(true)]
    public class FileContextMenuExt : IShellExtInit, IContextMenu
    {
        // The name of the selected file.
        private string selectedFile;

        private string menuText = "&上传文件";
        private IntPtr menuBmp = IntPtr.Zero;
        //private string verb = "csdisplay";
        //private string verbCanonicalName = "CSDisplayFileName";
        //private string verbHelpText = "Display File Name (C#)";
        
        private static readonly string RegistryName = "DocViewer";

        public FileContextMenuExt()
        {
            // Load the bitmap for the menu item.
            Bitmap bmp = Resources.OK;
            bmp.MakeTransparent(bmp.GetPixel(0, 0));
            this.menuBmp = bmp.GetHbitmap();
        }

        ~FileContextMenuExt()
        {
            if (this.menuBmp != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this.menuBmp);
                this.menuBmp = IntPtr.Zero;
            }
        }


        void OnVerbDisplayFileName(IntPtr hWnd)
        {
 
        }


        /// <summary>
        /// 注册表添加右键(注册)
        /// </summary>
        /// <param name="t"></param>
        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {

                ShellExtReg.RegisterShellExtContextMenuHandler(t.GUID, ".txt", "CSShellExtContextMenuHandler.FileContextMenuExt Class");
                ShellExtReg.RegisterAllFileShellContextMenu(t.GUID, "CSShellExtContextMenuHandler.FileContextMenuExt Class");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error
                throw;  // Re-throw the exception
            }
        }

        /// <summary>
        /// 注册表删除右键(注销)
        /// </summary>
        /// <param name="t"></param>
        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                ShellExtReg.UnregisterShellExtContextMenuHandler(t.GUID, ".txt");

                ShellExtReg.UnRegisterAllFileShellContextMenu(RegistryName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the error
                throw;  // Re-throw the exception
            }
        }



       
        /// <summary>
        /// Initialize the context menu handler.
        /// </summary>
        /// <param name="pidlFolder">
        /// A pointer to an ITEMIDLIST structure that uniquely identifies a folder.
        /// </param>
        /// <param name="pDataObj">
        /// A pointer to an IDataObject interface object that can be used to retrieve 
        /// the objects being acted upon.
        /// </param>
        /// <param name="hKeyProgID">
        /// The registry key for the file object or folder type.
        /// </param>
        public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
        {
            if (pDataObj == IntPtr.Zero)
            {
                throw new ArgumentException();
            }

            FORMATETC fe = new FORMATETC();
            fe.cfFormat = (short)CLIPFORMAT.CF_HDROP;
            fe.ptd = IntPtr.Zero;
            fe.dwAspect = DVASPECT.DVASPECT_CONTENT;
            fe.lindex = -1;
            fe.tymed = TYMED.TYMED_HGLOBAL;
            STGMEDIUM stm = new STGMEDIUM();

            // The pDataObj pointer contains the objects being acted upon. In this 
            // example, we get an HDROP handle for enumerating the selected files 
            // and folders.
            IDataObject dataObject = (IDataObject)Marshal.GetObjectForIUnknown(pDataObj);
            dataObject.GetData(ref fe, out stm);

            try
            {
                // Get an HDROP handle.
                IntPtr hDrop = stm.unionmember;
                if (hDrop == IntPtr.Zero)
                {
                    throw new ArgumentException();
                }

                // Determine how many files are involved in this operation.
                uint nFiles = NativeMethods.DragQueryFile(hDrop, UInt32.MaxValue, null, 0);

                // This code sample displays the custom context menu item when only 
                // one file is selected. 
                if (nFiles == 1)
                {
                    // Get the path of the file.
                    StringBuilder fileName = new StringBuilder(260);
                    if (0 == NativeMethods.DragQueryFile(hDrop, 0, fileName,
                        fileName.Capacity))
                    {
                        Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                    this.selectedFile = fileName.ToString();
                }
                else
                {
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                }

                // [-or-]

                // Enumerate the selected files and folders.
                //if (nFiles > 0)
                //{
                //    StringCollection selectedFiles = new StringCollection();
                //    StringBuilder fileName = new StringBuilder(260);
                //    for (uint i = 0; i < nFiles; i++)
                //    {
                //        // Get the next file name.
                //        if (0 != NativeMethods.DragQueryFile(hDrop, i, fileName,
                //            fileName.Capacity))
                //        {
                //            // Add the file name to the list.
                //            selectedFiles.Add(fileName.ToString());
                //        }
                //    }
                //
                //    // If we did not find any files we can work with, throw 
                //    // exception.
                //    if (selectedFiles.Count == 0)
                //    {
                //        Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                //    }
                //}
                //else
                //{
                //    Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                //}
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref stm);
            }
        }



       
        /// <summary>
        /// Add commands to a shortcut menu.
        /// </summary>
        /// <param name="hMenu">A handle to the shortcut menu.</param>
        /// <param name="iMenu">
        /// The zero-based position at which to insert the first new menu item.
        /// </param>
        /// <param name="idCmdFirst">
        /// The minimum value that the handler can specify for a menu item ID.
        /// </param>
        /// <param name="idCmdLast">
        /// The maximum value that the handler can specify for a menu item ID.
        /// </param>
        /// <param name="uFlags">
        /// Optional flags that specify how the shortcut menu can be changed.
        /// </param>
        /// <returns>
        /// If successful, returns an HRESULT value that has its severity value set 
        /// to SEVERITY_SUCCESS and its code value set to the offset of the largest 
        /// command identifier that was assigned, plus one.
        /// </returns>
        public int QueryContextMenu(
            IntPtr hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags)
        {
            //MessageBox.Show("QueryContextMenu");
            // If uFlags include CMF_DEFAULTONLY then we should not do anything.
            if (((uint)CMF.CMF_DEFAULTONLY & uFlags) != 0)
            {
                return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, 0);
            }

            MenuManager.Instance.Register(new MenuContext{IdCmdFirst = idCmdFirst});
            List<MENUITEMINFO> menuInfos = MenuManager.Instance.GetMenuItemInfo();

            for (int index = 0; index < menuInfos.Count; index++)
            {
                var info = menuInfos[index];
                if (!NativeMethods.InsertMenuItem(hMenu, iMenu + (uint)index, true, ref info))
                {

                    return Marshal.GetHRForLastWin32Error();
                }
            }
            
            // Return an HRESULT value with the severity set to SEVERITY_SUCCESS. 
            // Set the code value to the offset of the largest command identifier 
            // that was assigned, plus one (1).
            return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, MenuManager.IDM_DISPLAY2 - MenuManager.IDM_DISPLAY + 1);
        }

        /// <summary>
        /// Carry out the command associated with a shortcut menu item.
        /// </summary>
        /// <param name="pici">
        /// A pointer to a CMINVOKECOMMANDINFO or CMINVOKECOMMANDINFOEX structure 
        /// containing information about the command. 
        /// </param>
        public void InvokeCommand(IntPtr pici)
        {
            bool isUnicode = false;

            // Determine which structure is being passed in, CMINVOKECOMMANDINFO or 
            // CMINVOKECOMMANDINFOEX based on the cbSize member of lpcmi. Although 
            // the lpcmi parameter is declared in Shlobj.h as a CMINVOKECOMMANDINFO 
            // structure, in practice it often points to a CMINVOKECOMMANDINFOEX 
            // structure. This struct is an extended version of CMINVOKECOMMANDINFO 
            // and has additional members that allow Unicode strings to be passed.
            CMINVOKECOMMANDINFO ici = (CMINVOKECOMMANDINFO)Marshal.PtrToStructure(
                pici, typeof(CMINVOKECOMMANDINFO));
            CMINVOKECOMMANDINFOEX iciex = new CMINVOKECOMMANDINFOEX();
            if (ici.cbSize == Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX)))
            {
                if ((ici.fMask & CMIC.CMIC_MASK_UNICODE) != 0)
                {
                    isUnicode = true;
                    iciex = (CMINVOKECOMMANDINFOEX)Marshal.PtrToStructure(pici,
                        typeof(CMINVOKECOMMANDINFOEX));
                }
            }

            // Determines whether the command is identified by its offset or verb.
            // There are two ways to identify commands:
            // 
            //   1) The command's verb string 
            //   2) The command's identifier offset
            // 
            // If the high-order word of lpcmi->lpVerb (for the ANSI case) or 
            // lpcmi->lpVerbW (for the Unicode case) is nonzero, lpVerb or lpVerbW 
            // holds a verb string. If the high-order word is zero, the command 
            // offset is in the low-order word of lpcmi->lpVerb.

            // For the ANSI case, if the high-order word is not zero, the command's 
            // verb string is in lpcmi->lpVerb. 
            List<Menu> menus = MenuManager.Instance.Menus;
            
            foreach (var menu in menus)
            {
                if (menu.MenuType != MenuType.Normal)
                {
                    continue;
                }

                if (!isUnicode && NativeMethods.HighWord(ici.verb.ToInt32()) != 0)
                {
                    // Is the verb supported by this context menu extension?
                    if (Marshal.PtrToStringAnsi(ici.verb) == menu.Verb)
                    {
                        OnVerbDisplayFileName(ici.hwnd);
                    }
                    else
                    {
                        // If the verb is not recognized by the context menu handler, it 
                        // must return E_FAIL to allow it to be passed on to the other 
                        // context menu handlers that might implement that verb.
                        //Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                }

            // For the Unicode case, if the high-order word is not zero, the 
                // command's verb string is in lpcmi->lpVerbW. 
                else if (isUnicode && NativeMethods.HighWord(iciex.verbW.ToInt32()) != 0)
                {
                    // Is the verb supported by this context menu extension?
                    if (Marshal.PtrToStringUni(iciex.verbW) == menu.Verb)
                    {
                        OnVerbDisplayFileName(ici.hwnd);
                    }
                    else
                    {
                        // If the verb is not recognized by the context menu handler, it 
                        // must return E_FAIL to allow it to be passed on to the other 
                        // context menu handlers that might implement that verb.
                        //Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                }

                // If the command cannot be identified through the verb string, then 
                // check the identifier offset.
                else
                {
                
                    // Is the command identifier offset supported by this context menu 
                    // extension?
                    if (NativeMethods.LowWord(ici.verb.ToInt32()) == menu.Id)
                    {
                        OnVerbDisplayFileName(ici.hwnd);
                    }
                    else
                    {
                        // If the verb is not recognized by the context menu handler, it 
                        // must return E_FAIL to allow it to be passed on to the other 
                        // context menu handlers that might implement that verb.
                        // Marshal.ThrowExceptionForHR(WinError.E_FAIL);
                    }
                }
            }
            
        }

        /// <summary>
        /// Get information about a shortcut menu command, including the help string 
        /// and the language-independent, or canonical, name for the command.
        /// </summary>
        /// <param name="idCmd">Menu command identifier offset.</param>
        /// <param name="uFlags">
        /// Flags specifying the information to return. This parameter can have one 
        /// of the following values: GCS_HELPTEXTA, GCS_HELPTEXTW, GCS_VALIDATEA, 
        /// GCS_VALIDATEW, GCS_VERBA, GCS_VERBW.
        /// </param>
        /// <param name="pReserved">Reserved. Must be IntPtr.Zero</param>
        /// <param name="pszName">
        /// The address of the buffer to receive the null-terminated string being 
        /// retrieved.
        /// </param>
        /// <param name="cchMax">
        /// Size of the buffer, in characters, to receive the null-terminated string.
        /// </param>
        public void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax)
        {

            var menu = MenuManager.Instance.Menus.Find(f => f.Id == idCmd.ToUInt32());
            if (menu != null)
            {
      
                switch ((GCS)uFlags)
                {
                    case GCS.GCS_VERBW:
                        if (menu.VerbCanonicalName.Length > cchMax - 1)
                        {
                            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
                        }
                        else
                        {
                            pszName.Clear();
                            pszName.Append(menu.VerbCanonicalName);
                        }
                        break;

                    case GCS.GCS_HELPTEXTW:
                        if (menu.VerbHelpText.Length > cchMax - 1)
                        {
                            Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER);
                        }
                        else
                        {
                            pszName.Clear();
                            pszName.Append(menu.VerbHelpText);
                        }
                        break;
                }
            }
        }

    }
}