﻿using static System.Console;
using LibRocketNet;
using OpenEQ.Engine;

using OpenEQ.GUI.MoonRocket;
using MoonSharp.Interpreter;

namespace OpenEQ.GUI {
    [MoonSharpUserData]
    public class CoreGUI {
        OEQSystemInterface systemInterface;
        OEQRenderInterface renderInterface;

        Context context;
        ElementDocument doc;
        CoreEngine engine;
        CoreMoonRocket moonRocket;

        bool debugInitialised = false;

        public bool DebugMode {
            get {
                return Core.DebugMode;
            }
            set {
                if(value && !debugInitialised) {
                    Core.InitDebugger(context);
                    debugInitialised = true;
                }
                Core.DebugMode = value;
            }
        }
        
        public CoreGUI(CoreEngine engine) {
            this.engine = engine;
            systemInterface = new OEQSystemInterface();
            renderInterface = new OEQRenderInterface(engine);
            Core.Initialize();
            moonRocket = new CoreMoonRocket();
            moonRocket.RegisterGlobal("gui", this);
            moonRocket.RegisterGlobal("game", Game.Instance);

            Core.LoadFontFace("uiassets/Delicious-Bold.otf");
            Core.LoadFontFace("uiassets/Delicious-BoldItalic.otf");
            Core.LoadFontFace("uiassets/Delicious-Italic.otf");
            Core.LoadFontFace("uiassets/Delicious-Roman.otf");

            context = Core.CreateContext("default", new Vector2i(1280, 720));
            doc = context.LoadHtmlDocument("login.rml");
            doc.Show();
        }

        public void MouseDown(int button, KeyModifier modifiers) {
            context.ProcessMouseButtonDown(button, modifiers);
        }
        public void MouseUp(int button, KeyModifier modifiers) {
            context.ProcessMouseButtonUp(button, modifiers);
        }
        public void MouseWheel(int delta, KeyModifier modifiers) {
            context.ProcessMouseWheel(delta, modifiers);
        }
        public void MouseMoved(int x, int y, KeyModifier modifiers) {
            context.ProcessMouseMove((int) (x / engine.DisplayScale), (int) (y / engine.DisplayScale), modifiers);
        }

        public void TextInput(char x) {
            context.ProcessTextInput(x);
        }

        public void Update() {
            moonRocket.Update();
            context.Update();
        }

        public void Render() {
            renderInterface.Render(context);
        }

        public void Resize(int width, int height) {
            context.Dimensions = new Vector2i((int) (width / engine.DisplayScale), (int) (height / engine.DisplayScale));
            renderInterface.Resize(context.Dimensions.X, context.Dimensions.Y);
        }

        public void Shutdown() {
            context.Dispose();
            Core.Shutdown();
        }
    }
}
