using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor;
using FlaxEditor.GUI;
using FlaxEditor.GUI.ContextMenu;
using FlaxEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlaxParrelSync
{
    public class FlaxParrelSyncEditor : EditorPlugin
    {
        private ContextMenuButton _button;

        // The custom options key used to idenify them
        public const string SettingsName = "Flax Parrel Sync";
        
        public override Type GamePluginType => typeof(FlaxParrelSyncEditor);
        
        public override void InitializeEditor()
        {
            base.InitializeEditor();

            Editor.Options.AddCustomSettings(SettingsName, () => new FlaxParrelSyncPreferences());
            
            _button = Editor.UI.MenuTools.ContextMenu.AddButton("Flax Parrel Sync");
            _button.Clicked += () => new ClonesManagerWindow().Show();
        }
        
        public override void Deinitialize()
        {
            if (_button != null)
            {
                _button.Dispose();
                _button = null;
            }
            
            Editor.Options.RemoveCustomSettings(SettingsName);

            base.Deinitialize();
        }
        
        public FlaxParrelSyncPreferences GetPreferences()
        {
            FlaxParrelSyncPreferences preferences = new FlaxParrelSyncPreferences();
            
            var jObject = JObject.Parse(Editor.Options.Options.CustomSettings[SettingsName]);
            
            preferences.AdditionalSymbolicLinkFolders = jObject["AdditionalSymbolicLinkFolders"].ToObject<List<string>>();

            return preferences;
        }
    }
}
