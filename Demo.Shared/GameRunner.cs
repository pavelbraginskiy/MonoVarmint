﻿#region Using Statements
using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoVarmint.Widgets;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.IO;

#endregion

namespace Demo.Shared
{
    /// <summary>
    /// Your game control starts here
    /// </summary>
    public abstract class GameRunner: IDisposable
    {
        GameController _controller;
        public Color GlobalBackgroundColor { get { return Color.DarkGray; } }
        public string DynamicText {  get { return "Current Time: " + DateTime.Now.ToLongTimeString(); } }
        public Vector2 ScreenSize { get { return _controller.ScreenSize; } }

        //-----------------------------------------------------------------------------------------------
        // ctor 
        //-----------------------------------------------------------------------------------------------
        public GameRunner()
        {
#if WINDOWS
            _controller = new GameController(500, 900);
#else
            _controller = new GameController();
#endif
            _controller.OnLoaded += () => _controller.SetScreen("MainScreen");
        }

        //-----------------------------------------------------------------------------------------------
        // Start the game 
        //-----------------------------------------------------------------------------------------------
        public void Run()
        {
            _controller.Run();
        }

        //-----------------------------------------------------------------------------------------------
        // Dispose
        //-----------------------------------------------------------------------------------------------
        public void Dispose()
        {
            _controller.Dispose();
        }
    }
}
