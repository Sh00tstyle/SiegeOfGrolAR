using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuBehaviour))]
public class MenuBehaviourEditor : Editor
{


    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        //EditorGUILayout.PropertyField(serializedObject.FindProperty("_animations"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_canvasGroup"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_canvasRect"));
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _DrawArray(serializedObject.FindProperty("animations"));
        if (GUILayout.Button("Add menu interaction"))
        {
            MenuBehaviour menu = (MenuBehaviour)target;
            menu.animations.Add(new MenuAnimation());
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();



    }

    void _DrawArray(SerializedProperty pArray)
    {
        GUILayout.Label("Menu interactions", EditorStyles.boldLabel); //3
        for (int i = 0; i < pArray.arraySize; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SerializedProperty animation = pArray.GetArrayElementAtIndex(i);
            UnityEngine.Object canvasGroup = animation.FindPropertyRelative("menu").objectReferenceValue;
            AnimationOption animationOption = (AnimationOption)animation.FindPropertyRelative("animation").enumValueIndex;
            SerializedProperty swipeDetection = animation.FindPropertyRelative("swipeDetection");
            Ease ease = (Ease)animation.FindPropertyRelative("ease").enumValueIndex;
            GUILayout.Label("Destination settings", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(animation.FindPropertyRelative("menu"));

            if (canvasGroup == null)
            {

                if (GUILayout.Button("Remove menu interaction", EditorStyles.toolbarButton))
                {
                    MenuBehaviour menu = (MenuBehaviour)target;
                    menu.animations.RemoveAt(i);
                }
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUILayout.PropertyField(animation.FindPropertyRelative("stackOptions"));

            GUILayout.Label("Animation settings", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(animation.FindPropertyRelative("animation"));
            if (animationOption != AnimationOption.INSTANT)
            {
                EditorGUILayout.PropertyField(animation.FindPropertyRelative("ease"));
                if (ease == Ease.Unset)
                    EditorGUILayout.PropertyField(animation.FindPropertyRelative("customCurve"));
                EditorGUILayout.PropertyField(animation.FindPropertyRelative("easeDuration"));
                if (animationOption != AnimationOption.FADEIN)
                    EditorGUILayout.PropertyField(animation.FindPropertyRelative("direction"));
            }

            GUILayout.Label("Animation triggers", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(animation.FindPropertyRelative("triggerButton"));
            EditorGUILayout.PropertyField(animation.FindPropertyRelative("swipeDetection"));
            if (swipeDetection.objectReferenceValue != null)
                EditorGUILayout.PropertyField(animation.FindPropertyRelative("swipeDirection"));


            GUILayout.Space(5f);
            if (GUILayout.Button("Remove menu interaction", EditorStyles.toolbarButton))
            {
                MenuBehaviour menu = (MenuBehaviour)target;
                menu.animations.RemoveAt(i);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(25f);
        }


    }
}

