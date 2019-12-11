//using DG.Tweening;
//using System;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(Carousel))]
//public class CarouselEditor : Editor
//{
//    private bool _circles = true, _text = true;

//    public override void OnInspectorGUI()
//    {

//        var minSizeProperty = serializedObject.FindProperty("_minSize");
//        var maxSizeProperty = serializedObject.FindProperty("_maxSize");

//        var minAlphaProperty = serializedObject.FindProperty("_minAlpha");
//        var maxAlphaProperty = serializedObject.FindProperty("_maxAlpha");
//        // Start
//        GUILayout.BeginVertical(EditorStyles.helpBox); 

//        // Carousel main options
//        GUILayout.BeginVertical(EditorStyles.helpBox);
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_scaling"));
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_container"));
//        GUILayout.BeginVertical(EditorStyles.helpBox);
//        EditorGUILayout.PropertyField(serializedObject.FindProperty("_carouselPanels"), true);
//        GUILayout.EndVertical();
//        GUILayout.EndVertical();






//        // Circles  

//        _circles = EditorGUILayout.Foldout(_circles, "Circles", EditorStyles.foldout);
//        if (_circles)
//        {
//            EditorGUI.BeginChangeCheck();
//            GUILayout.BeginVertical(EditorStyles.helpBox);

//            GUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.PropertyField(serializedObject.FindProperty("_circleIndicators"), true);
//            GUILayout.EndVertical();

//            GUILayout.Label("Circle size", EditorStyles.miniBoldLabel);
//            GUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.BeginHorizontal();
//            float minSize = EditorGUILayout.FloatField(minSizeProperty.floatValue);
//            float maxSize = EditorGUILayout.FloatField(maxSizeProperty.floatValue);
//            EditorGUILayout.EndHorizontal();
//            EditorGUILayout.MinMaxSlider(ref minSize, ref maxSize, 0, 1);
//            GUILayout.EndVertical();

//            GUILayout.Label("Circle alpha", EditorStyles.miniBoldLabel);
//            GUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.BeginHorizontal();
//            float minAlpha = EditorGUILayout.FloatField(minAlphaProperty.floatValue);
//            float maxAlpha = EditorGUILayout.FloatField(maxAlphaProperty.floatValue);
//            EditorGUILayout.EndHorizontal();
//            EditorGUILayout.MinMaxSlider(ref minAlpha, ref maxAlpha, 0, 1);
//            GUILayout.EndVertical();

//            GUILayout.Label("Circle animation speed", EditorStyles.miniBoldLabel);
//            GUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.FloatField(serializedObject.FindProperty("_speed").floatValue);
//            GUILayout.EndVertical();

//            GUILayout.EndVertical();

//            bool hasChanges = EditorGUI.EndChangeCheck();
//            if (hasChanges)
//            {
//                maxSizeProperty.floatValue = maxSize;
//                minSizeProperty.floatValue = minSize;
//                maxAlphaProperty.floatValue = maxAlpha;
//                minAlphaProperty.floatValue = minAlpha;
//            }
//        }
//        //Text

//        _text = EditorGUILayout.Foldout(_text, "Text", EditorStyles.foldout);
//        if (_text)
//        {
//            GUILayout.BeginVertical(EditorStyles.helpBox);
//            EditorGUILayout.PropertyField(serializedObject.FindProperty("_nextText"));
//            EditorGUILayout.PropertyField(serializedObject.FindProperty("_previousText"));
//            GUILayout.Label("Button fade speed", EditorStyles.miniBoldLabel);
//            EditorGUILayout.FloatField(serializedObject.FindProperty("_buttonFadeSpeed").floatValue);
//            GUILayout.EndVertical();
//        }


//        // End
//        EditorGUILayout.EndVertical();

//        serializedObject.ApplyModifiedProperties();
//    }
//}

