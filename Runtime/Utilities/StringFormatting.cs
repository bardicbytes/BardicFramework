using UnityEngine;

namespace BardicBytes.BardicFramework.Utilities
{
    public class StringFormatting : MonoBehaviour
    {
        const string backingFieldPost = "k__BackingField";
        public static string GetBackingFieldName(string propName) => string.Format("<{1}>{0}", backingFieldPost, propName);

    }
}
