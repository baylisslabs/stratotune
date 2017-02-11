using System;
using System.IO;

using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{   
    public class FileSystemServiceImpl : IFileSystemService
    {
        static Logger _log = LogManager.GetLogger("FileSystemServiceImpl");
              
        public FileSystemServiceImpl ()
        {
        }

        public int CreateFolders (string[] paths)
        {
            int i = 0;
            foreach (var path in paths) {
                try
                {
                    _log.Debug("Directory.CreateDirectory({0})",path);
                    Directory.CreateDirectory(path);
                    ++i;
                }
                catch(Exception ex) {
                    _log.Error(String.Format("Directory.CreateDirectory({0}) failed",path),ex);
                }
            }
            return i;
        }

        public bool InitialiseWithConfig(FileSystemConfigSection config)
        {
            var paths = config.FoldersToCreate.Split(';');
            int count = this.CreateFolders(paths);
            return count==paths.Length;
        }
    }
}

