using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidNes.Emulator;

namespace VoidNes
{
    internal class EmulatorGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private string _romPath;

        private NesSystem _system;

        public EmulatorGame(string romPath)
        {
            _romPath = romPath;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 256;
            _graphics.PreferredBackBufferHeight = 240;

            Window.AllowUserResizing = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            try
            {
                _system = new NesSystem(_romPath);
            }
            catch (Exception)
            {
                // TODO: Something better than this
                throw;
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            base.Draw(gameTime);
        }
    }
}
