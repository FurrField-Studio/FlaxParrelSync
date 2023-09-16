using Flax.Build;

public class FlaxParrelSyncEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("FlaxParrelSyncEditor");
    }
}
