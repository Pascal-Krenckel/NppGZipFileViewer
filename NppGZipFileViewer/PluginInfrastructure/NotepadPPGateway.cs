// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using NppGZipFileViewer;
using NppPluginNET.PluginInfrastructure;
using System;
using System.Linq;
using System.Text;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    public interface INotepadPPGateway
    {
        void FileNew();

        string GetCurrentFilePath();
        unsafe string GetFilePath(int bufferId);
        void SetCurrentLanguage(LangType language);

        bool SetStatusBar(NppMsg statusBarType, string status);
    }

    /// <summary>
    /// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
    /// incomplete. Please help fill in the blanks.
    /// </summary>
    public class NotepadPPGateway : INotepadPPGateway
    {
        private const int Unused = 0;
        private IntPtr NppHandle => PluginBase.nppData._nppHandle;

        public void FileNew()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, Unused, NppMenuCmd.IDM_FILE_NEW);
        }

        /// <summary>
        /// Gets the path of the current document.
        /// </summary>
        public string GetCurrentFilePath()
        {
            var path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);
            return path.ToString();
        }

        /// <summary>
        /// Gets the path of the current document.
        /// </summary>
        public unsafe string GetFilePath(int bufferId)
        {
            var path = new StringBuilder(2000);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
            return path.ToString();
        }

        public void SetCurrentLanguage(LangType language)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETCURRENTLANGTYPE, Unused, (int)language);
        }

        public bool SetStatusBar(NppMsg statusBarType, string str)
        {
            return Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETSTATUSBAR, (int)statusBarType, str) == IntPtr.Zero;
        }

        public Int64 GetBufferEncoding(IntPtr bufferId)
        {
            return (Win32.SendMessage(NppHandle, NppMsg.NPPM_GETBUFFERENCODING, bufferId, 0).ToInt64());
        }       

        public void SetMenuItemCheck(int cmdIndex, bool @checked)
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items[cmdIndex]._cmdID, @checked ? 1 : 0);
        }

        public void SendMenuEncoding(NppEncoding enc)
        {
            switch(enc)
            {
                case NppEncoding.ANSI:
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_MENUCOMMAND, 0, (int)NppMenuCmd.IDM_FORMAT_ANSI); break;
                case NppEncoding.UTF8_BOM:
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_MENUCOMMAND, 0, (int)NppMenuCmd.IDM_FORMAT_UTF_8); break;
                case NppEncoding.UTF16_LE:
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_MENUCOMMAND, 0, (int)NppMenuCmd.IDM_FORMAT_UCS_2LE); break;
                case NppEncoding.UTF16_BE:
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_MENUCOMMAND, 0, (int)NppMenuCmd.IDM_FORMAT_UCS_2BE); break;
                case NppEncoding.UTF8:
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_MENUCOMMAND, 0, (int)NppMenuCmd.IDM_FORMAT_AS_UTF_8); break;
            }
        }

        public void SetMenuItemCheck(string commandName, bool @checked)
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SETMENUITEMCHECK, PluginBase._funcItems.Items.First(cmd => cmd._itemName == commandName)._cmdID, @checked ? 1 : 0);
        }
        public void SetMenuItemCheck(FuncItem cmd, bool @checked)
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SETMENUITEMCHECK, cmd._cmdID, @checked ? 1 : 0);
        }


        public StringBuilder GetFullPathFromBufferId(IntPtr id)
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(NppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, id, path);
            return path;
        }

        public IntPtr GetCurrentBufferId()
        {
            return Win32.SendMessage(NppHandle, NppMsg.NPPM_GETCURRENTBUFFERID, 0, 0);
        }

        internal void MakeCurrentBufferDirty()
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_MAKECURRENTBUFFERDIRTY, 0, 0);
        }

        internal void SwitchToFile(StringBuilder path)
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SWITCHTOFILE, 0, path);
        }

        internal void SaveCurrentFile()
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SAVECURRENTFILE, 0, 0);
        }

        internal void SwitchToFile(string path)
        {
            Win32.SendMessage(NppHandle, NppMsg.NPPM_SWITCHTOFILE, 0, path);
        }
    }

    /// <summary>
    /// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
    /// incomplete. Please help fill in the blanks.
    /// </summary>
    class NppResource
    {
        private const int Unused = 0;

        public void ClearIndicator()
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)Resource.NPPM_INTERNAL_CLEARINDICATOR, Unused, Unused);
        }
    }
}
