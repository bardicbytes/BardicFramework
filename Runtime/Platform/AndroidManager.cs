/**
 * alex@bardicbytes.com
 * Copyright (c) 2020 Bardic Bytes, LLC
 **/

using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Core.Prefixes.Platform + "Android")]
    public class AndroidManager : PlatformManager
    {
        public override string AccountID => "AndroidUser";
        public override string Platform => "Android";

        public override bool IsInitialized => true;
    }
}