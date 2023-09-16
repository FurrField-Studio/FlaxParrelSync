using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using FlaxEngine;

namespace FlaxParrelSync
{
    public class ValidateCopiedFoldersIntegrity
    {
        public static void ValidateFolder(string targetRoot, string originalRoot, string folderName)
        {
            var targetFolderPath = Path.Combine(targetRoot, folderName);
            var targetFolderHash = CreateMd5ForFolder(targetFolderPath);

            var originalFolderPath = Path.Combine(originalRoot, folderName);
            var originalFolderHash = CreateMd5ForFolder(originalFolderPath);

            if (targetFolderHash != originalFolderHash)
            {
                Debug.Log("ParrelSync: Detected changes in '" + folderName + "' directory. Updating cloned project...");
                
                Directory.Delete(targetFolderPath, true);

                CopyFilesRecursively(originalFolderPath, targetFolderPath);
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        static string CreateMd5ForFolder(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            MD5 md5 = MD5.Create();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }

            return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        }
    }
}