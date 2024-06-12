using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    // Sistemas de sons
    [SerializeField] private AudioSource _bgMusicSrc;
    [SerializeField] private AudioSource _sfxSrc;

    // Listas de sons
    [SerializeField] private AudioClip[] _bgMusics;
    [SerializeField] private AudioClip[] _sfxClips;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GetVolumeBg();
    }

    #region Play Sound
    public void PlayBgMusic(int idBgMusic)
    {
        AudioClip clip = _bgMusics[idBgMusic];
        _bgMusicSrc.Stop();
        _bgMusicSrc.clip = clip;
        _bgMusicSrc.loop = true;
        _bgMusicSrc.Play();

    }
    public void PlaySfx(int idSfx)
    {
        AudioClip clip = _sfxClips[idSfx];
        _sfxSrc.PlayOneShot(clip);
    }
    #endregion

    #region Volume changer
    public void MuteSFX(bool option)
    {
        _sfxSrc.mute = option;
    }
    public void MuteBG(bool option)
    {
        _bgMusicSrc.mute = option;
    }
    public void UpdateVolumeSfx(float valor)
    {
        _sfxSrc.volume = valor;
        SaveVolumeSfx();
    }
    public void UpdateVolumeBg(float valor)
    {
        _bgMusicSrc.volume = valor;
        SaveVolumeBg();
    }
    #endregion

    #region Get and Set
    public void SaveVolumeBg()
    {
        float volumeMusic = _bgMusicSrc.volume;
        PlayerPrefs.SetFloat("VolumeBG", volumeMusic);

    }
    public void SaveVolumeSfx()
    {
     float volumeMusic = _sfxSrc.volume;
        PlayerPrefs.SetFloat("VolumeSFX", volumeMusic);
    }
    public float GetVolumeBg()
    {
        float volume = PlayerPrefs.GetFloat("VolumeBG", 0.5f);
        _bgMusicSrc.volume = volume;
        return volume;
    }
    public float GetVolumeSfx()
    {
        float volume = PlayerPrefs.GetFloat("VolumeSFX", 0.5f);
        _sfxSrc.volume = volume;
        return volume;
    }
    #endregion
}
