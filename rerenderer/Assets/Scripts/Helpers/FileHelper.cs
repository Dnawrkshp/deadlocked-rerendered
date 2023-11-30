using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHelper
{

    public static void CopyAll(string SourcePath, string DestinationPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(Path.Combine(DestinationPath, dirPath.Remove(0, SourcePath.Length)));

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
            if (!newPath.EndsWith(".meta"))
                File.Copy(newPath, Path.Combine(DestinationPath, newPath.Remove(0, SourcePath.Length)), true);
    }

}
