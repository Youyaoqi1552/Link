using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Linq;

public class ShoeBoxFontGenerator
{
    [MenuItem("Tools/Font Tools/ShoeBox Font")]
    public static void GenerateFont()
    {
        var path = EditorUtility.OpenFilePanel("Import ShoeBox Font", string.Empty, "xml");
        if (!string.IsNullOrEmpty(path))
        {
            Import(path);
        }
    }

    static void Import(string path)
    {
        var root = XElement.Load(path);
        
        var infoElem = root.Element("info");
        var fontName = infoElem!.Attribute("face")!.Value;
        var idx = fontName.LastIndexOf("Font", StringComparison.InvariantCulture);
        if (idx >= 0)
        {
            fontName = fontName.Substring(0, idx);
        }

        var common = root.Element("common");
        var textureW = (float)common!.Attribute("scaleW");
        var textureH = (float)common!.Attribute("scaleH");
        var charInfo = ParseChars(root.Element("chars"), textureW, textureH);

        var targetAssetPath = $"Assets/AssetsPackage/UI/Fonts/{fontName}/";
        var targetPath = Path.Combine(Application.dataPath, "AssetsPackage", "UI", "Fonts", $"{fontName}");
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        
        // copy texture
        File.Copy(Path.ChangeExtension(path, "png"), Path.Combine(targetPath, fontName + ".png"), true);
        AssetDatabase.Refresh();

        var texturePath = $"{targetAssetPath}{fontName}.png";
        var settings = new TextureImporterSettings();
        var importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        importer!.ReadTextureSettings(settings);
#pragma warning disable 618
        settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#pragma warning restore 618
        settings.mipmapEnabled = false;
        settings.spriteGenerateFallbackPhysicsShape = false;
        importer.SetTextureSettings(settings);
        
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.Refresh();
        
        // generate material
        var matPath = $"{targetAssetPath}{fontName}.mat";
        var shader = Shader.Find("Unlit/Transparent");
        var material = new Material(shader) 
        {
            mainTexture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D
        };
        AssetDatabase.CreateAsset(material, matPath);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        
        // generate font
        var font = new Font(fontName)
        {
            characterInfo = charInfo,
            material = material,
        };
        AssetDatabase.CreateAsset(font, $"{targetAssetPath}{fontName}.fontsettings");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static CharacterInfo[] ParseChars(XElement chars, float texW, float texH)
    {
        var count = (int)chars.Attribute("count");
        var charInfo = new CharacterInfo[count];
        var elements = chars.Elements("char");
        count = 0;

        foreach (var e in elements)
        {
            var x = (float)e.Attribute("x");
            var y = (float)e.Attribute("y");
            var w = (float)e.Attribute("width");
            var h = (float)e.Attribute("height");
            var yOffset = (float)e.Attribute("yoffset");
            var id = (int)e.Attribute("id");
            
            charInfo[count].index = id;
#pragma warning disable 618
            charInfo[count].uv.width = w / texW;
            charInfo[count].uv.height = h / texH;
            charInfo[count].uv.x = x / texW;
            charInfo[count].uv.y = 1f - (y / texH) - (h / texH);
            charInfo[count].vert.x = 0;
            charInfo[count].vert.y = -yOffset;
            charInfo[count].vert.width = w;
            charInfo[count].vert.height = -h;
            charInfo[count].width = w;
            charInfo[count].flipped = false;
#pragma warning restore 618

            count++;
        }
        return charInfo;
    }
}