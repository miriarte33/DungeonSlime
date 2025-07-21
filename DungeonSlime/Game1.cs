using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1 : Core
{
    private Sprite _slime;
    private Sprite _bat;

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        var atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
        _slime = atlas.CreateSprite("slime");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        _bat = atlas.CreateSprite("bat");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime at a scale of 4x
        _slime.Draw(SpriteBatch, Vector2.One);
        // Put the bat 10px to the right of the slime.
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));

        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
