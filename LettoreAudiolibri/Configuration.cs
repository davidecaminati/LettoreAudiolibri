using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    class Configuration
    {
        // Field 
        public string ComPort;
        public string BookPath;

        public Configuration(string comPort,string bookPath)
        {
            ComPort = comPort;
            BookPath = bookPath;
        }
    }
}
