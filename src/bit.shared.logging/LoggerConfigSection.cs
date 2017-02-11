using System;
using bit.shared.appconfig;

namespace bit.shared.logging
{
    public class LoggerConfigSection : ConfigSection
    {
        public override string Name { get { return "bit.shared.logging.LoggerConfigSection"; } }

        public bool Trace { get; private set; }
        public bool Debug { get; private set; }
        public bool Info { get; private set; }
        public bool Warn { get; private set; }
        public bool Error { get; private set; }
        public bool Fatal { get; private set; }

        public LoggerConfigSection()
        {
        }

        public LoggerConfigSection(
            bool trace
            ,bool debug
            ,bool info
            ,bool warn
            ,bool error
            ,bool fatal)
        {
            this.Trace = trace;
            this.Debug = debug;
            this.Info = info;
            this.Warn = warn;
            this.Error = error;
            this.Fatal = fatal;
        }

    }
}

