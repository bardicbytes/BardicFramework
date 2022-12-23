//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.ProcGen
{
    [CreateAssetMenu(menuName = Prefixes.ProcGen+"TexLayer")]
    public class TexLayer : ScriptableObject, IBardicEditorable
    {
        public static implicit operator Texture2D(TexLayer tex) => tex.Generated2D;
        public static implicit operator Texture3D(TexLayer tex) => tex.Generated3D;

        private const float MAX_RANDOM_OFFSET = 99999; //arbitrary large number

        [field: SerializeField] public CombineMode Mode = CombineMode.Add;
        [field: SerializeField] public bool UseSeed { get; protected set; } = true;
        [field: SerializeField] public int Seed { get; protected set; }  = 99;
        [field: SerializeField] public float Freq { get; protected set; } = 5f;
        [field: SerializeField] public Vector3 Offset { get; protected set; } = Vector3.zero;
        [field: SerializeField] public AnimationCurve ValueOffset { get; protected set; } = default;
        [field: SerializeField] public Gradient OutputGradient { get; protected set; } = default;
        [field: SerializeField] public ExportConfig OutputConfig = new ExportConfig(512, 1);
        [field: SerializeField] public Color BGColor { get; protected set; } = Color.clear;
        [field: SerializeField] public List<TexLayer> Layers = new List<TexLayer>();
        
        [Space]
        [SerializeField] private Texture2D[] pregenerated = null;

        public int TexWidth => OutputConfig.Width;
        public int TexHeight => OutputConfig.Height;
        public int DimDepth => OutputConfig.DimDepth;

        private Texture3D generated3D;
        public Texture3D Generated3D => generated3D == null ? Generate3D(Vector3.zero, null) : null;
        public Texture2D Generated2D => pregenerated[0] == null ? Generate() : pregenerated[0];

        public string[] EditorFieldNames => new string[] { "Layers"};
        public bool DrawOtherFields => true;
        void Reset()
        {
            ValueOffset = new AnimationCurve(new Keyframe(0, 0) { inWeight = 0, outWeight = 0 }, new Keyframe(1, 1));

            GradientColorKey[] ck = { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.black, 1f) };
            GradientAlphaKey[] ak = { new GradientAlphaKey(1f, 0f) };
            OutputGradient = new Gradient();
            OutputGradient.SetKeys(ck, ak);
        }

        protected virtual void OnValidate() { }

        public Texture2D Generate(Vector2Int tileCoords, Texture2D target) => Generate(Vector3.zero, tileCoords, target);

        public Texture2D Generate(Vector3 extraOffset, Vector2Int tileCoords, Texture2D target)
        {
            Vector3 pixelCoords = new Vector3(OutputConfig.Width * tileCoords.x, OutputConfig.Height * tileCoords.y, OutputConfig.DimDepth);
            return Generate(pixelCoords + extraOffset, target);
        }

        public Texture2D Generate(Vector3 extraOffset) => Generate(extraOffset, null);
        public Texture2D Generate() => Generate(Vector3.zero, null);
        public Texture2D Generate(Vector3 extraOffset, Texture2D targetAsset) => Generate<Texture2D>(extraOffset, targetAsset);
        public Texture3D Generate3D(Vector3 extraOffset, Texture3D targetAsset) => Generate(extraOffset, targetAsset);

        private T Generate<T>(Vector3 extraOffset, T targetAsset) where T : Texture
        {
            UnityEngine.Profiling.Profiler.BeginSample("Generate Texture " + name);
            if (Layers == null || Layers.Count == 0) return default(T);

            bool is2D = typeof(T) == typeof(Texture2D);

            Texture texture = targetAsset;
            if (texture != null && is2D) OutputConfig.ApplyTo2D((Texture2D)texture);
            int generationDepth = DimDepth;

            if (typeof(T) == typeof(Texture2D))
            {
                if (texture == null) texture = OutputConfig.Generate2D(GetTexName());
                generationDepth = 1;
            }
            else if (typeof(T) == typeof(Texture3D))
            {
                if (texture == null) texture = OutputConfig.Generate3D(GetTexName());
            }

            UnityEngine.Profiling.Profiler.EndSample();

            bool use32 = texture.width == texture.height;
            Color32[] colors32 = null;
            Color[] colors = null;
            if (use32)
                colors32 = new Color32[TexHeight * TexWidth * generationDepth];
            else
                colors = new Color[TexHeight * TexWidth * generationDepth];

            UnityEngine.Profiling.Profiler.BeginSample("Layer Combination " + name);
            int colorIndex = 0;
            for (int z = 0; z < generationDepth; z++)
                for (int y = 0; y < TexHeight; y++)
                    for (int x = 0; x < TexWidth; x++)
                    {
                        Color color = BGColor;
                        for (int l = 0; l < Layers.Count; l++)
                        {
                            if (Layers[l] == null) continue;
                            color = Combine(color,
                                GetPixelColor(Layers[l], new Vector3(x + OutputConfig.TexRect.x, OutputConfig.TexRect.y + y, z) + extraOffset),
                                Layers[l].Mode);
                        }
                        if (use32) colors32[colorIndex++] = color;
                        else colors[colorIndex++] = color;
                    }

            if (is2D)
            {
                if (use32)
                    ((Texture2D)texture).SetPixels32(colors32);
                else
                    ((Texture2D)texture).SetPixels(colors);
                ((Texture2D)texture).Apply();
            }
            else
            {
                ((Texture3D)texture).SetPixels32(colors32);
                ((Texture3D)texture).Apply();
            }

            UnityEngine.Profiling.Profiler.EndSample();

            if (Application.isPlaying)
            {
                if (pregenerated == null || pregenerated.Length == 1) pregenerated = new Texture2D[1];
                if (is2D) pregenerated[0] = (Texture2D)texture;
                else generated3D = (Texture3D)texture;
            }
            return (T)texture;
            
            string GetTexName(string suffix = "")
            {
                return string.Format("Tex_{0}_{1}x{2}x{3}{4}", name, TexWidth, TexHeight, DimDepth, suffix);
            }
        }

        private Color GetPixelColor(TexLayer layer, Vector3 coords)
        {
            if (layer == null) return (coords.x % 2 == 0 || coords.y % 2 == 0) ? Color.magenta : Color.black;

            return layer.CalcColor(new Vector3(
                coords.x / TexWidth * layer.Freq,
                coords.y / TexHeight * layer.Freq,
                coords.z / DimDepth * layer.Freq
                ));
        }

        private Color Combine(Color baseColor, Color layerColor, CombineMode mode)
        {
            switch (mode)
            {
                case CombineMode.Add:
                    baseColor += layerColor;
                    break;
                case CombineMode.Subtract:
                    baseColor -= layerColor;
                    break;
                case CombineMode.Multiply:
                    baseColor.r *= layerColor.r;
                    baseColor.g *= layerColor.g;
                    baseColor.b *= layerColor.b;
                    baseColor.a *= layerColor.a;
                    break;
                case CombineMode.Divide:
                    baseColor.r /= layerColor.r;
                    baseColor.g /= layerColor.g;
                    baseColor.b /= layerColor.b;
                    baseColor.a /= layerColor.a;
                    break;
            }
            return baseColor;
        }

        public virtual Color CalcColor(Vector3 rawCoords)
        {
            float value = Calc(rawCoords);

            Color color = Color.magenta;
            try { color = OutputGradient.Evaluate(value); }
            catch (System.Exception ex) { Debug.LogWarning("Noise Layer probably has a misconfugured color gradient.\n" + ex.Message, this); }

            return color;
        }
        public Vector3 ApplyOffset(Vector3 rawCoords)
        {
            if (UseSeed) Random.InitState(Seed);
            float randomOffsetX = UseSeed ? Random.Range(-MAX_RANDOM_OFFSET, MAX_RANDOM_OFFSET) : 0;
            if (UseSeed) Random.InitState((int)(Seed / 2f)); //reinitializing so both values aren't the same
            float randomOffsetY = UseSeed ? Random.Range(-MAX_RANDOM_OFFSET, MAX_RANDOM_OFFSET) : 0;
            if (UseSeed) Random.InitState((int)(Seed / 2f)); //reinitializing so both values aren't the same
            float randomOffsetZ = UseSeed ? Random.Range(-MAX_RANDOM_OFFSET, MAX_RANDOM_OFFSET) : 0;

            return rawCoords + Offset + new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ); ;
        }

        /// <summary>
        /// Offsets the value with the animation curve
        /// </summary>
        /// <param name="rawCoords">the pixel coordiantes</param>
        /// <returns>the offset value</returns>
        public float Calc(Vector3 rawCoords)
        {
            float value = CalcFlat(rawCoords);
            try { value = ValueOffset.Evaluate(value); }
            catch (System.Exception ex) { Debug.LogWarning("Noise Layer probably has a misconfigured value offset curve.\n" + ex.Message, this); }
            return value;
        }

        public float CalcFlat(Vector3 coords) => Eval(ApplyOffset(coords));

        protected virtual float Eval(Vector3 coords) => 0f;
    }
}