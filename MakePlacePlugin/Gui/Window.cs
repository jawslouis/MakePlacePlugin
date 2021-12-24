using Dalamud.Plugin;

namespace MakePlacePlugin.Gui
{
    public abstract class Window<T> where T : IDalamudPlugin
    {
        protected bool WindowVisible;
        protected bool WindowCanUpload;
        protected bool WindowCanImport;
        public virtual bool Visible
        {
            get => WindowVisible;
            set => WindowVisible = value;
        }
        public virtual bool CanUpload
        {
            get => WindowCanUpload;
            set => WindowCanUpload = value;
        }
        public virtual bool CanImport
        {
            get => WindowCanImport;
            set => WindowCanImport = value;
        }
        protected T Plugin { get; }

        protected Window(T plugin)
        {
            Plugin = plugin;
        }
        public void Draw()
        {
            if (Visible)
            {
                DrawUi();
                
            }
            DrawScreen();
        }

        protected abstract void DrawUi();
        protected abstract void DrawScreen();                
    }
}