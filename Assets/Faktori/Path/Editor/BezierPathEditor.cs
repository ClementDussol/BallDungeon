using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Faktori.Path
{
    [CustomEditor(typeof(BezierPath))]
    public class BezierPathEditor : Editor
    {
        private BezierPath _path;
        private Transform _handleTransform;
        private Quaternion _handleRotation;

        private bool _canEdit = false;
        private int _tangentModeSelected = 0;
        private int _selectedBezierPointIndex;

        private SerializedProperty _bezierPoints;
        private GUIContent EditPathIcon => new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Faktori/Icons/ShapeToolPro.png"), "Edit Path");

        private void OnSceneGUI()
        {
            _path = target as BezierPath;
            _handleTransform = _path.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;

            DrawLines();
            DrawHandles();
        }
    
        private void DrawHandles()
        {
            for (int i = 0; i < _path.Count; i++)
            {
                DrawHandleForPointAtIndex(i);
            }
        }

        public override void OnInspectorGUI()
        {
            _bezierPoints = serializedObject.FindProperty("_points");
        
            DrawDefaultInspector();
        
            GUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding = new RectOffset(8, 8, 6, 6);
        
            GUIStyle styleToggled = new GUIStyle(style);

            _canEdit = GUILayout.Toggle(_canEdit, EditPathIcon, style, GUILayout.ExpandWidth(false));
        
            GUILayout.Label("Edit Path");
            GUILayout.EndHorizontal();
        
            if(_canEdit)
                DrawBezierPointInspector(_selectedBezierPointIndex);
        }

        public void DrawBezierPointInspector(int index)
        {
            EditorGUILayout.PropertyField(_bezierPoints.GetArrayElementAtIndex(index));
            serializedObject.ApplyModifiedProperties();
        }
    
        private Vector3 DrawHandleForPointAtIndex(int index)
        {
            EditorGUI.BeginChangeCheck();

            BezierPoint bezierPoint = _path.GetBezierPoint(index);
        
            Vector3 position = Handles.FreeMoveHandle(bezierPoint.position, _handleRotation, .25f, Vector3.zero, Handles.SphereHandleCap);
            Vector3 inTangentPosition = Handles.FreeMoveHandle(position + bezierPoint.inTangent, _handleRotation, .1f, Vector3.zero, Handles.SphereHandleCap);
            Vector3 outTangentPosition = Handles.FreeMoveHandle(position + bezierPoint.outTangent, _handleRotation, .1f, Vector3.zero, Handles.SphereHandleCap);
        
            Handles.DrawLine(bezierPoint.position, inTangentPosition, 1.5f);
            Handles.DrawLine(bezierPoint.position, outTangentPosition, 1.5f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_path, "Move Point");
                EditorUtility.SetDirty(_path);
                _selectedBezierPointIndex = index;
                _path.SetBezierPoint(index, new BezierPoint(position, inTangentPosition - position, outTangentPosition - position));
            }

            return position;
        }

        private void DrawLines()
        {
            Handles.color = Color.white;

            BezierPoint lineStart = _path.GetBezierPoint(0);
            for (int i = 1; i < _path.Count; i++)
            {
                BezierPoint lineEnd = _path.GetBezierPoint(i);

                Handles.DrawBezier(lineStart.position, lineEnd.position, lineStart.position + lineStart.outTangent, lineEnd.position + lineEnd.inTangent, Color.white, Texture2D.whiteTexture, 1.5f);
                lineStart = lineEnd;
            }

            BezierPoint first = _path.GetBezierPoint(0);
            BezierPoint last = _path.GetBezierPoint(_path.Count - 1);
        
            if(_path.closed)
                Handles.DrawBezier(last.position, first.position, last.position + last.outTangent, first.position + first.inTangent, Color.white, Texture2D.whiteTexture, 1.5f);
        }
    }

    [CustomPropertyDrawer(typeof(BezierPoint))]
    public class BezierPointDrawer : PropertyDrawer
    {
        private GUIContent BrokenTangentIcon => new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Faktori/Icons/TangentBrokenPro.png"), "Edit Path");
        private GUIContent ContinuousTangentIcon => new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Faktori/Icons/TangentContinuousPro.png"), "Edit Path");
        private GUIContent LinearTangentIcon => new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Faktori/Icons/TangentLinearPro.png"), "Edit Path");
    
        private int _selectedTangentMode = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(8, 8, 6, 6);
        
            SerializedProperty tangentMode = property.FindPropertyRelative("tangentMode");
            _selectedTangentMode = tangentMode.enumValueIndex;
        
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Tangent Mode");
            _selectedTangentMode = GUILayout.Toolbar(_selectedTangentMode, new GUIContent[] {BrokenTangentIcon, ContinuousTangentIcon, LinearTangentIcon}, buttonStyle, GUILayout.MinHeight(24));

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}