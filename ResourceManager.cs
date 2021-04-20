using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swordfish
{
    public class ImageResource
    {
        public string name = "";
        public CachedImage image = null;

		public ImageResource(string _name, CachedImage _image)
        {
            name = _name;
            image = _image;
        }
    }

	public class ModelResource
    {
        public string name = "";
        public CachedModel model = null;

        public ModelResource(string _name, CachedModel _model)
        {
            name = _name;
            model = _model;
        }
    }

    public class ResourceManager : ScriptableObject
    {
        public List<ImageResource> imageResources;

		//  Keep this object alive
		private static ResourceManager _instance;
		public static ResourceManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ScriptableObject.CreateInstance<ResourceManager>();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

        public ResourceManager()
        {
            imageResources = new List<ImageResource>();
        }

		public void Initialize()
		{
            //  Shouldn't load on initialize but it is possible
			// Load();
		}

		//	Static methods

        public static void Load()
        {
            ResourceManager.instance.imageResources.Clear();

            string path = @"textures/items/";
			foreach (string file in Directory.GetFiles(path, "*.png"))
			{
				byte[] imgData = File.ReadAllBytes(file);
				Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                texture.LoadImage(imgData);
                texture.filterMode = FilterMode.Point;
                texture.anisoLevel = 1;
				string imgName = Path.GetFileNameWithoutExtension(file);

				ResourceManager.instance.imageResources.Add( new ImageResource( "item." + imgName, new CachedImage(texture, imgName) ) );
			}

			path = @"textures/atlas/";
			foreach (string file in Directory.GetFiles(path, "*.png"))
			{
				byte[] imgData = File.ReadAllBytes(file);
                Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                texture.LoadImage(imgData);
                texture.filterMode = FilterMode.Point;
                texture.anisoLevel = 1;
				string imgName = Path.GetFileNameWithoutExtension(file);

				ResourceManager.instance.imageResources.Add( new ImageResource( "atlas." + imgName, new CachedImage(texture, imgName) ));
			}

            path = @"textures/character/";
			foreach (string file in Directory.GetFiles(path, "*.png"))
			{
				byte[] imgData = File.ReadAllBytes(file);
                Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                texture.LoadImage(imgData);
                texture.filterMode = FilterMode.Point;
                texture.anisoLevel = 1;
				string imgName = Path.GetFileNameWithoutExtension(file);

				ResourceManager.instance.imageResources.Add( new ImageResource( "char." + imgName, new CachedImage(texture, imgName) ));
			}
        }

		public static ImageResource[] GetImages()
		{
			return ResourceManager.instance.imageResources.ToArray();
		}

        public static ImageResource GetImage(int _index)
        {
            return ResourceManager.instance.imageResources[_index];
        }

        public static ImageResource GetImage(string _name)
        {
            for (int i = 0; i < ResourceManager.instance.imageResources.Count; i++)
            {
                if (ResourceManager.instance.imageResources[i].name == _name)
                {
                    return ResourceManager.instance.imageResources[i];
                }
            }

            return null;
        }

		public static int GetImageIndex(string _name)
        {
            for (int i = 0; i < ResourceManager.instance.imageResources.Count; i++)
            {
                if (ResourceManager.instance.imageResources[i].name == _name)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}