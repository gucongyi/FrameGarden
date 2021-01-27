using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Company;
using System;
using Company.SoundInternal;
using UnityEngine.UI;

/// <summary>
/// 游戏的声音播放。
/// </summary>
public class GameSoundPlayer : MonoBehaviour
{
    public const string MuteMusicKey = "MuteMusicKey";
    public const string MuteSoundEffectKey = "MuteSoundEffectKey";

    private bool musicMuted = false;
    private bool soundEffectMuted = false;

    public string currentMusicName; 

    //播放音效
    public SoundPlayer SfxPlayer;
    //播放背景音乐
    public SoundPlayer MusicPlayer; 
    private int bgMusicId = -1;
    private static GameSoundPlayer instance;
    public static GameSoundPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                //ZLog.Error("GameSoundPlayer not found!");
            }
            return instance;
        }
    }

    public bool MusicMuted
    {
        get
        {
            return musicMuted;
        } 
    }

    public bool SoundEffectMuted
    {
        get
        {
            return soundEffectMuted;
        } 
    }
     
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        if (instance==null)instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

        MuteMusic(PlayerPrefs.GetInt(MuteMusicKey, 0) == 1);
        MuteSoundEffect(PlayerPrefs.GetInt(MuteSoundEffectKey, 0) == 1);

        MusicPlayer.SetCustomVolume(PlayerPrefs.GetFloat("MUSIC_VOLUME",0.5f));
        SfxPlayer.SetCustomVolume(PlayerPrefs.GetFloat("MUSICEFFECTS_VOLUME", 0.5f));
    }
    public void PlayBgMusic(string musicName,bool replayWithSameName=false,SoundPlayer musicPlayer=null, Action<SoundEvent> onEnd = null)
    {
        if (musicName == currentMusicName)
            return;
        if (MusicPlayer == null && musicPlayer == null)
            return;
        
        if (bgMusicId != -1 && MusicPlayer!=null)
        {
            MusicPlayer.Stop(bgMusicId); 
        }

        if (musicPlayer != null)
        {
            if (MusicPlayer!=null&&musicPlayer != MusicPlayer&& MusicPlayer.gameObject!=null)
              Destroy(MusicPlayer.gameObject);
            MusicPlayer = musicPlayer;
            if (musicPlayer.transform.parent != null) musicPlayer.transform.parent = null;
            DontDestroyOnLoad(MusicPlayer.gameObject);
        }

        if (replayWithSameName && currentMusicName == musicName)
            return;

        currentMusicName = musicName; 
        SoundPlayer soundPlayer = MusicPlayer;
        bgMusicId = soundPlayer.Play(musicName, soundevent=>{  
            if(onEnd!=null)
                onEnd(soundevent);
            if (soundPlayer != MusicPlayer&& soundPlayer.gameObject!=null)
                Destroy(soundPlayer.gameObject);
            else
                currentMusicName = null;
        });
    } 

    public void StopBGMusic(float fadeOut=-1)
    {
        if (bgMusicId != -1)
        {
            MusicPlayer.Stop(bgMusicId,fadeOut); 
        }
        currentMusicName = null;
    }

    

    public int PlaySoundEffect(string soundName,Transform soundParent=null,Action<SoundEvent> onComplete=null)
    { 
        return SfxPlayer.Play(soundName, soundParent,onComplete);
    }
    public int PlaySoundEffect(string soundName, Vector3 soundPositon , Action<SoundEvent> onComplete = null)
    {
        return SfxPlayer.Play(soundName, soundPositon, onComplete);
    }

    public void StopSoundEffect(int soundEventID)
    {
        SfxPlayer.Stop(soundEventID);
    }

    public void MuteMusic(bool mMuted)
    {
        if (this.musicMuted == mMuted) return;
        this.musicMuted = mMuted;

        PlayerPrefs.SetInt(MuteMusicKey, mMuted ? 1 : 0 );
        PlayerPrefs.Save(); 
        SoundPlayer.ToggleMusicMute(this.musicMuted);
    }

    public void MuteSoundEffect(bool sEffectMuted)
    {
        if (this.soundEffectMuted == sEffectMuted) return;

        this.soundEffectMuted = sEffectMuted;

        PlayerPrefs.SetInt(MuteSoundEffectKey, sEffectMuted ? 1 : 0 );
        PlayerPrefs.Save(); 
        SoundPlayer.ToggleSoundsMute(this.soundEffectMuted);
    }
    private void FixedUpdate()
    {
        //点击检测
        //抬起
        if (Input.GetMouseButtonUp(0))
        {
            //播放音效点击
            var ClickGo = StaticData.UI_GetCurrentSelect();
            if (ClickGo == null)
            {
                return;
            }
            Button buttonClick = ClickGo.GetComponentInParent<Button>();
            Toggle toggleClick= ClickGo.GetComponentInParent<Toggle>();
            if ((buttonClick != null|| toggleClick!=null) && ClickGo.CompareTag(TagHelper.ChatMsgSend) == false)
            {
                GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectClickOn);
            }
        }
    }
}