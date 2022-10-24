/**
 * alex@bardicbytes.com
 * Copyright (c) 2022 Bardic Bytes, LLC
 **/
using UnityEngine;

namespace BB.BardicFramework.Platform
{
	public abstract class PlatformManager : ScriptableObject
	{
		protected bool isInitialized = false;
		public virtual bool IsInitialized => isInitialized;
		public abstract string AccountID { get; }
		public virtual string UserName { get; private set; } = "";
		public abstract string Platform { get; }
		[field: SerializeField] public AnalyticsManager AnalMan { get; private set; } = default;

		protected virtual void OnValidate()
		{
			isInitialized = false;
			Deinitialize();
		}

		public virtual void Initialize() { }
		public virtual void Deinitialize() { }

		//index based on episode collection
		public virtual bool HasDLC(int episode) => true;

		public virtual void ManagedUpdate() { }

	}
}