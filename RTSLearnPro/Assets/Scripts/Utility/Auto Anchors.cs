using UnityEngine;
using UnityEditor;

using System.Linq;
using System.Reflection;

namespace CustomFun
{
    [CustomEditor(typeof(RectTransform)), CanEditMultipleObjects]
    public sealed class RectTransformInspector : Editor
    {
        private Editor instance;
        private MethodInfo onSceneGUI;

        private static readonly object[] emptyArray = new object[0];

        private void OnEnable()
        {
            //��ȡ�༭������
            var editorType = Assembly.GetAssembly(typeof(Editor)).GetTypes().FirstOrDefault(m => m.Name == "RectTransformEditor");
            //�����༭����ʵ��
            instance = CreateEditor(targets, editorType);
            //ͨ�������ȡOnSceneGUI����
            onSceneGUI = editorType.GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public override void OnInspectorGUI()
        {
            if (instance)
            {
                instance.OnInspectorGUI();
            }
            GUILayout.Space(20f);

            if (GUILayout.Button("Auto Anchors"))
            {
                //��������ѡ�е�RectTransform
                for (int i = 0; i < targets.Length; i++)
                {
                    RectTransform tempTarget = targets[i] as RectTransform;
                    Undo.RecordObject(tempTarget, "Auto Anchors");
                    RectTransform prt = tempTarget.parent as RectTransform;
                    Vector2 anchorMin = new Vector2(
                        tempTarget.anchorMin.x + tempTarget.offsetMin.x / prt.rect.width,
                        tempTarget.anchorMin.y + tempTarget.offsetMin.y / prt.rect.height);
                    Vector2 anchorMax = new Vector2(
                        tempTarget.anchorMax.x + tempTarget.offsetMax.x / prt.rect.width,
                        tempTarget.anchorMax.y + tempTarget.offsetMax.y / prt.rect.height);
                    tempTarget.anchorMin = anchorMin;
                    tempTarget.anchorMax = anchorMax;
                    tempTarget.offsetMin = tempTarget.offsetMax = Vector2.zero;
                }
            }
        }

        private void OnSceneGUI()
        {
            if (instance)
            {
                onSceneGUI.Invoke(instance, emptyArray);
            }
        }
        private void OnDisable()
        {
            if (instance)
            {
                DestroyImmediate(instance);
            }
        }

    }
}