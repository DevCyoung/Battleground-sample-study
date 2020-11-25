using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 루프 페이드인 페이드아웃 
// 오디오 클립 속성
// 
public class SoundClip
{

    public SoundPlayType playType = SoundPlayType.None;
    public string clipPath = string.Empty;
    public string clipName = string.Empty;
    public float MaxVolume = 1.0f;
    public bool isLoop = false;
    public float[] checkTime = new float[0];
    public float[] setTime = new float[0];
    public int readId = 0;

    private AudioClip clip = null;
    public int currentLoop = 0; //현재 몇회 반복중인지
    public float pitch = 0;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // 감쇄효과
    public float minDistance = 10000.0f;
    public float maxDistance = 50000.0f;
    public float spatialBlend = 1.0f;

    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;

    public Interpolate.Function ineterpolate_Func; // 보간함수
    public bool isFadeIn = false;
    public bool isFadeout = false;

    public SoundClip() { }
    public SoundClip(string clipPath , string clipName)
    {
        this.clipPath = clipPath;
        this.clipName = clipName;
    }

    public void PreLoad()
    {
        if( this.clip == null )
        {
            string fullPath = this.clipPath + this.clipName;
            clip =  Resources.Load(fullPath) as AudioClip;
        }
    }

    public void AddLoop()
    {
        this.checkTime = ArrayHelper.Add( 0.0f, this.checkTime );
        this.setTime = ArrayHelper.Add(0.0f, this.setTime);
    }

    public void RemoveLoop(int index)
    {
        this.checkTime = ArrayHelper.Remove(index, this.checkTime);
        this.setTime = ArrayHelper.Remove(index, this.setTime);
    }

    public AudioClip GetClip()
    {
        if( this.clip == null )
        {
            PreLoad();
        }

        if( this.clip == null && this.clipName != string.Empty )
        {
            //경로가 잘못된경우
            Debug.LogWarning($"사운드 클립의 경로가 잘못됬습니다. : {this.clipName} ");
            return null;

        }
        return clip;
    }

    public void ReleaseClip()
    {
        if( this.clip != null )
        {
            clip = null;
        }
    }
    public bool HasLoop()
    {
        return this.checkTime.Length > 0;
    }
    public void NextLoop()
    {
        this.currentLoop++;
        if(this.currentLoop >= this.checkTime.Length)
        {
            this.currentLoop = 0;
        }
    }
    public void CheckLoop(AudioSource source) //반복됨
    {
        if( HasLoop() && source.time >= this.checkTime[this.currentLoop])
        {
            source.time = this.setTime[this.currentLoop];
            this.NextLoop();
        }
    }

    public void FadeIn(float time , Interpolate.EaseType easeType)
    {
        this.isFadeout = false;
        this.fadeTime1 = 0.0f;// 내가원하는시간까지의 페이드인
        this.fadeTime2 = time;
        this.ineterpolate_Func = Interpolate.Ease(easeType);
        this.isFadeIn = true;
    }

    public void FadeOut(float time , Interpolate.EaseType easeType)
    {
        this.isFadeIn = false;
        this.fadeTime1 = 0.0f; // 내가원하는시간까지의 페이드아웃
        this.fadeTime2 = time;
        this.ineterpolate_Func = Interpolate.Ease(easeType);
        this.isFadeout = true;
    }

    /// <summary>
    /// 페이드 인 , 아웃 효과 프로세스 
    /// </summary>
    public void DoFade(float time , AudioSource audio)
    {
        if( this.isFadeIn == true)
        {
            this.fadeTime1 += time;
            audio.volume = Interpolate.Ease(this.ineterpolate_Func, 0, MaxVolume , this.fadeTime1 , this.fadeTime2); // 볼륨이 천천히 증가함
            if( this.fadeTime1 >= this.fadeTime2 )
            {
                this.isFadeIn = false;
            }


        }
        else if ( isFadeout == true)
        {
            this.fadeTime1 += time;
            audio.volume = Interpolate.Ease(this.ineterpolate_Func, MaxVolume, 0 - this.MaxVolume , this.fadeTime1, this.fadeTime2);
            if( this.fadeTime1 >= this.fadeTime2)
            {
                this.isFadeIn = false;
                audio.Stop();
            }

        }
    }














}
