using System.IO;
using System.Net.Mime;
using FlaxEngine;
using FlaxEditor;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Elements;
using FlaxEditor.Windows;
using FlaxEngine.GUI;

namespace FlaxParrelSync
{
    public class ClonesManagerWindow : CustomEditorWindow
    {
        /// <summary>
        /// Returns true if project clone exists.
        /// </summary>
        public bool isCloneCreated
        {
            get { return ClonesManager.GetCloneProjectsPath().Count >= 1; }
        }

        public override void Initialize(LayoutElementsContainer layout)
        {
            // If it is a clone project...
            if (ClonesManager.IsClone())
            {
                //Find out the original project name and show the help box
                string originalProjectPath = ClonesManager.GetOriginalProjectPath();

                if (originalProjectPath == string.Empty)
                {
                    // If original project cannot be found, display warning message.
                    layout.Label("Warning:\n" +
                                 "This project is a clone, but the link to the original seems lost.\n" +
                                 "You have to manually open the original and create a new clone instead of this one.\n", TextAlignment.Center);
                }
                else
                {
                    // If original project is present, display some usage info.
                    layout.Label("This project is a clone of the project '" + Path.GetFileName(originalProjectPath) + "'.\n" +
                                 "If you want to make changes the project files or manage clones, please open the original project through Flax Launcher.", TextAlignment.Center);
                }

                var cloneArgumentsPanel = layout.HorizontalPanel();

                var argumentsLabel = cloneArgumentsPanel.Label("Arguments");

                string argumentFilePath = Path.Combine(ClonesManager.GetCurrentProjectPath(), ClonesManager.ArgumentFileName);
                //Need to be careful with file reading / writing since it will effect the deletion of
                //  the clone project(The directory won't be fully deleted if there's still file inside being read or write).
                //The argument file will be deleted first at the beginning of the project deletion process
                //to prevent any further being read and write.
                //Will need to take some extra cautious if want to change the design of how file editing is handled.
                if (File.Exists(argumentFilePath))
                {
                    string argument = File.ReadAllText(argumentFilePath, System.Text.Encoding.UTF8);
                    var argumentsTextBox = cloneArgumentsPanel.TextBox(true);
                            
                    argumentsTextBox.Text = "";
                    argumentsTextBox.Control.SetAnchorPreset(AnchorPresets.StretchAll, false, false);
                    argumentsTextBox.Control.Offsets = new Margin(argumentsLabel.Control.Width + 10, 0, 0, 0);

                    File.WriteAllText(argumentFilePath, argumentsTextBox.Text, System.Text.Encoding.UTF8);
                }
                else
                {
                    cloneArgumentsPanel.Label("No argument file found.");
                    cloneArgumentsPanel.Control.SetAnchorPreset(AnchorPresets.StretchAll, false, false);
                    cloneArgumentsPanel.Control.Offsets = new Margin(argumentsLabel.Control.Width + 10, 0, 0, 0);
                }
            }
            else // If it is an original project...
            {
                if (isCloneCreated)
                {
                    layout.Label("Clones of this Project");

                    var cloneProjectsPath = ClonesManager.GetCloneProjectsPath();
                    for (int i = 0; i < cloneProjectsPath.Count; i++)
                    {
                        var mainPanel = layout.VerticalPanel();
                        mainPanel.Panel.BackgroundColor = Color.DimGray;
                        string cloneProjectPath = cloneProjectsPath[i];
        
                        bool isOpenInAnotherInstance = ClonesManager.IsCloneProjectRunning(cloneProjectPath);

                        if (isOpenInAnotherInstance == true)
                        {
                            var statusLabel = mainPanel.Label("Clone " + i + " (Running)");
                            statusLabel.Label.Font = statusLabel.Label.Font.GetBold();
                        }
                        else
                        {
                            var statusLabel = mainPanel.Label("Clone " + i);
                        }

                        //Path
                        var projectPathPanel = mainPanel.HorizontalPanel();
                        projectPathPanel.Panel.AutoSize = true;

                        var cloneProjectPathTextBox = projectPathPanel.TextBox(false);
                        cloneProjectPathTextBox.Text = cloneProjectPath;

                        var viewFolderButton = projectPathPanel.Button("View Folder");
                        viewFolderButton.Control.SetAnchorPreset(AnchorPresets.MiddleRight, false, false);
                        viewFolderButton.Button.Clicked += () =>  ClonesManager.OpenProjectInFileExplorer(cloneProjectPath);
                        
                        cloneProjectPathTextBox.Control.SetAnchorPreset(AnchorPresets.StretchAll, false, false);
                        cloneProjectPathTextBox.Control.Offsets = new Margin(0, viewFolderButton.Control.Width + 10, 0, 0);

                        //Arguments
                        var cloneArgumentsPanel = mainPanel.HorizontalPanel();
                        
                        var argumentsLabel = cloneArgumentsPanel.Label("Arguments");

                        string argumentFilePath = Path.Combine(cloneProjectPath, ClonesManager.ArgumentFileName);
                        //Need to be careful with file reading / writing since it will effect the deletion of
                        //the clone project(The directory won't be fully deleted if there's still file inside being read or write).
                        //The argument file will be deleted first at the beginning of the project deletion process
                        //to prevent any further being read and write.
                        //Will need to take some extra cautious if want to change the design of how file editing is handled.
                        if (File.Exists(argumentFilePath))
                        {
                            string argument = File.ReadAllText(argumentFilePath, System.Text.Encoding.UTF8);
                            var argumentsTextBox = cloneArgumentsPanel.TextBox(true);
                            
                            argumentsTextBox.Text = "";
                            argumentsTextBox.Control.SetAnchorPreset(AnchorPresets.StretchAll, false, false);
                            argumentsTextBox.Control.Offsets = new Margin(argumentsLabel.Control.Width + 10, 0, 0, 0);

                            argumentsTextBox.TextBox.EditEnd += () =>
                            {
                                File.WriteAllText(argumentFilePath, argumentsTextBox.Text, System.Text.Encoding.UTF8);
                            };
                        }
                        else
                        {
                            cloneArgumentsPanel.Label("No argument file found.");
                        }

                        //Open in editor
                        var openInEditorButton = mainPanel.Button("Open in New Editor");
                        openInEditorButton.Button.Clicked += () =>
                        {
                            if(isOpenInAnotherInstance) return;
                            
                            ClonesManager.OpenProject(cloneProjectPath);
                        };
                        
                        //Delete
                        var deleteButton = mainPanel.Button("Delete");
                        deleteButton.Button.Clicked += () =>
                        {
                            ClonesManager.DeleteClone(cloneProjectPath);
                        };
                    }

                    var newCloneButton = layout.Button("Add new clone");
                    newCloneButton.Button.Clicked += () => ClonesManager.CreateCloneFromCurrent();
                }
                else
                {
                    // If no clone created yet, we must create it.
                    layout.Label("No project clones found. Create a new one!");

                    var newCloneButton = layout.Button("Create new clone");
                    newCloneButton.Button.ButtonClicked += button => ClonesManager.CreateCloneFromCurrent();
                }
            }
        }
    }
}
