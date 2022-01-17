using System;
using System.IO;
using System.Reflection;

namespace DirectoryExer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            string s = GetJSON();
            RunTest(s, "projects/junk/apitest");


            //string s = MockFileSystem.CreateFromFileSystem(@"C:\projects", 5).ToJSON();
            //System.Diagnostics.Trace.WriteLine(s);
        }

        private static void RunTest(string inJSON, string inPath)
        {
            ExerBase.FileSystem = MockFileSystem.CreateFromJSON(inJSON);
            int allDCount = ExerBase.FileSystem.Root.GetDirCount();
            int allFCount = ExerBase.FileSystem.Root.GetFileCount();
            MockDir dir = ExerBase.FileSystem.GetDir(inPath);
            int beforeDDir = dir.GetDirCount();
            int beforeFDir = dir.GetFileCount();
            ExerMain.DeleteDirectory(inPath);
            int afterAllDCount = ExerBase.FileSystem.Root.GetDirCount();
            int afterAllFCount = ExerBase.FileSystem.Root.GetFileCount();
            if ((afterAllDCount + beforeDDir) != allDCount || (afterAllFCount + beforeFDir) != allFCount)
            {
                throw new Exception("Wrong file count");
            }


        }

        static string GetJSON()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DirectoryExer.dir1.json";
            string result;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }



    }
}
