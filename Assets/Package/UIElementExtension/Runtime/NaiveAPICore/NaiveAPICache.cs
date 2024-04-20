using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NaiveAPI.UITK
{
    public static class NaiveAPICache
    {
        public const string RootName = "NaiveAPICache";
        public static void MakeDirValid(string path)
        {
            path = Path.GetDirectoryName(path);
            var names = path.Split('/', '\\');

            var currCachePath = "";
            foreach (var name in names)
            {
                currCachePath = Path.Join(currCachePath, name);
                if (!Directory.Exists(currCachePath))
                    Directory.CreateDirectory(currCachePath);
            }
        }
        public static void Save(string path, string data)
        {
            path = Path.Join(Application.temporaryCachePath, RootName, path);
            MakeDirValid(path);
            File.WriteAllText(path, data);
        }
        public static string Load(string path)
        {
            path = Path.Join(Application.temporaryCachePath, RootName, path);
            if(File.Exists(path))
                return File.ReadAllText(path);
            else return "";
        }
    }
}
