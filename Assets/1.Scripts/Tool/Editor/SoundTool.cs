﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using UnityObject = UnityEngine.Object;
using System.Runtime.CompilerServices;

public class SoundTool : EditorWindow
{
    public int uiWidthLarge = 450;
    public int uiwidthMiddle = 300;
    public int uiWidthSmall = 200;

    private int selection = 0;
    private Vector2 SP1 = Vector2.zero;
    private Vector2 SP2 = Vector2.zero;
    private AudioClip soundSource;
    private static SoundData soundData;

    [MenuItem("Tools/Sound Tool")]
    static void Int()
    {
        soundData = CreateInstance<SoundData>();
        soundData.LoadData();

        SoundTool window = GetWindow<SoundTool>(false, "Sound Tool");
        window.Show(); // GUI 가 보여진다.
    }

    private void OnGUI()
    {
        if( soundData == null )
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        {
            UnityObject source = soundSource;

            EditorHelper.EditorToolTopLayer(soundData, ref selection, ref source, uiwidthMiddle);
            
            soundSource = (AudioClip)source;

            EditorGUILayout.BeginHorizontal();
            {
                EditorHelper.EditorToolListLayer(ref SP1, soundData, ref selection, ref source, uiwidthMiddle);
                SoundClip sound = soundData.soundClips[selection];
                soundSource = (AudioClip)source;

                EditorGUILayout.BeginVertical();
                {
                    this.SP2 = EditorGUILayout.BeginScrollView(SP2);
                    {
                        if( soundData.GetDataCount() > 0 )
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                
                                //SoundClip sound = soundData.soundClips[selection];
                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));
                                soundData.names[selection] = EditorGUILayout.TextField("Name", soundData.names[selection] , GUILayout.Width(uiWidthLarge));
                                
                                sound.playType = (SoundPlayType)EditorGUILayout.EnumPopup("PlayType", sound.playType, GUILayout.Width(uiWidthLarge));
                                sound.MaxVolume = EditorGUILayout.FloatField("Max Volume", sound.MaxVolume , GUILayout.Width(uiWidthLarge));
                                sound.isLoop = EditorGUILayout.Toggle("LoopClip", sound.isLoop, GUILayout.Width(uiWidthLarge));
                                
                                EditorGUILayout.Separator();

                                if( this.soundSource == null && sound.clipName != string.Empty )
                                {
                                    this.soundSource = Resources.Load(sound.clipPath + sound.clipName) as AudioClip;
                                }
                                this.soundSource = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", this.soundSource, typeof(AudioClip) , false , GUILayout.Width(uiWidthLarge));

                                if( soundSource != null )
                                {
                                    //Defalut
                                    
                                    //soundData.names[selection] = soundSource.name;
                                    //sound.playType = SoundPlayType.EFFECT;
                                     
                                    
                                    



                                    sound.clipPath = EditorHelper.GetPath(soundSource);
                                    sound.clipName = soundSource.name;
                                    sound.pitch = EditorGUILayout.Slider("Pitch", sound.pitch, -3.0f, 3.0f , GUILayout.Width(uiWidthLarge));
                                    sound.dopplerLevel = EditorGUILayout.Slider("Doppler", sound.dopplerLevel, 0.0f, 5.0f, GUILayout.Width(uiWidthLarge));
                                    sound.rolloffMode = (AudioRolloffMode)EditorGUILayout.EnumPopup("Volume Rolloff", sound.rolloffMode, GUILayout.Width(uiWidthLarge));
                                    sound.minDistance = EditorGUILayout.FloatField("Min Distance", sound.minDistance, GUILayout.Width(uiWidthLarge));
                                    sound.maxDistance = EditorGUILayout.FloatField("Max Distance", sound.maxDistance, GUILayout.Width(uiWidthLarge));
                                    sound.spatialBlend = EditorGUILayout.Slider("PanLevel", sound.spatialBlend, 0.0f, 1.0f, GUILayout.Width(uiWidthLarge));

                                }
                                else
                                {
                                    sound.clipName = string.Empty;
                                    sound.clipPath = string.Empty;
                                }
                                EditorGUILayout.Separator();
                                if( GUILayout.Button("Add Loop" , GUILayout.Width(uiwidthMiddle)) )
                                {
                                    soundData.soundClips[selection].AddLoop();
                                }
                                for (int i = 0; i < soundData.soundClips[selection].checkTime.Length ; i++)
                                {
                                    EditorGUILayout.BeginVertical("box");
                                    {
                                        GUILayout.Label("Loop Step + i", EditorStyles.boldLabel);
                                        if( GUILayout.Button("Remove" , GUILayout.Width(uiwidthMiddle)))
                                        {
                                            soundData.soundClips[selection].RemoveLoop(i);
                                            return;
                                        }

                                        sound.checkTime[i] = EditorGUILayout.FloatField("Check Time", sound.checkTime[i], GUILayout.Width(uiwidthMiddle));
                                        sound.setTime[i] = EditorGUILayout.FloatField("Set Time", sound.setTime[i], GUILayout.Width(uiwidthMiddle));    

                                    }
                                    EditorGUILayout.EndVertical();
                                }




                            }
                            EditorGUILayout.EndVertical();
                        }

                    }
                    EditorGUILayout.EndScrollView();



                }
                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        //하단

        EditorGUILayout.BeginHorizontal();
        {
            if( GUILayout.Button("Reload"))
            {
                soundData = CreateInstance<SoundData>();
                soundData.LoadData();
                selection = 0;
                this.soundSource = null;

            }
            if( GUILayout.Button("Save"))
            {
                soundData.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            }

        }
        EditorGUILayout.EndHorizontal();


        
    }

    public void CreateEnumStructure()
    {
        string enumName = "SoundList";
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < soundData.names.Length  ; i++)
        {
            if( !soundData.names[i].ToLower().Contains("none"))
            {
                builder.AppendLine("     " + soundData.names[i] + " = " + i.ToString() + ",");

            }

        }
        EditorHelper.CreateEnumStructure(enumName, builder);

    }








}
