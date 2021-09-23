using System;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Platformer
{
    public class Platform : Entity
    {
        public Platform() : base("tileset")
        {
            sprite.TextureRect = new IntRect(0, 0, 18, 18);
            sprite.Origin = new Vector2f(9, 9);
        }
    }
}
