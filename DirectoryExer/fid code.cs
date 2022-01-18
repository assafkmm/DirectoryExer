//https://dotnetfiddle.net/LaDweN

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class ExerMain : ExerBase
{
    internal static void DeleteDirectory(string inPath)
    {
        // use the following functions (defined in base class) to delete the folder
        //string[] GetFiles(string inPath)
        //string[] GetDirectories(string inPath)
        //void DeleteFile(string inPath)
        //void DeleteEmptyDir(string inPath)



    }
}




//DO NOT LOOK DOWN







































public class ExerBase
{
    private static MockFileSystem sFileSystem;

    internal static MockFileSystem FileSystem { get => sFileSystem; set => sFileSystem = value; }

    protected static string[] GetFiles(string inPath)
    {
        MockDir dir = sFileSystem.GetDir(inPath);

        return dir.Files.Select(fil => string.Concat(inPath, "/", fil.FileName)).ToArray();
    }


    protected static string[] GetDirectories(string inPath)
    {
        MockDir dir = sFileSystem.GetDir(inPath);

        return dir.Dirs.Select(dirs => string.Concat(inPath, "/", dirs.DirName)).ToArray();
    }

    protected static void DeleteFile(string inPath)
    {
        MockFile file = sFileSystem.GetFile(inPath);

        file.Delete();
    }

    protected static void DeleteEmptyDir(string inPath)
    {
        MockDir dir = sFileSystem.GetDir(inPath);

        dir.Delete();
    }
}

public class MockFileSystem
{
    public static MockFileSystem CreateFromJSON(string inString)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<MockFileSystem>(inString);
    }

    public static MockFileSystem CreateFromFileSystem(string inString, int? inDepth)
    {
        DirectoryInfo dir = new DirectoryInfo(inString);
        MockFileSystem retVal = new MockFileSystem();
        retVal.Root = MockDir.CreateFromFileSystem(dir, inDepth);

        return retVal;
    }

    public string ToJSON()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

    public MockFileSystem()
    {
    }

    public MockFile GetFile(string inPath)
    {
        string[] dirs = inPath.Split("/");
        MockDir curr = Root;
        for (int i = 1; i < dirs.Length - 1; ++i)
        {
            curr = curr.GetDir(dirs[i]);
        }

        return curr.GetFile(dirs[dirs.Length - 1]);
    }

    public MockDir GetDir(string inPath)
    {
        string[] dirs = inPath.Split("/");
        MockDir curr = Root;
        for (int i = 1; i < dirs.Length; ++i)
        {
            curr = curr.GetDir(dirs[i]);
        }

        return curr;
    }
    public MockDir Root { get; set; }
}

public class MockFile
{
    public string FileName { get; set; }

    internal static MockFile CreateFromFileSystem(FileInfo fi)
    {
        MockFile retVal = new MockFile();

        retVal.FileName = fi.Name;

        return retVal;
    }

    internal void Delete()
    {
        Dir.DeleteFile(this);
    }

    public MockDir Dir { get; set; }

}

public class MockDir
{
    public string DirName { get; set; }
    public List<MockFile> Files
    {
        get;
        set;
    }

    public List<MockDir> Dirs
    {
        get;
        set;
    }
    public MockDir Parent { get; private set; }

    public int GetFileCount()
    {
        return Files.Count + Dirs.Aggregate(0, (total, dir) => total + dir.GetFileCount());
    }

    public int GetDirCount()
    {
        return 1 + Dirs.Aggregate(0, (total, dir) => total + dir.GetDirCount());
    }

    internal static MockDir CreateFromFileSystem(DirectoryInfo dir, int? inDepth)
    {
        MockDir retVal = new MockDir();
        retVal.DirName = dir.Name;
        retVal.Files = new List<MockFile>();
        retVal.Dirs = new List<MockDir>();
        if (!inDepth.HasValue || inDepth.Value != 0)
        {
            try
            {
                int? newDepth = inDepth.HasValue ? inDepth.Value - 1 : null;

                foreach (FileInfo fi in dir.EnumerateFiles())
                {
                    retVal.Files.Add(MockFile.CreateFromFileSystem(fi));
                }
                foreach (DirectoryInfo di in dir.EnumerateDirectories())
                {
                    retVal.Dirs.Add(MockDir.CreateFromFileSystem(di, newDepth));
                }
            }
            catch
            {
                // ignore
            }
        }

        return retVal;
    }

    internal MockDir GetDir(string v)
    {
        MockDir dir = Dirs.FirstOrDefault(dir => string.Compare(v, dir.DirName, true) == 0);

        if (dir != null)
        {
            dir.Parent = this;
        }

        return dir;
    }

    internal MockFile GetFile(string v)
    {
        MockFile file = Files.FirstOrDefault(fil => string.Compare(v, fil.FileName, true) == 0);

        file.Dir = this;

        return file;
    }

    internal void DeleteFile(MockFile mockFile)
    {
        Files.Remove(mockFile);
    }

    internal void Delete()
    {
        if (Files.Count != 0 && Dirs.Count != 0)
        {
            throw new InvalidOperationException("Cannot delete not empty directory");
        }
        Parent?.DeleteDir(this);
    }

    private void DeleteDir(MockDir mockDir)
    {
        Dirs.Remove(mockDir);
    }
}

public class Program
{
    public static void Main()
    {
        string JSON = MockFileSystem.CreateFromFileSystem("/dev", 5).ToJSON();
        //Console.WriteLine(JSON);
        RunTest(JSON, "dev/fd");

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
            throw new Exception("Wrong file/dir count");
        }
        Console.WriteLine("ALL GOOD!!!");


    }
}