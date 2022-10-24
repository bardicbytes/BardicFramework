//alex@bardicbytes.com
#pragma warning disable 414

//#if UNITY_EDITOR || DRM_FREE || UNITY_ANDROID || !(STEAMWORKS_NET || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#if DRM_FREE || UNITY_ANDROID || !STEAMWORKS_NET
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

using System;
using UnityEngine.Audio;
using SF = UnityEngine.SerializeField;
using BB.BardicFramework.EventVars;

namespace BB.BardicFramework.Platform
{
	//
	// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
	// It handles the basics of starting up and shutting down the SteamAPI for use.
	
	[CreateAssetMenu(menuName = Prefixes.Platform + "Steam")]
	public class SteamManager : PlatformManager
	{
		//public const int APP_ID = 1471600;
		//public const int EP1DLC_ID = 1885890;
		//public const int EP2DLC_ID = 1885891;
		//public const int EP3DLC_ID = 1923610;

		[SF] private uint BASE_APP_ID;// = { APP_ID, APP_ID, EP1DLC_ID, EP2DLC_ID, EP3DLC_ID };
		[SF] private uint[] EP_DLC_ID;// = { APP_ID, APP_ID, EP1DLC_ID, EP2DLC_ID, EP3DLC_ID };

		[SF] private AudioMixer mixer = default;
		[SF] private string mixerParam = "MusicVolume";
		[SF] BoolEventVar onOverlayActive;
		//[SF] private UnityEvent<bool> onOverlayActive;

		protected uint accountID = default;
		public override string AccountID => "s" + accountID.ToString();
		public override string Platform => "steam";

#if !DISABLESTEAMWORKS
	private float prevVal;
	public override string UserName => Steamworks.SteamFriends.GetPersonaName();


	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
	{
		if (nSeverity == 0 && Debug.isDebugBuild) Debug.Log(pchDebugText);
		if (nSeverity > 0) Debug.LogWarning(pchDebugText);
	}

	// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
	public override void Initialize()
	{
		if (isInitialized)
		{
			//while valid, this is rare enough to warrant logging
			Debug.Log("SteamManager Initialized while already initialized.");
			return;
		}

		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}

		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}

		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (System.DllNotFoundException e)
		{ // We catch this exception here, as it will be the first occurrence of it.
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

			Application.Quit();
			return;
		}

		isInitialized = SteamAPI.Init();
		if (!isInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);

			return;
		}

		m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
		SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);

		m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);

		accountID = SteamUser.GetSteamID().GetAccountID().m_AccountID;
	}

	
    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
    {
		onOverlayActive.Raise(pCallback.m_bActive != 0);
		if (pCallback.m_bActive != 0)
		{
			onOverlayActive.Raise(false);
			//Debug.Log("Steam Overlay has been activated");
			mixer.GetFloat(mixerParam, out prevVal);
			mixer.SetFloat(mixerParam, -80);
		}
		else
		{
			
			//Debug.Log("Steam Overlay has been closed");
			mixer.SetFloat(mixerParam, prevVal);
		}
	}
	public override bool HasDLC(int episode)
	{
		var id = EP_DLC_ID[Mathf.Clamp(episode, 0, EP_DLC_ID.Length)];
		if (id == BASE_APP_ID) return true;

		AppId_t dlcID = new AppId_t(id);
		return SteamApps.BIsDlcInstalled(dlcID);
	}

	public override void Deinitialize()
	{
		SteamAPI.Shutdown();
	}

	public override void ManagedUpdate()
	{
		if (!isInitialized)
		{
			return;
		}

		try
		{
			SteamAPI.RunCallbacks();
		}
		catch (InvalidOperationException ex)
		{
			Debug.LogWarning(ex.Message);
			isInitialized = false;
			Initialize();
		}
	}

#endif

	}
}