﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoundThing.Extensions;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Elements;
using SoundThing.UI.Services;
using System;
using System.Linq;

namespace SoundThing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private UIManager _uiManager;
        private MusicManager _musicManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {   
            _musicManager = new MusicManager();
            _uiManager = new UIManager();

            var songPicker = new Dropdown<Song>(
                position: new Rectangle(32, 32, 256, 32),
                spacing: 16,
                data: _musicManager.Songs,
                uiManager: _uiManager,
                musicManager: _musicManager);

            songPicker.OnItemSelected += (Song song) =>
            {
                _musicManager.Play(song);
            };
            _uiManager.Add(songPicker);

            var scaleNotePicker = new Dropdown<SongChanger>(
                new Rectangle(300, 32, 64, 32),
                16,
                Enum.GetValues(typeof(MusicNote))
                    .OfType<MusicNote>()
                    .Where(p=>p != MusicNote.Rest)
                    .Select(note => new SongChanger(
                        note.ToString().Replace("Sharp", "#"),
                        (s) => s.Scale = s.Scale.ChangeKey(note))),
                _uiManager,
                _musicManager);

            scaleNotePicker.OnItemSelected += (SongChanger c) => c.Activate(_musicManager);
            _uiManager.Add(scaleNotePicker);

            var scaleTypePicker = new Dropdown<SongChanger>(
               new Rectangle(400, 32, 280, 32),
               16,
               Enum.GetValues(typeof(ScaleType))
                   .OfType<ScaleType>()
                   .Select(scaleType => new SongChanger(
                       scaleType.ToString().AddSpacesAtCapitals(),
                       (s) => s.Scale = s.Scale.ChangeScaleType(scaleType))),
               _uiManager,
               _musicManager);
            scaleTypePicker.OnItemSelected += (SongChanger c) => c.Activate(_musicManager);
            _uiManager.Add(scaleTypePicker);

            _uiManager.Add(new ScaleChordList(
               new Rectangle(700, 32, 280, 32),           
               _uiManager,
               _musicManager));

            _uiManager.Add(new ChordList(
                 new Rectangle(1000, 32, 280, 32),
                 _uiManager,
                 _musicManager));

            _uiManager.Add(new ParameterDialSet(new Rectangle(50, 620, 64, 64),
                150,
                _uiManager, 
                _musicManager));

            _uiManager.Add(new EnumParameterPicker<GeneratorType>(new Rectangle(50, 720, 200, 32),
                _uiManager,
                _musicManager));

            var bpmDial = new BPMDial(
                new Rectangle(50,320, 64,64), 
                _uiManager, 
                _musicManager);
            _uiManager.Add(bpmDial);





            _musicManager.SongChanged += (song) => _uiManager.OnSongChanged(song);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _uiManager.Font = Content.Load<SpriteFont>("Font");

            var dial = Content.Load<Texture2D>("dial");

            var uiBackground = new Texture2D(GraphicsDevice, 16, 16);
            Color[] uiBackgroundPixels = new Color[uiBackground.Width * uiBackground.Height];
            for(int i =0; i < uiBackgroundPixels.Length; i++)
                uiBackgroundPixels[i] = Color.White;

            uiBackground.SetData(uiBackgroundPixels);
            _uiManager.Textures[TextureKey.ElementBackground]= uiBackground;
            _uiManager.Textures[TextureKey.Dial] = dial;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _musicManager.Update();
            _uiManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _uiManager.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
