using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    public Dictionary<string, AudioClip> _audioDic ;
    public List<AudioSource> _AudioSource;
    public AudioSource _BGMSource;
    public Dictionary<AudioClip, AudioSource> _Pool = new Dictionary<AudioClip, AudioSource>();
    public int MaxPoolCount = 10;
    public float CurSoundVolume=1f;//用于设置音量
    public float CurBGMVolume=1f;//用于设置BGM音量
    protected override void Awake()
    {
        base.Awake();
        _audioDic = new Dictionary<string, AudioClip>();
    }

    private void Start()
    {
        for (int i = 0; i < MaxPoolCount; i++)
        {
         var obj=new GameObject($"pool{i}");
         obj.transform.parent = transform;
         var a=obj.AddComponent<AudioSource>();
         _AudioSource.Add(a);
        }
    }

    public AudioSource GetAudioSource(AudioClip clip)
    {
        if (_Pool.ContainsKey(clip))
        {
            return _Pool[clip];
        }
        for (int i = 0; i < _audioDic.Count; i++)
        {
            if (!_Pool.ContainsValue(_AudioSource[i]))
            {
                return _AudioSource[i];
            }
        }

      return _AudioSource[_AudioSource.Count - 1];
    }
    public void  PlayBGM(string name, float volume = 2f)
    {
        if (volume == 2f)
        {
            volume = CurBGMVolume;
        }
        AudioClip _clip=GetSound(name);
    
        _BGMSource.Stop();
        _BGMSource.clip = _clip;
        _BGMSource.Play();
    }

    private void Update()
    {
        _BGMSource.volume = CurBGMVolume;
    }

    public void PlaySound(string name, float volume = 2f)
    {
        if (volume == 2f)
        {
            volume = CurSoundVolume;
        }
        else
        {
            volume *= CurSoundVolume;
        }
        Debug.Log("播放音乐");
        AudioClip _clip=GetSound(name);
        AudioSource a = GetAudioSource(_clip);
        a.PlayOneShot(_clip);
        a.volume = volume;
        _Pool[_clip] = a;
        StartCoroutine(RemoveWhenPlayFinish(a, _clip));
    }

    IEnumerator RemoveWhenPlayFinish(AudioSource a,AudioClip b)
    {
        yield return new WaitUntil(() => { return a.isPlaying == false; });
        if (_Pool.ContainsKey(b))
        {
            _Pool.Remove(b);
        }
    
    }
    private AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }
    
    public  AudioClip GetSound(string path)
    {
        if (!_audioDic.ContainsKey(path))
        {
            _audioDic[path] = LoadAudio(path);
        }

        return _audioDic[path];

    }
    public void PlaySound(AudioSource audioSource, string name, float volume = 2f)
    {
        if (volume == 2f)
        {
            volume = CurSoundVolume;
        }
        audioSource.PlayOneShot(GetSound(name));
        audioSource.volume = volume;
        
    }
}
