using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace fbc_webapi.Classes
{
    public class CustomFileStream : FileStream
    {
        public string FileToDelete { get; set; }

        public CustomFileStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public CustomFileStream(SafeFileHandle handle, FileAccess access) : base(handle, access)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
        {
        }

        public CustomFileStream(SafeFileHandle handle, FileAccess access, int bufferSize) : base(handle, access, bufferSize)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
        }

        public CustomFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) : base(handle, access, bufferSize, isAsync)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, bool deleteFileOnClose) : base(path, mode, access, share, bufferSize, options)
        {
            if (deleteFileOnClose)
                FileToDelete = path;
        }

        public CustomFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options) : base(path, mode, rights, share, bufferSize, options)
        {
        }

        public CustomFileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity) : base(path, mode, rights, share, bufferSize, options, fileSecurity)
        {
        }

        public override void Close()
        {
            base.Close();

            if (!string.IsNullOrEmpty(FileToDelete))
                Utils.TentarApagarFicheiroTemporario(FileToDelete);
        }
    }
}