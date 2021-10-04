using System.Text;
using System.Collections.Generic;
using System;
using SFML.System;
using SFML.Graphics;
using System.IO;

namespace Platformer
{
    public class Scene
    {
        private Dictionary<string, Texture> textures; 
        private List<Entity> entities;
        private string currentScene;
        private string nextScene;

        public Scene()
        {
            textures = new Dictionary<string, Texture>();
            entities = new List<Entity>();
        }

        public void Spawn(Entity entity)  //Adds an entity to the list and loads the correct texture. 
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

        public bool TryMove(Entity entity, Vector2f movement)   //Checks collisions and limits movements based on those
        {
            entity.Position += movement;
            bool collided = false;
            
            for (int i = 0; i < entities.Count; i++)
            {
                Entity other = entities[i];
                if (!other.Solid) continue;
                if (other == entity) continue;

                FloatRect boundsA = entity.Bounds;
                FloatRect boundsB = other.Bounds;
                if (Collision.RectangleRectangle(boundsA, boundsB, out Collision.Hit hit))
                {
                    entity.Position += hit.Normal * hit.Overlap;
                    i = -1;
                    collided = true;
                }
            }

            return collided;
        }

        public void Reload()
        {
            nextScene = currentScene;
        }

        public void Load(string nextScene)
        {
            this.nextScene = nextScene;
        }

        private void HandleSceneChange()    //Parses the text scene text file and creates the entities based on different characters
        {
            if (nextScene == null) return;
            entities.Clear();
            Spawn(new Background());

            string file = $"assets/{nextScene}.txt";
            Console.WriteLine($"Loading scene '{file}'");

            foreach (var line in File.ReadLines(file, Encoding.UTF8))
            {
                string parsed = line.Trim();
                int commentAt = parsed.IndexOf('#');

                if (commentAt >= 0)
                {
                    parsed = parsed.Substring(0, commentAt);
                    parsed = parsed.Trim();
                }

                if (parsed.Length < 1) continue;

                string[] words = parsed.Split(" ");
                
                Vector2f pos = new Vector2f(int.Parse(words[1]), int.Parse(words[2])); //Parses numbers form text file into a vector position

                switch (words[0])
                {
                    case "w":
                        Spawn(new Platform{Position = pos});
                        break;
                    case "d":
                        Spawn(new Door{Position = pos, NextRoom = words[3]});
                        break;
                    case "k":
                        Spawn(new Key{Position = pos});
                        break;
                    case "h":
                        Spawn(new Hero{Position = pos});
                        break;
                }
            }

            currentScene = nextScene;
            nextScene = null;
        }

        public bool FindByType<T>(out T found) where T : Entity
        {
            foreach (Entity entity in entities)
            {
                if (entity is T typed)
                {
                    found = typed;
                    return true;
                }
            }

            found = default(T);
            return false;
        }

        public void UpdateAll(float deltaTime)
        {
            HandleSceneChange();
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
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                entity.Render(target);
            }
        }
    }
}
