/**
 * alex@bardicbytes.com
 * Copyright (c) 2020 Bardic Bytes, LLC
 **/

using UnityEngine;

namespace BB.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "DRM Free")]
    public class NoDrmManager : PlatformManager
    {
        private const string defaultPlatformName = "defaultplatform";
        public override string AccountID => "Default Profile";
        public override string Platform => defaultPlatformName;

        public override bool IsInitialized => true;
    }
}