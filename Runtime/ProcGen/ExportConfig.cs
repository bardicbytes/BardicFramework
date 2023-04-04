//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.ProcGen
{
    public enum CombineMode { Add = 0, Subtract = 1, Multiply = 2, Divide = 3 }
    public enum DimMode { Auto = 0, TwoD = 1, ThreeD = 2 }

    [System.Serializable]
    public class ExportConfig
    {
        [field: HideInInspector] [field: SerializeField] public string ExportGuid { get; protected set; }

        [field: SerializeField] public bool ForceSquare { get; protected set; } = true;
        [field: SerializeField] public RectInt TexRect { get; protected set; } = new RectInt(0, 0, 256, 256);
        [Tooltip("The z depth of the 3d noise generated. (for voxels)")]
        [field: SerializeField] public int DimDepth { get; protected set; } = 1;
        [field: Space]
        [field: SerializeField] public TextureWrapMode WrapMode { get; protected set; } = TextureWrapMode.Repeat;
        [field: SerializeField] public FilterMode FilterMode { get; protected set; } = FilterMode.Bilinear;
        [field: SerializeField] public int AnisoLvl { get; protected set; } = 9;
        [field: SerializeField] public TextureFormat TexFormat { get; protected set; } = TextureFormat.RGB24;
        [field: SerializeField] public bool MipMapsEnabled { get; protected set; } = false;

        public int Height => TexRect.height;
        public int Width => TexRect.width;

        public ExportConfig(int scale, int depth)
        {
            var r = TexRect;
            r.width = scale;
            r.height = scale;
            TexRect = r;
            this.DimDepth = depth;
        }

        public void OnValidate()
        {
            var r = TexRect;

            if (Width < 0) r.width = 0;
            if (Height < 0) r.height = 0;

            if (DimDepth < 0) DimDepth = 0;
            if (ForceSquare && Width != Height) r.height = Width;
            TexRect = r;
        }

        public Texture2D Generate2D(string name)
        {
            return new Texture2D(Width, Height, TexFormat, MipMapsEnabled)
            {
                name = name,
                wrapMode = WrapMode,
                filterMode = FilterMode,
                anisoLevel = AnisoLvl
            };
        }

        public Texture3D Generate3D(string name)
        {
            return new Texture3D(TexRect.width, TexRect.height, DimDepth, TexFormat, MipMapsEnabled)
            {
                name = name,
                wrapMode = WrapMode,
                filterMode = FilterMode,
                anisoLevel = AnisoLvl
            };
        }

        public void ApplyTo2D(Texture2D target)
        {
            target.wrapMode = WrapMode;
            target.filterMode = FilterMode;
            target.anisoLevel = AnisoLvl;
            target.Reinitialize(Width, Height, TexFormat, MipMapsEnabled);
        }
    }
}