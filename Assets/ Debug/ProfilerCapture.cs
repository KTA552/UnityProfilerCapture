using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.Profiling;
using System;

namespace kt
{
    /// <summary>
    /// Profilerの情報を取得するウィンドウ
    /// </summary>
    public class ProfilerCapture : EditorWindow
    {

        [MenuItem("Tools/kt/ProfilerCapture")]
        private static void ShowWindow()
        {
            var window = GetWindow<ProfilerCapture>();
            window.titleContent = new GUIContent("ProfilerCapture");
            window.Show();
        }

        string _logText = string.Empty;

        int _captureIndex = 0;

        Vector2 _textScrollPos = Vector2.zero;

        /// <summary>
        /// UI表示
        /// </summary>
        private void OnGUI()
        {
            var firstIndex = ProfilerDriver.firstFrameIndex;
            var lastIndex = ProfilerDriver.lastFrameIndex;

            using (new EditorGUILayout.VerticalScope())
            {
                // Profilerから取得するフレームを選択
                _captureIndex = EditorGUILayout.IntSlider("CaptureFrame", _captureIndex, firstIndex, lastIndex);

                if (GUILayout.Button("Capture"))
                {
                    // ProfilerのRoot情報を取得
                    var property = new ProfilerProperty();

                    property.SetRoot(_captureIndex - 1, ProfilerColumn.SelfTime, ProfilerViewType.Hierarchy);

                    if (property.HasChildren)
                    {
                        _logText = string.Empty;
                        _logText += "Name, SelfTime, SelfPercent \n";
                        _logText += "---\n";
                    }

                    // ネストする子供がいなくなるまで回す
                    while (property.Next(true))
                    {
                        var value = property.GetColumnAsSingle(ProfilerColumn.SelfTime);

                        // 根こそぎ取れるので適当なしきい値で切る
                        if (value > 0.01)
                        {
                            _logText += String.Format("{0}, {1}, {2}. \n",
                                            property.GetColumn(ProfilerColumn.FunctionName),
                                            property.GetColumn(ProfilerColumn.SelfTime),
                                            property.GetColumn(ProfilerColumn.SelfPercent));

                        }
                    }

                    _logText += "---\n";
                }


                // 結果表示
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_textScrollPos))
                {
                    _textScrollPos = scrollView.scrollPosition;

                    EditorGUILayout.TextArea(_logText);
                }
            }

        }
    }
}


