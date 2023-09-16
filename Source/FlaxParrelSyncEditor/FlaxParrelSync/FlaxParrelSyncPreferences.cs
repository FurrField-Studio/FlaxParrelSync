using System.Collections.Generic;
using FlaxEngine;

namespace FlaxParrelSync
{
    public class FlaxParrelSyncPreferences
    {
        [Tooltip("Additional check for Lockfile to check if file is actually locked.")]
        public bool AlsoCheckLockFileStaPref = true;
        
        [Tooltip("Additional folders to symbolic link to cloned projects.")]
        public List<string> AdditionalSymbolicLinkFolders = new List<string>();
    }
}