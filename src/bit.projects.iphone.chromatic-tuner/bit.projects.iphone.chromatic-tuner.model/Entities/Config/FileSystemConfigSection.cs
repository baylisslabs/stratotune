using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
    public class FileSystemConfigSection : ConfigSection
    {
        public override string Name { get { return "bit.projects.iphone.chromatictuner.FileSystemConfigSection"; } }

        // ';' delimited list of folders to create if not existing
        public string FoldersToCreate { get; set; }
    }
}

