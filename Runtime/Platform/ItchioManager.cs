//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Itchio")]
    public class ItchioManager : PlatformManager
    {
        public override string AccountID => "Default Profile";
        public override string Platform => "Itchio";

        public override bool IsInitialized => true;
    }
}