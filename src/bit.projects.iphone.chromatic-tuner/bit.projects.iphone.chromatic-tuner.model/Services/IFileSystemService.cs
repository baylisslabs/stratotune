using System;

using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public interface IFileSystemService
    {
        int CreateFolders(string[] paths);
        bool InitialiseWithConfig(FileSystemConfigSection config);       
    }
    
}
