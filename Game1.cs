using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Services.Instruments;
using SoundThing.Services.NoteEventBuilders;

namespace SoundThing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Band _band;
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
           
            var scale = Scale.Create(ScaleType.MajorScale, 
                new NoteInfo(MusicNote.C, 2, 1.0));

            var drumBuilder = new PercussionBuilder(120, NoteType.Quarter);

            drumBuilder.AddGroup(NoteType.Quarter, DrumPart.Kick, DrumPart.HiHat)
                       .Add(NoteType.Quarter, DrumPart.HiHat)
                       .AddGroup(NoteType.Quarter, DrumPart.Snare, DrumPart.HiHat)
                       .Add(NoteType.Quarter, DrumPart.HiHat)
                       .Repeat(11);

            //var noteBuilder = new ScaleNoteBuilder(scale, bpm: 120, beatNote: NoteType.Quarter)
            //    .AddNotes(NoteType.Quarter, 4, 4, 4, 4);

            var noteBuilder = new ScaleNoteBuilder(scale, bpm: 120, beatNote: NoteType.Quarter)
                .AddQuarters(1,0,1,0)
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
                .AddHalves(1)
                .AddWholes(0);

            var noteBuilder2 = new ScaleNoteBuilder(scale, bpm: 120, beatNote: NoteType.Quarter)
                .AddWholes(1, 1)
                .AddWholes(5, 5)
                .AddWholes(1, 1)
                .AddWholes(5, 5)
                .AddWholes(1, 1)
                .SetOctave(1)
                .AddHalves(5,7)
                .SetOctave(2)
                .AddHalves(1)
                .AddWholes(0);

           // var noteBuilder3 = noteBuilder.ChangeScale(scale.ChangeOctave(3));

            var player1 = new Player(new TestInstrument(), noteBuilder);
            var player3 = new Player(new TestDrumKit(), drumBuilder);
            var player2 = new Player(new SineInstrument(), noteBuilder2);
          //  var player4 = new Player(new SineInstrument(), noteBuilder3);

            _band = new Band(player1);
              _sound = _band.GenerateSound();
            
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
