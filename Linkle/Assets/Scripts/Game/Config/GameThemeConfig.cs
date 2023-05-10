using System.Collections.Generic;
using UnityEngine;

public class GameThemeConfig : ScriptableObject
{
    public Sprite[] sprites;
    private Dictionary<string, Sprite> spriteSheet;

    private void OnEnable()
    {
        if (sprites != null && sprites.Length > 0)
        {
            spriteSheet = new Dictionary<string, Sprite>();
            foreach (var sprite in sprites)
            {
                spriteSheet[sprite.name] = sprite;
            }
        }
    }

    public Sprite GetSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return null;
        }

        spriteSheet.TryGetValue(spriteName, out var sprite);
        return sprite;
    }
    
    public Sprite GetSpriteAt(int index)
    {
        return sprites[index];
    }
}
