﻿using UnityEngine;
using UnityEditor;
using System.Text;
using UnityObject = UnityEngine.Object;
using UnityEditor.EditorTools;

//Editor폴더 안에넣어야 제대로 동작한다.중요!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// 게임빌드엔 포함되지않음 



/// <summary>
/// 
/// </summary>
public class EffectTool : EditorWindow // <= 이것을 상속받으면 툴을 띄울수 있게된다.
{
    // UI 그리는데 필요한 변수들
    public int uiWidthLarger = 300; // < = 픽셀값
    public int uiWidthMiddle = 200;
    private int selection = 0;

    private Vector2 SP1 = Vector2.zero;
    private Vector2 SP2 = Vector2.zero;

    //이펙트 클립 
    private GameObject effectSource;

    //이펙트 데이터를 가지고있습니다.
    private static EffectData effectdata; // effecClips 를 가지고있다.


    [MenuItem("Tools/Effect Tool")] //상단바에 이펙트 툴을 클릭시
    static void Init() // 툴생성자
    {
        effectdata = ScriptableObject.CreateInstance<EffectData>();
        effectdata.LoadData();

        EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
        window.Show();
    }

    // 툴은 생성됬으니 그려줌 
    private void OnGUI()
    {
        if (effectdata == null)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        {

            //상단 ADD REMOVE COPY
            UnityObject source = effectSource;
            EditorHelper.EditorToolTopLayer(effectdata, ref selection, ref source, this.uiWidthMiddle);  // source 에는 게임오브젝트도 들어갈수있고 사운드클립도 들어갈수있기에
            // UnityObject형으로 넣는다.
            effectSource = (GameObject)source;

            EditorGUILayout.BeginHorizontal();
            {
                // 중간 데이터목록 
                EditorHelper.EditorToolListLayer(ref SP1, effectdata, ref selection, ref source, this.uiWidthLarger);
                effectSource = (GameObject)source;


                // 설정부분
                EditorGUILayout.BeginVertical();
                {
                    SP2 = EditorGUILayout.BeginScrollView(SP2);
                    {
                        if(effectdata.GetDataCount() > 0 )
                        {
                            // 데이터가 있다면

                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarger));

                                effectdata.names[selection] = EditorGUILayout.TextField( effectdata.names[selection], GUILayout.Width(uiWidthLarger * 1.5f) );
                                effectdata.effectClips[selection].effectType = (EffectType)EditorGUILayout.EnumPopup("이펙트 타입" , effectdata.effectClips[selection].effectType ,
                                    GUILayout.Width(uiWidthLarger));

                                EditorGUILayout.Separator();

                                if( effectSource == null && effectdata.effectClips[selection].effectName != string.Empty )
                                {

                                    effectdata.effectClips[selection].PreLoad();
                                    effectSource = Resources.Load(effectdata.effectClips[selection].effectPath +
                                                                  effectdata.effectClips[selection].effectName) as GameObject;
                                }
                                effectSource = (GameObject)EditorGUILayout.ObjectField("이펙트", this.effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarger * 1.5f));
                                if( effectSource != null )
                                {
                                    effectdata.effectClips[selection].effectPath = EditorHelper.GetPath(this.effectSource);
                                    effectdata.effectClips[selection].effectName = effectSource.name;
                                }
                                else
                                {
                                    effectdata.effectClips[selection].effectPath = string.Empty;
                                    effectdata.effectClips[selection].effectName = string.Empty;
                                    effectSource = null;
                                }
                                EditorGUILayout.Separator();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

        // 하단

        EditorGUILayout.BeginHorizontal();
        {
            if( GUILayout.Button("Reload String"))
            {
                effectdata = CreateInstance<EffectData>();
                effectdata.LoadData();
                selection = 0;
                this.effectSource = null;
            }
            if( GUILayout.Button("Save"))
            {
                EffectTool.effectdata.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); // 세이브시 한번 리프레쉬 하고 저장

            }
        }
        EditorGUILayout.EndHorizontal();



    }

    public void CreateEnumStructure()
    {
        string enumName = "EffectList";
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();
        for (int i = 0; i < effectdata.names.Length; i++)
        {
            if(effectdata.names[i] != string.Empty)
            {
                builder.AppendLine("    " + effectdata.names[i] + " =  " + i + ",");
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }
















}
