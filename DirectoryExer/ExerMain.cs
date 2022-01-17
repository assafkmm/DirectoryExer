using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryExer
{
    public  class ExerMain : ExerBase
    {
        internal static void DeleteDirectory(string inPath)
        {
            foreach (string file in GetFiles(inPath))
            {
                DeleteFile(file);
            }
            foreach (string dir in GetDirectories(inPath))
            {
                DeleteDirectory(dir);
            }
            DeleteEmptyDir(inPath);
        }
    }
}
