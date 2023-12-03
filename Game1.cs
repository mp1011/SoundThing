using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoundThing.Extensions;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using System.Collections.Generic;

namespace SoundThing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private SoundEffectInstance _sound;
        private double _time = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var instrument = new TestInstrument();
     
            var scale = Scale.Create(ScaleType.MajorScale, 
                new NoteInfo(MusicNote.C, 2, 1.0));

            //var noteBuilder = new NoteBuilder(scale, bpm: 120, beatNote: NoteType.Quarter)
            //    .AddNotes(NoteType.Quarter, 1, 1, 5, 5, 6, 6)
            //    .AddNotes(NoteType.Half, 5)
            //    .AddNotes(NoteType.Quarter, 4, 4, 3, 3, 2, 2)
            //    .AddNotes(NoteType.Half, 1)
            //    .AddNotes(NoteType.Quarter, 5, 5, 4, 4, 3, 3)
            //    .AddNotes(NoteType.Half, 2)
            //    .AddNotes(NoteType.Quarter, 5, 5, 4, 4, 3, 3)
            //    .AddNotes(NoteType.Half, 2)
            //    .AddNotes(NoteType.Quarter, 1, 1, 5, 5, 6, 6)
            //    .AddNotes(NoteType.Half, 5)
            //    .AddNotes(NoteType.Quarter, 4, 4, 3, 3, 2, 2)
            //    .AddNotes(NoteType.Half, 1); 

            var noteBuilder = new NoteBuilder(scale, bpm: 120, beatNote: NoteType.Quarter)
                .AddQuarters(1, 1, 5, 5)
                .AddEights(6, 6, 6, 6)
                .AddHalves(5)
                .AddQuarters(4, 4, 3, 3, 2, 2)
                .AddHalves(1)
                .AddQuarters(5).AddEights(5, 5).AddQuarters(4, 4)
                .AddQuarters(3).AddEights(3, 3).AddHalves(2)
                .AddQuarters(5).AddEights(5, 5, 4, 4, 4, 4)
                .AddQuarters(3).AddEights(3, 3).AddHalves(2)
                .AddQuarters(1, 1, 5, 5)
                .AddEights(6, 6, 6, 6)
                .AddHalves(5)
                .AddQuarters(4, 4, 3, 3, 2, 2)
                .AddHalves(1);

            _player = new Player(instrument, noteBuilder);

            _sound = _player.GenerateSound();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_sound.State == SoundState.Stopped)
                _sound.Play();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
