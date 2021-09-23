using System.Collections.Generic;
using System;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Platformer
{
    public class Scene
    {
        private Dictionary<string, Texture> textures; 
        private List<Entity> entities;

        public Scene()
        {
            textures = new Dictionary<string, Texture>();
            entities = new List<Entity>();
        }

        public void Spawn(Entity entity)
        {
            entities.Add(entity);
            entity.Create(this);
        }

        public Texture LoadTexture (string name)
        {
            if (textures.TryGetValue(name, out Texture found))
            {
                return found;
            }

            string fileName = $"assets/{name}.png";
            Texture texture = new Texture(fileName);
            textures.Add(name, texture);
            return texture;
        }

        public void UpdateAll(float deltaTime)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Entity entity = entities[i];
                entity.Update(this, deltaTime);
            }

            for (int i = 0; i < entities.Count;)
            {
                Entity entity = entities[i];

                if (entity.Dead)
                {
                    entities.RemoveAt(i);
                }else
                i++;
            }
        }

        public void RenderAll(RenderTarget target)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Entity entity = entities[i];
                entity.Render(target);
            }
        }
    }
}