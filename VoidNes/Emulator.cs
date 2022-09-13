using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoidNes
{
    internal class Emulator : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private string _romPath;

        private Cartridge _cartridge;

        public Emulator(string romPath)
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

            _cartridge = new Cartridge(_romPath);

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
