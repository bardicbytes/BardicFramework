//alex@bardicbytes.com
using UnityEngine;

namespace BB.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Android")]
    public class AndroidManager : PlatformManager
    {
        public override string AccountID => "AndroidUser";
        public override string Platform => "Android";

        public override bool IsInitialized => true;
    }
}