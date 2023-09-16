using System.Collections.Generic;
using System.Linq;
using FlaxEngine;

namespace FlaxParrelSync
{
    public class Project : System.ICloneable
    {
        public string name;
        public string projectPath;
        string rootPath;

        public string projectFilename => name + ".flaxproj";

        //linkable
        public string contentPath;
        public string sourcePath;
        
        //copy
        public string binariesPath;
        public string cachePath;

        char[] separator = new char[2] { '/', '\\' };

        /// <summary>
        /// Default constructor
        /// </summary>
        public Project()
        {

        }


        /// <summary>
        /// Initialize the project object by parsing its full path returned by Unity into a bunch of individual folder names and paths.
        /// </summary>
        /// <param name="path"></param>
        public Project(string path)
        {
            ParsePath(path);
        }


        /// <summary>
        /// Create a new object with the same settings
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Project newProject = new Project();
            newProject.name = name;
            newProject.projectPath = projectPath;
            newProject.rootPath = rootPath;
            
            newProject.contentPath = contentPath;
            newProject.sourcePath = sourcePath;

            newProject.separator = separator;


            return newProject;
        }

        /// <summary>
        /// Debug override so we can quickly print out the project info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string printString = name + "\n" +
                                 rootPath + "\n" +
                                 projectPath + "\n" +
                                 contentPath + "\n" +
                                 sourcePath;
            return (printString);
        }

        private void ParsePath(string path)
        {
            //Unity's Application functions return the Assets path in the Editor. 
            projectPath = path;

            //pop off the last part of the path for the project name, keep the rest for the root path
            List<string> pathArray = projectPath.Split(separator).ToList<string>();
            name = pathArray.Last();

            pathArray.RemoveAt(pathArray.Count() - 1);
            rootPath = string.Join(separator[0].ToString(), pathArray.ToArray());

            contentPath = projectPath + "/Content";
            sourcePath = projectPath + "/Source";

            binariesPath = projectPath + "/Binaries";
            cachePath = projectPath + "/Cache";
        }
    }
}