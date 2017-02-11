using System;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner
{
    /// <summary>
    /// Qeuries the execution environment and sets environment variables, and also will do the app config flags
    /// </summary>
    public class EnvConfig : EnvConfigBase
    {       
        public class Keys
        {        
            public const string APP_SUPPORT_FOLDER = "APP_SUPPORT_FOLDER";           
        }      
              
        public override void Initialise()
        {           
            this.Set(Keys.APP_SUPPORT_FOLDER,getFolderPath(NSSearchPathDirectory.ApplicationSupportDirectory,NSSearchPathDomain.User));
        }
               
        private string getFolderPath (NSSearchPathDirectory directory, NSSearchPathDomain domain)
        {
            var fm = new NSFileManager ();
            NSError err;
            var url = fm.GetUrl (directory, domain, null, false, out err);
            if (err == null) {
                return url.Path;
            }
            return null;
        }
    }    
}

