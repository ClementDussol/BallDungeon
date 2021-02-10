using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.U2D.Path;
using UnityEngine;

namespace Faktori.Path
{
    [CustomEditor(typeof(PolygonPath))]
    public class PolygonPathEditor : Editor
    {
        private PolygonPath _path;
        private Transform _handleTransform;
        private Quaternion _handleRotation;

        private SelectionInfo _selectionInfo;

        private static Color _pointSelectedColor = new Color(0, 1f, 0.5f);
        private static Color _pointHoverColor = Color.white;
        private static Color _pointColor = Color.grey;
        private static Color _pointGhostColor = new Color(0, 1f, 0.5f);
        private static Color _pointDeleteColor = new Color(1, .33f, .5f);
        
        private static float _pointSelectedRadius = .1f;
        private static float _pointHoverRadius = .1f;
        private static float _pointRadius = .075f;
        private static float _pointGhostRadius = .05f;

        private static Texture2D _lineTexture;
        
        private void OnEnable()
        {
            _path = target as PolygonPath;
            _handleTransform = _path.transform;
            _selectionInfo = new SelectionInfo();
            _lineTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Faktori/Path/Editor/Icons/line-texture.png");
        }

        private void OnSceneGUI()
        {
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            else
            {
                HandleInput(Event.current);
            }
            
                        
            DrawLines();
            DrawHandles();
        }

        void DeletePointUnderMouse()
        {
            Undo.RecordObject(_path, "Delete Point");
            _path.RemoveAt(_selectionInfo.pointIndex);
            _selectionInfo.pointIsSelected = false;
            _selectionInfo.mouseIsOverPoint = false;
            _selectionInfo.pointIndex = -1;
        }

        private void DrawHandles()
        {
            for (int i = 0; i < _path.Count; i++)
            {
                DrawHandleForPointAtIndex(i);
            }
            
            if(_selectionInfo.mouseIsOverLine && !_selectionInfo.pointIsSelected)
            {
                Handles.color = _pointGhostColor;
                Handles.DrawSolidDisc(_selectionInfo.pointOnSelectedLine, -Vector3.forward, _pointGhostRadius);
            }
        }

        private void HandleInput(Event GUIevent)
        {
            Vector3 mousePosition = GUIevent.mousePosition;
            mousePosition = HandleUtility.GUIPointToWorldRay(GUIevent.mousePosition).origin;
            mousePosition.z = 0;

            if (GUIevent.type == EventType.MouseDown && GUIevent.button == 0)
            {
                if(GUIevent.modifiers == EventModifiers.None)
                {
                    HandleMouseDown(mousePosition);
                }
                else if (GUIevent.modifiers == EventModifiers.Shift)
                {
                    HandleShiftMouseDown(mousePosition);
                }
            } 
            
            if (GUIevent.type == EventType.MouseUp && GUIevent.button == 0 && GUIevent.modifiers == EventModifiers.None)
            {
                HandleMouseUp(mousePosition);
            }   
            
            if (GUIevent.type == EventType.MouseDrag && GUIevent.button == 0 && GUIevent.modifiers == EventModifiers.None )
            {
                HandleMouseDrag(mousePosition);
            }
            
            if (_selectionInfo.mouseIsOverLine)
            {
                _selectionInfo.pointOnSelectedLine = HandleUtility.ProjectPointLine(mousePosition, _path.GetPoint(_selectionInfo.lineIndex), _path.GetPoint((_selectionInfo.lineIndex + 1) % _path.Count));
            }
            
            if(!_selectionInfo.pointIsSelected)
                UpdateMouseOverSelection(mousePosition);
        }

        private void HandleMouseDown(Vector3 mousePosition)
        {
            if(!_selectionInfo.mouseIsOverPoint)
            {
                int index;
                Vector3 position;
                
                Undo.RecordObject(_path, "Add Point");
                EditorUtility.SetDirty(_path);

                if (_selectionInfo.mouseIsOverLine)
                {
                    index = _selectionInfo.lineIndex + 1;
                    position = HandleUtility.ProjectPointLine(mousePosition, _path.GetPoint(_selectionInfo.lineIndex), _path.GetPoint(index % _path.Count));
                }
                else
                {
                    index = _path.Count;
                    position = mousePosition;
                }
                
                _selectionInfo.offset = position - mousePosition;
                _path.InsertPoint(index, position);
                _path.OnValidate();
                _selectionInfo.pointIndex = index;
                _selectionInfo.mouseIsOverLine = false;
                _selectionInfo.lineIndex = -1;
            }
            
            _selectionInfo.pointIsSelected = true;
            _selectionInfo.positionAtDragStart = mousePosition;
        }

        private void HandleShiftMouseDown(Vector3 mousePosition)
        {
            if(_selectionInfo.mouseIsOverPoint)
            {
                DeletePointUnderMouse();
                _path.OnValidate();
            }
        }
        
        private void HandleMouseDrag(Vector3 mousePosition)
        {
            if (_selectionInfo.pointIsSelected)
            {
                SceneView.RepaintAll();
                _path.SetPoint(_selectionInfo.pointIndex, mousePosition + _selectionInfo.offset);
                _path.OnValidate();
            }
        }

        private void HandleMouseUp(Vector3 mousePosition)
        {
            if (_selectionInfo.pointIsSelected)
            {
                _path.SetPoint(_selectionInfo.pointIndex, _selectionInfo.positionAtDragStart);
                Undo.RecordObject(_path, "Move Point");
                _path.SetPoint(_selectionInfo.pointIndex, mousePosition + _selectionInfo.offset);
                
                EditorUtility.SetDirty(_path);
                
                _selectionInfo.pointIsSelected = false;
                _selectionInfo.pointIndex = -1;
                _selectionInfo.offset = Vector3.zero;
            }
        }
        
        private void DrawHandleForPointAtIndex(int index)
        {
            float size;
            if (_selectionInfo.pointIndex == index)
            {
                size = _selectionInfo.pointIsSelected ? _pointSelectedRadius : _pointHoverRadius;
                Handles.color = _selectionInfo.pointIsSelected 
                    ? _pointSelectedColor 
                    : Event.current.modifiers == EventModifiers.Shift 
                        ? _pointDeleteColor 
                        : _pointHoverColor;
            }
            else
            {
                size = _pointRadius;
                Handles.color = _pointColor;
            }

            Handles.DrawSolidDisc(_path.GetPoint(index), -Vector3.forward, size);
        }

        private void UpdateMouseOverSelection(Vector3 mousePosition)
        {
            //MOUSE OVER POINT
            int mouseOverPointIndex = -1;
            for (int i = 0; i < _path.Count; i++)
            {
                Vector3 point = _path.GetPoint(i);
                if (Vector3.Distance(point, mousePosition) < .25f)
                {
                    mouseOverPointIndex = i;
                    break;
                }
            }

            if (mouseOverPointIndex != _selectionInfo.pointIndex)
            {
                _selectionInfo.pointIndex = mouseOverPointIndex;
                _selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;
            }

            // MOUSE OVER LINE
            if (_selectionInfo.mouseIsOverPoint)
            {
                _selectionInfo.mouseIsOverLine = false;
                _selectionInfo.lineIndex = -1;
            }
            else
            {
                int mouseOverLineIndex = -1;
                float closestLineDistance = .5f;
                for (int i = 0; i < (_path.closed ? _path.Count : _path.Count - 1); i++)
                {
                    Vector3 point = _path.GetPoint(i);
                    Vector3 next = _path.GetPoint((i + 1) % _path.Count);

                    float distanceFromLine = HandleUtility.DistancePointToLineSegment(mousePosition, point, next);
                    if (distanceFromLine < .25f)
                    {
                        closestLineDistance = distanceFromLine;
                        mouseOverLineIndex = i;
                    }
                }

                if (_selectionInfo.lineIndex != mouseOverLineIndex)
                {
                    _selectionInfo.lineIndex = mouseOverLineIndex;
                    _selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                }
            }
        }
        private void DrawLines()
        {
            for (int i = 0; i < (_path.closed ? _path.Count : _path.Count - 1); i++)
            {
                Handles.color = i == _selectionInfo.lineIndex ? Color.white : Color.gray;
                Handles.DrawLine(_path.GetPoint(i), _path.GetPoint((i + 1) % _path.Count));
            }
        }

        public class SelectionInfo
        {
            public int pointIndex = -1;
            public bool mouseIsOverPoint = false;
            public bool pointIsSelected = false;
            public Vector3 positionAtDragStart;
            public Vector3 offset;
            public int lineIndex = -1;
            public bool mouseIsOverLine = false;
            public Vector3 pointOnSelectedLine;
        }
    }
}
