/**
 * alex@bardicbytes.com
 * Copyright (c) 2020 Bardic Bytes, LLC
 **/

using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Core.Prefixes.Platform + "Web")]
    public class WebManager : PlatformManager
    {
        [SerializeField] private string siteName = "";
        public override string AccountID => "WebUser";
        public override string Platform => "Web" + siteName;

        public override bool IsInitialized => true;
    }
}