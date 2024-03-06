namespace NaiveAPI.RuntimeWindowUtils
{
    [CustomRuntimeDrawer(typeof(RuntimeWindow), DrawDerivedType = true)]
    public class RuntimeWindowDrawer : StandardDrawer<RuntimeWindow>
    {
        protected override void OnCreateGUI()
        {
            AddDrawer("Tab Name",            () => value.TabName,            v => value.TabName           = v);
            AddDrawer("Enable Tab",          () => value.EnableTab,          v => value.EnableTab         = v);
            AddDrawer("Context Menu", () => value.EnableContextMenu,  v => value.EnableContextMenu = v);
            AddDrawer("Limit in Parent",     () => value.LimitInParent,      v => value.LimitInParent     = v);
            AddDrawer("Limit Size",          () => value.LimitSize,          v => value.LimitSize         = v);
            AddDrawer("Dragable",            () => value.Dragable,           v => value.Dragable          = v);
            AddDrawer("Resizable",           () => value.Resizable,          v => value.Resizable         = v);
            AddDrawer("SnapLayout",          () => value.SnapLayout,         v => value.SnapLayout        = v);
            AddDrawer("SnapBorder",          () => value.SnapBorder,         v => value.SnapBorder        = v);
            AddDrawer("Popup on Click",      () => value.PopupOnClick,       v => value.PopupOnClick      = v);
            AddDrawer("Min Size",            () => value.MinSize,            v => value.MinSize           = v);
            AddDrawer("Max Size",            () => value.MaxSize,            v => value.MaxSize           = v);
        }
    }
}
