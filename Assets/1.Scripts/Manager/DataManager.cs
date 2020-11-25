using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데이터를 읽고불러옴 
/// </summary>
public class DataManager : MonoBehaviour
{
    private static SoundData soundData = null;
    private static EffectData effectData = null;


    private void Start()
    {
        if( effectData == null)
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();

        }
        if( soundData == null )
        {
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();
        }
        
    }

    public static EffectData EffectData()
    {
        if( effectData == null )
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();
        }
        return effectData;  
    }
    public static SoundData SoundData()
    {
        if(soundData == null )
        {
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();
        }
        return soundData;
    }
}
