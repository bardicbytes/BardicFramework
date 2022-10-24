/**
 * alex@bardicbytes.com
 * Copyright (c) 2020 Bardic Bytes, LLC
 **/

using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Core.Prefixes.Platform + "Itchio")]
    public class ItchioManager : PlatformManager
    {
        public override string AccountID => "Default Profile";
        public override string Platform => "Itchio";

        public override bool IsInitialized => true;
    }
}