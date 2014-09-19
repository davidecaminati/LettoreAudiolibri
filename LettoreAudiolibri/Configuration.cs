using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace WindowsFormsApplication1
{
    class Configuration
    {
        public ISynchronizeInvoke EventSyncInvoke { get; set; }
        public event EventHandler ReadConfigurationError;
        // Field 
        public string ComPort;
        public string BookPath;
        public string path_fileconfig;
        public string working_Dir; 

        //Costructor
        public Configuration()
        {
        }


        public bool LoadConfiguration(string file_config)
        {
            if (ReadConfigFile(file_config))
            {
                this.path_fileconfig = file_config;
                this.working_Dir = Path.GetDirectoryName(file_config);
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool ReadConfigFile(string config_file)
        {
            try
            {
                string line;
                // Read the configuration file line by line.
                System.IO.StreamReader file = new System.IO.StreamReader(config_file);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("@"))
                    {
                        this.BookPath = line.Split('@')[1];
                    }
                    if (line.StartsWith("|"))
                    {
                        this.ComPort = line.Split('|')[1];
                    }
                }
                file.Close();
                return true;
            }
            catch
            {
                RaiseReadConfigurationError(EventArgs.Empty);
                return false;
            }
        }


        private void RaiseReadConfigurationError(EventArgs e)
        {
            EventHandler readConfigurationError = this.ReadConfigurationError;

            // Check for no subscribers
            if (readConfigurationError == null)
                return;

            if (EventSyncInvoke == null)
                readConfigurationError(this, e);
            else
                EventSyncInvoke.Invoke(readConfigurationError, new object[] { this, e });
        }
    }
}
