using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CachedImage
{
    public string name;

    public Texture2D texture;
    public Sprite sprite;

    public CachedImage(Texture2D _texture, string _name = "default")
    {
        name = _name;
        texture = _texture;

        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
