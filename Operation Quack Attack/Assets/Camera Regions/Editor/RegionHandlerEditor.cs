/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using UnityEditor;
using UnityEngine;

/* The custom editor class for the RegionHandler class.
*/

[CustomEditor(typeof(RegionHandler))]
public class RegionHandlerEditor : Editor {
    RegionHandler handler;

    Transform handleTransform;
    Quaternion handleRotation;

    int selectedIndex = -1;

    Color regionFillColorUnselected = new Color(1, 1, 1, 0);

    void OnSceneGUI() {
        handler = (RegionHandler)target;
        Tools.current = Tool.None;

        handleTransform = handler.transform;
        handleRotation = (Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity);

        for (int i = 0; i < handler.Regions.Count; i++) {
            Vector2 p0 = ShowPoint(i, ref handler.Regions[i].p0);
            Vector2 p1 = ShowPoint(i, ref handler.Regions[i].p1);

            Vector3[] verts = {
                new Vector3(p0.x, p0.y),
                new Vector3(p1.x, p0.y),
                new Vector3(p1.x, p1.y),
                new Vector3(p0.x, p1.y)
            };

            Handles.color = handler.IsRegionValidated(i) ? Color.white : Color.red;
            Handles.DrawSolidRectangleWithOutline(verts, (selectedIndex == i ? handler.regionFillColor : regionFillColorUnselected), handler.regionOutlineColor);
        }
    }

    public override void OnInspectorGUI() {
        if (handler != null) {
            GUILayout.Label("Region Colors");
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            handler.regionOutlineColor = EditorGUILayout.ColorField("Outline", handler.regionOutlineColor);
            handler.regionFillColor = EditorGUILayout.ColorField("Fill", handler.regionFillColor);
            if (EditorGUI.EndChangeCheck()) {
                SceneView.RepaintAll();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Region", GUILayout.Height(25))) {
                Undo.RecordObject(target, "Add Region");

                Ray worldRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
                handler.AddRegion(new Vector2(worldRay.origin.x, worldRay.origin.y));
                selectedIndex = handler.Regions.Count - 1;
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginDisabledGroup(handler.Regions.Count > 0 ? false : true);
            if (GUILayout.Button("Validate Regions", GUILayout.Height(25))) {
                handler.Validate();
                SceneView.RepaintAll();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            if (selectedIndex >= 0 && selectedIndex < handler.Regions.Count) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField("Selected Region Index", selectedIndex);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();
                Vector2 p0 = EditorGUILayout.Vector2Field("Point 0", handler.Regions[selectedIndex].p0);
                Vector2 p1 = EditorGUILayout.Vector2Field("Point 1", handler.Regions[selectedIndex].p1);
                if (EditorGUI.EndChangeCheck()) {
                    handler.Regions[selectedIndex].p0 = p0;
                    handler.Regions[selectedIndex].p1 = p1;
                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Remove Region", GUILayout.Height(25))) {
                    Undo.RecordObject(target, "Remove Region");
                    handler.RemoveRegion(selectedIndex);
                    selectedIndex--;
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }

    Vector3 ShowPoint(int _index, ref Vector2 _point) {
        Vector2 point = handleTransform.TransformPoint(_point);
        float size = HandleUtility.GetHandleSize(point);

        Handles.color = (selectedIndex == _index ? Color.green : Color.grey);
        if (Handles.Button(point, handleRotation, 0.1f * size, 0.1f * size, Handles.DotHandleCap)) {
            selectedIndex = _index;
            Repaint();
        }

        if (selectedIndex == _index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(handler, "Move Point");
                EditorUtility.SetDirty(handler);

                _point = handleTransform.InverseTransformPoint(point);
            }
        }

        return point;
    }

}
