﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MonoVarmint.Widgets
{
    public partial class GameController : IMediaRenderer
    {
        Dictionary<string, SpriteFont> _fontsByName = new Dictionary<string, SpriteFont>();
        Dictionary<string, VarmintSoundEffect> _soundsByName = new Dictionary<string, VarmintSoundEffect>();
        Dictionary<string, Texture2D> _glyphsByName = new Dictionary<string, Texture2D>();
        Dictionary<string, VarmintSprite> _spritesByName = new Dictionary<string, VarmintSprite>();

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Do some visual setup to get the normalize the coordinate system, load default
        /// content, and then call out for any user-assigned work using the OnLoaded event.
        /// </summary>
        //-----------------------------------------------------------------------------------------------
        protected override void LoadContent()
        {
            Content = new EmbeddedContentManager(_graphics.GraphicsDevice);
            Content.RootDirectory = "Content";

            // Set up a back buffer to render to
            _backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            _backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            if ((float)_backBufferHeight / _backBufferWidth < 1.6)
            {
                _backBufferWidth = (int)(_backBufferHeight / 1.6);
                _backBufferXOffset = (GraphicsDevice.PresentationParameters.BackBufferWidth - _backBufferWidth) / 2;
            }

            _backBuffer = new RenderTarget2D(
                _graphics.GraphicsDevice,
                _backBufferWidth,
                _backBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);

            var scaleFactor = _backBufferWidth / 1000.0f;
            _scaleToNativeResolution = Matrix.CreateScale(new Vector3(scaleFactor, scaleFactor, 1));

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _utilityBlockTexture = Content.Load<Texture2D>("_utility_block");
            _circleTexture = Content.Load<Texture2D>("_utility_circle");
            _defaultFont = Content.Load<SpriteFont>("_utility_SegoeUI");

            _fontsByName.Add("_utility_SegoeUI", _defaultFont);
            SelectFont();

            // Widgets
            _screensByName = VarmintWidget.LoadLayout(this, _bindingContext);

            _visualTree = _screensByName["_default_screen_"];
            OnLoaded?.Invoke();
        }


        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Glyphs are textures with single images.   These can be saved as .xnb files embedded anywhere 
        /// in the project or they can be textures stored somewhere under the Content folder.  
        /// Glyphs are stored under the text key used to load them here.
        /// </summary>
        //-----------------------------------------------------------------------------------------------
        public void LoadGlyphs(params string[] names)
        {
            foreach (var name in names)
            {
                _glyphsByName.Add(name, Content.Load<Texture2D>(name));
            }
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Sprites are textures that contain a set of images, all the same size (width x heigt).   
        /// These can be saved as .xnb files embedded anywhere in the project or they can be 
        /// textures stored somewhere under the Content folder. Sprites are stored under the text 
        /// key used to load them here.
        /// </summary>
        //-----------------------------------------------------------------------------------------------
        public void LoadSprite(string name, int width, int height)
        {
            var spriteTexture = Content.Load<Texture2D>(name);
            _spritesByName.Add(name, new VarmintSprite(spriteTexture, width, height));
        }

        //-----------------------------------------------------------------------------------------------
        /// <summary>
        /// Load sound effects
        /// </summary>
        //-----------------------------------------------------------------------------------------------
        public void LoadSounds(params string[] names)
        {
            foreach (var name in names)
            {
                _soundsByName.Add(name, new VarmintSoundEffect() { Effect = Content.Load<SoundEffect>(name) });
            }
        }

    }
}