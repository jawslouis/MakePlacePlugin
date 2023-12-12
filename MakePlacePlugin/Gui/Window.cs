// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Gui.Window`1
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using Dalamud.Plugin;

namespace MakePlacePlugin.Gui; 

public abstract class Window<T> where T : IDalamudPlugin {
    protected bool WindowCanImport;
    protected bool WindowCanUpload;
    protected bool WindowVisible;

    protected Window(T plugin) {
        this.Plugin = plugin;
    }

    public virtual bool Visible {
        get => this.WindowVisible;
        set => this.WindowVisible = value;
    }

    public virtual bool CanUpload {
        get => this.WindowCanUpload;
        set => this.WindowCanUpload = value;
    }

    public virtual bool CanImport {
        get => this.WindowCanImport;
        set => this.WindowCanImport = value;
    }

    protected T Plugin { get; }

    public void Draw() {
        if (this.Visible)
            this.DrawUi();
        this.DrawScreen();
    }

    protected abstract void DrawUi();

    protected abstract void DrawScreen();
}
