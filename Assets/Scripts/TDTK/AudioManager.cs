using System.Collections.Generic;
using UnityEngine;

namespace TDTK
{
	public class AudioManager : MonoBehaviour
	{
		[Tooltip("Check to keep using the same AudioManager gameObject when loading the new scene\nOtherwise the music will get cut off as soon as a new scene loads")]
		public bool dontDestroyOnLoad = true;

		private List<AudioSource> audioSourceList = new List<AudioSource>();

		private static float musicVolume = 0.75f;

		private static float sfxVolume = 0.75f;

		[Header("Music")]
		public List<AudioClip> musicList;

		public bool playMusic = true;

		public bool shuffle;

		private int currentTrackID;

		private AudioSource musicSource;

		private static AudioManager instance;

		private GameObject thisObj;

		private Transform thisT;

		[Header("Sound Effect")]
		public AudioClip gameWonSound;

		public AudioClip gameLostSound;

		public AudioClip lostLifeSound;

		public AudioClip newWaveSound;

		public AudioClip waveClearedSound;

		public AudioClip creepReachDestinationSound;

		public AudioClip towerDestroyedSound;

		public AudioClip towerConstructedSound;

		public AudioClip towerConstructingSound;

		public AudioClip towerSoldSound;

		public AudioClip towerUpgradedSound;

		public AudioClip abilityActivatedSound;

		public AudioClip energyFullSound;

		public AudioClip fpsModeSound;

		public AudioClip fpsReloadSound;

		public AudioClip fpsSwitchWeaponSound;

		public AudioClip perkPurchasedSound;

		public static void Init()
		{
			if (!(instance != null))
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "AudioManager";
				gameObject.AddComponent<AudioManager>();
			}
		}

		private void Awake()
		{
			if (instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			instance = this;
			thisObj = base.gameObject;
			thisT = base.transform;
			if (dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(thisObj);
			}
			if (playMusic && musicList != null && musicList.Count > 0)
			{
				musicSource = thisObj.AddComponent<AudioSource>();
				musicSource.loop = false;
				musicSource.volume = musicVolume;
				musicSource.ignoreListenerVolume = true;
				if (shuffle)
				{
					currentTrackID = UnityEngine.Random.Range(0, musicList.Count);
				}
				musicSource.clip = musicList[currentTrackID];
				musicSource.Play();
			}
			audioSourceList = new List<AudioSource>();
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "AudioSource" + (i + 1);
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
				audioSource.loop = false;
				gameObject.transform.parent = thisT;
				gameObject.transform.localPosition = Vector3.zero;
				audioSourceList.Add(audioSource);
			}
			AudioListener.volume = sfxVolume;
		}

		private void Update()
		{
			if (!(musicSource != null) || musicSource.isPlaying)
			{
				return;
			}
			if (shuffle)
			{
				musicSource.clip = musicList[Random.Range(0, musicList.Count)];
			}
			else
			{
				musicSource.clip = musicList[currentTrackID];
				currentTrackID++;
				if (currentTrackID == musicList.Count)
				{
					currentTrackID = 0;
				}
			}
			musicSource.Play();
		}

		private void OnEnable()
		{
			TDTK.onLifeE += OnLostLife;
			TDTK.onGameOverE += OnGameOver;
			TDTK.onNewWaveE += OnNewWave;
			TDTK.onWaveClearedE += OnWaveCleared;
			TDTK.onCreepDestinationE += OnCreepDestination;
			TDTK.onTowerDestroyedE += OnTowerDestroyed;
			TDTK.onTowerConstructingE += OnTowerConstructing;
			TDTK.onTowerConstructedE += OnTowerConstructed;
			TDTK.onTowerUpgradedE += OnTowerUpgraded;
			TDTK.onTowerSoldE += OnTowerSold;
			TDTK.onAbilityActivatedE += OnAbilityActivated;
			TDTK.onEnergyFullE += OnEnergyFull;
			TDTK.onFPSModeE += OnFPSMode;
			TDTK.onFPSReloadE += OnFPSReload;
			TDTK.onFPSSwitchWeaponE += OnFPSSwitchWeapon;
			TDTK.onPerkPurchasedE += OnPerkPurchased;
		}

		private void OnDisable()
		{
			TDTK.onLifeE -= OnLostLife;
			TDTK.onGameOverE -= OnGameOver;
			TDTK.onNewWaveE -= OnNewWave;
			TDTK.onWaveClearedE -= OnWaveCleared;
			TDTK.onCreepDestinationE -= OnCreepDestination;
			TDTK.onTowerDestroyedE -= OnTowerDestroyed;
			TDTK.onTowerConstructingE -= OnTowerConstructing;
			TDTK.onTowerConstructedE -= OnTowerConstructed;
			TDTK.onTowerUpgradedE -= OnTowerUpgraded;
			TDTK.onTowerSoldE -= OnTowerSold;
			TDTK.onAbilityActivatedE -= OnAbilityActivated;
			TDTK.onEnergyFullE -= OnEnergyFull;
			TDTK.onFPSModeE -= OnFPSMode;
			TDTK.onFPSReloadE -= OnFPSReload;
			TDTK.onFPSSwitchWeaponE -= OnFPSSwitchWeapon;
			TDTK.onPerkPurchasedE -= OnPerkPurchased;
		}

		private void OnGameOver(bool playerWon)
		{
			if (playerWon)
			{
				_PlaySoundNew("Victory");
			}
			else
			{
				_PlaySoundNew("Lose");
			}
		}

		private void OnLostLife(int lostLife)
		{
			if (lostLife < 0 && lostLifeSound != null)
			{
				_PlaySound(lostLifeSound);
			}
		}

		private void OnNewWave(int waveID)
		{
			_PlaySoundNew("NewWave");
		}

		private void OnWaveCleared(int waveID)
		{
			if (waveClearedSound != null)
			{
				_PlaySound(waveClearedSound);
			}
		}

		private void OnCreepDestination(UnitCreep creep)
		{
			_PlaySoundNew("CreepEnd");
		}

		private void OnTowerDestroyed(UnitTower tower)
		{
			if (towerDestroyedSound != null)
			{
				_PlaySound(towerDestroyedSound);
			}
		}

		private void OnTowerConstructed(UnitTower tower)
		{
			if (towerConstructedSound != null)
			{
				_PlaySound(towerConstructedSound);
			}
		}

		private void OnTowerConstructing(UnitTower tower)
		{
			_PlaySoundNew("Buiding");
		}

		private void OnTowerSold(UnitTower tower)
		{
			_PlaySoundNew("Sell");
		}

		private void OnTowerUpgraded(UnitTower tower)
		{
			_PlaySoundNew("UpgradeTower");
		}

		private void OnAbilityActivated(Ability ab)
		{
			if (abilityActivatedSound != null)
			{
				_PlaySound(abilityActivatedSound);
			}
		}

		private void OnEnergyFull()
		{
			if (energyFullSound != null)
			{
				_PlaySound(energyFullSound);
			}
		}

		private void OnFPSMode(bool flag)
		{
			if (fpsModeSound != null)
			{
				_PlaySound(fpsModeSound);
			}
		}

		private void OnFPSReload(bool flag)
		{
			if (flag && fpsReloadSound != null)
			{
				_PlaySound(fpsReloadSound);
			}
		}

		private void OnFPSSwitchWeapon()
		{
			if (fpsSwitchWeaponSound != null)
			{
				_PlaySound(fpsSwitchWeaponSound);
			}
		}

		private void OnPerkPurchased(Perk perk)
		{
			if (perkPurchasedSound != null)
			{
				_PlaySound(perkPurchasedSound);
			}
		}

		private int GetUnusedAudioSourceID()
		{
			for (int i = 0; i < audioSourceList.Count; i++)
			{
				if (!audioSourceList[i].isPlaying)
				{
					return i;
				}
			}
			return 0;
		}

		public static void PlaySound(AudioClip clip)
		{
			if (instance == null)
			{
				Init();
			}
			instance._PlaySound(clip);
		}

		public void _PlaySoundNew(string NameSfx)
		{
			AudioController.Play(NameSfx);
		}

		public void _PlaySound(AudioClip clip)
		{
			int unusedAudioSourceID = GetUnusedAudioSourceID();
			audioSourceList[unusedAudioSourceID].clip = clip;
			audioSourceList[unusedAudioSourceID].Play();
		}

		public static void SetSFXVolume(float val)
		{
			sfxVolume = val;
			AudioListener.volume = val;
		}

		public static void SetMusicVolume(float val)
		{
			musicVolume = val;
			if ((bool)instance && (bool)instance.musicSource)
			{
				instance.musicSource.volume = val;
			}
		}

		public static float GetMusicVolume()
		{
			return musicVolume;
		}

		public static float GetSFXVolume()
		{
			return sfxVolume;
		}
	}
}
