#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UltimateBarTextureGenerator
{
    private const int Width = 220;
    private const int Height = 40;

    // Palette: Dark Iron / Steel Casing
    private static readonly Color C_Transparent   = new Color(0, 0, 0, 0);
    private static readonly Color C_OuterBorder    = new Color32(10, 11, 14, 255);   // #0A0B0E
    private static readonly Color C_MetalHighlight = new Color32(110, 120, 140, 255); // #6E788C
    private static readonly Color C_MetalMid       = new Color32(50, 56, 68, 255);    // #323844
    private static readonly Color C_MetalShadow    = new Color32(22, 25, 32, 255);    // #161920
    private static readonly Color C_RivetHighlight = new Color32(185, 195, 215, 255); // #B9C3D7

    // Palette: Trench Background
    private static readonly Color C_TrenchTop      = new Color32(12, 14, 18, 255);    // #0C0E12
    private static readonly Color C_TrenchBottom   = new Color32(18, 21, 28, 255);    // #12151C
    private static readonly Color C_TrenchShadow   = new Color32(6, 7, 9, 255);       // #060709

    // Palette: Grayscale Fill (Pure luminance for dynamic script tinting)
    private static readonly Color C_FillCrest      = new Color32(255, 255, 255, 255); // Top rim specular
    private static readonly Color C_FillUpper      = new Color32(220, 220, 220, 255);
    private static readonly Color C_FillCore       = new Color32(165, 165, 165, 255);
    private static readonly Color C_FillDeep       = new Color32(90, 90, 90, 255);
    private static readonly Color C_FillBase       = new Color32(35, 35, 35, 255);

    [MenuItem("Tools/Generate Ultimate Bar Textures")]
    public static void GenerateAllTextures()
    {
        string folderPath = "Assets/Sprites/UI";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        CreateAndSavePng(GenerateBackground(), $"{folderPath}/UltimateBar_Background.png");
        CreateAndSavePng(GenerateFill(),       $"{folderPath}/UltimateBar_Fill.png");
        CreateAndSavePng(GenerateOverlay(),    $"{folderPath}/UltimateBar_FrameOverlay.png");

        AssetDatabase.Refresh();
        Debug.Log("<color=#55ff55><b>[UltimateBar]</b> Successfully generated 220x40 pixel art textures in Assets/Sprites/UI/!</color>");
    }

    // -------------------------------------------------------------
    // 1. BACKGROUND: Heavy chamfered frame + dark recessed slot
    // -------------------------------------------------------------
    private static Texture2D GenerateBackground()
    {
        Texture2D tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        ClearTexture(tex, C_Transparent);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // Chamfer 3px off the corners
                if (IsCornerCut(x, y, 3)) continue;

                // 1px Outer dark outline
                if (IsOuterBorder(x, y, 1))
                {
                    tex.SetPixel(x, y, C_OuterBorder);
                }
                // 2px Metallic Bevel
                else if (IsOuterBorder(x, y, 3))
                {
                    // Directional lighting: light from top-left, shadow on bottom-right
                    bool isTopOrLeft = (y >= Height - 3) || (x <= 3);
                    tex.SetPixel(x, y, isTopOrLeft ? C_MetalHighlight : C_MetalShadow);
                }
                // Recessed Trench (Inner fill slot: x in [4, 215], y in [4, 35])
                else
                {
                    if (y >= Height - 6)
                    {
                        tex.SetPixel(x, y, C_TrenchShadow); // Inner drop shadow from top lip
                    }
                    else
                    {
                        float gradientRatio = (float)(y - 4) / (Height - 10);
                        Color trenchColor = Color.Lerp(C_TrenchBottom, C_TrenchTop, gradientRatio);
                        tex.SetPixel(x, y, trenchColor);
                    }
                }
            }
        }

        tex.Apply();
        return tex;
    }

    // -------------------------------------------------------------
    // 2. FILL: Pure grayscale energy cylinder with core highlight
    // -------------------------------------------------------------
    private static Texture2D GenerateFill()
    {
        Texture2D tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        ClearTexture(tex, C_Transparent);

        // Fill occupies inner slot: x in [4, 215], y in [4, 35] (Size: 212 x 32)
        for (int y = 4; y <= Height - 5; y++)
        {
            float yRel = (float)(y - 4) / (Height - 9); // 0 (bottom) to 1 (top)

            for (int x = 4; x <= Width - 5; x++)
            {
                Color baseFill;

                // Vertical energy cylinder shading
                if (yRel >= 0.90f) baseFill = C_FillCrest; // Sharp 2px white crest
                else if (yRel >= 0.70f) baseFill = Color.Lerp(C_FillCore, C_FillUpper, (yRel - 0.70f) / 0.20f);
                else if (yRel >= 0.35f) baseFill = Color.Lerp(C_FillDeep, C_FillCore,  (yRel - 0.35f) / 0.35f);
                else baseFill = Color.Lerp(C_FillBase, C_FillDeep, yRel / 0.35f);

                // Subtle diagonal scanlines / energy texture every 14 pixels
                if ((x + y) % 14 == 0 || (x + y + 1) % 14 == 0)
                {
                    baseFill = Color.Lerp(baseFill, Color.white, 0.15f);
                }

                tex.SetPixel(x, y, baseFill);
            }
        }

        tex.Apply();
        return tex;
    }

    // -------------------------------------------------------------
    // 3. FRAME OVERLAY: Corner brackets, rivets, & Midpoint Notch (x=110)
    // -------------------------------------------------------------
    private static Texture2D GenerateOverlay()
    {
        Texture2D tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        ClearTexture(tex, C_Transparent);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (IsCornerCut(x, y, 3)) continue;

                // Outer border and metallic bevel frame
                if (IsOuterBorder(x, y, 1))
                {
                    tex.SetPixel(x, y, C_OuterBorder);
                }
                else if (IsOuterBorder(x, y, 3))
                {
                    bool isTopOrLeft = (y >= Height - 3) || (x <= 3);
                    tex.SetPixel(x, y, isTopOrLeft ? C_MetalHighlight : C_MetalShadow);
                }

                // Decorative corner brackets (reinforced left and right caps)
                if ((x <= 7 || x >= Width - 8) && y >= 3 && y <= Height - 4)
                {
                    if (x == 7 || x == Width - 8)
                        tex.SetPixel(x, y, C_OuterBorder);
                    else
                        tex.SetPixel(x, y, C_MetalMid);
                }

                // Rivets in left and right caps
                if ((x == 4 && (y == 7 || y == Height - 8)) ||
                    (x == Width - 5 && (y == 7 || y == Height - 8)))
                {
                    tex.SetPixel(x, y, C_RivetHighlight);
                }

                // ----------------------------------------------------------
                // MIDPOINT NOTCH at x = 109-110 (15 Energy / Stagger Milestone)
                // ----------------------------------------------------------
                bool inMidX = (x == 109 || x == 110);
                bool inNotchY = (y >= Height - 9 && y <= Height - 4) || (y >= 3 && y <= 8);

                if (inMidX && inNotchY)
                {
                    // Beveled vertical pointer
                    tex.SetPixel(x, y, x == 109 ? C_RivetHighlight : C_OuterBorder);
                }

                // Small triangular diamond marker at top center
                if (y == Height - 4 && (x >= 108 && x <= 111))
                {
                    tex.SetPixel(x, y, C_RivetHighlight);
                }
            }
        }

        tex.Apply();
        return tex;
    }

    // -------------------------------------------------------------
    // Helper Utilities
    // -------------------------------------------------------------
    private static bool IsCornerCut(int x, int y, int cutSize)
    {
        bool bL = (x + y) < cutSize;
        bool tL = (x + (Height - 1 - y)) < cutSize;
        bool bR = ((Width - 1 - x) + y) < cutSize;
        bool tR = ((Width - 1 - x) + (Height - 1 - y)) < cutSize;
        return bL || tL || bR || tR;
    }

    private static bool IsOuterBorder(int x, int y, int thickness)
    {
        return x < thickness || x >= Width - thickness || y < thickness || y >= Height - thickness;
    }

    private static void ClearTexture(Texture2D tex, Color color)
    {
        Color[] clearPixels = new Color[Width * Height];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = color;
        tex.SetPixels(clearPixels);
    }

    private static void CreateAndSavePng(Texture2D texture, string assetPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        File.WriteAllBytes(assetPath, bytes);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        // Automatically configure pixel-art import settings
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
    }
}
#endif