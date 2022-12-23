//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Web")]
    public class WebManager : PlatformManager
    {
        [SerializeField] private string siteName = "";
        public override string AccountID => "WebUser";
        public override string Platform => "Web" + siteName;

        public override bool IsInitialized => true;
    }
}