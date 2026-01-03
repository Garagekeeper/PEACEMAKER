#if UNITY_EDITOR
using Resources.Script.Audio;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Resources.Script.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(AudioPreset))]
    public class AudioPresetEditor : Editor
    {
        private static bool showBasic;
        private static bool show3D;
        private static bool showDynamicPitch;
        private static bool show6D;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AudioPreset preset = (AudioPreset)target;

            Undo.RecordObject(preset, $"Modified {preset}");

            EditorGUI.BeginChangeCheck();

            // Audio Source

            preset.audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", preset.audioClip, typeof(AudioClip), true);
            preset.output = (AudioMixerGroup)EditorGUILayout.ObjectField("Output", preset.output, typeof(AudioMixerGroup), true);

            GUILayout.Space(8);

            EditorGUI.indentLevel++;

            // BASIC SETTINGS

            EditorGUILayout.BeginVertical("box");

            showBasic = EditorGUILayout.Foldout(showBasic, "Basic Audio Settings", true);

            EditorGUILayout.EndVertical();

            if (showBasic)
            {
                EditorGUILayout.BeginVertical("box");

                preset.mute = EditorGUILayout.ToggleLeft("Mute", preset.mute);
                preset.bypassEffects = EditorGUILayout.ToggleLeft("Bypass Effects", preset.bypassEffects);
                preset.bypassListenerEffects = EditorGUILayout.ToggleLeft("Bypass Listener Effects", preset.bypassListenerEffects);
                preset.bypassReverbZones = EditorGUILayout.ToggleLeft("Bypass Reverb Zones", preset.bypassReverbZones);
                preset.playOnAwake = EditorGUILayout.ToggleLeft("Play On Awake", preset.playOnAwake);
                preset.loop = EditorGUILayout.ToggleLeft("Loop", preset.loop);

                GUILayout.Space(5);

                preset.priority = EditorGUILayout.IntSlider("Priority", preset.priority, 0, 256);
                preset.volume = EditorGUILayout.Slider("Volume", preset.volume, 0, 1);
                preset.pitch = EditorGUILayout.Slider("Pitch", preset.pitch, -3, 3);
                preset.stereoPan = EditorGUILayout.Slider("Stereo Pan", preset.stereoPan, -1, 3);
                preset.spatialBlend = EditorGUILayout.Slider("Spatial Blend", preset.spatialBlend, 0, 1);
                preset.reverbZoneMix = EditorGUILayout.Slider("Reverb Zone Mix", preset.reverbZoneMix, 0, 1.1f);

                EditorGUILayout.EndVertical();
            }

            // DYNAMIC PITCH SETTINGS

            EditorGUILayout.BeginVertical("box");

            showDynamicPitch = EditorGUILayout.Foldout(showDynamicPitch, "Dynamic Pitch Settings", true);

            EditorGUILayout.EndVertical();

            if (showDynamicPitch)
            {
                EditorGUILayout.BeginVertical("box");

                preset.syncPitchWithTimeScale = EditorGUILayout.ToggleLeft("Sync Pitch With Time Scale", preset.syncPitchWithTimeScale);
                preset.useRandomPitchOffset = EditorGUILayout.ToggleLeft("Dynamic Pitch Enabled", preset.useRandomPitchOffset);
                preset.randomPitchOffset = EditorGUILayout.Slider("pitchOffset", preset.randomPitchOffset, -3, 3);

                EditorGUILayout.EndVertical();
            }

            // 3D SOUND SETTINGS
            EditorGUILayout.BeginVertical("box");


            show3D = EditorGUILayout.Foldout(show3D, "3D Sound Settings", true);

            EditorGUILayout.EndVertical();

            if (show3D)
            {
                EditorGUILayout.BeginVertical("box");

                preset.dopplerLevel = EditorGUILayout.Slider("Doppler Level", preset.dopplerLevel, 0, 5);
                preset.spread = EditorGUILayout.Slider("Spread", preset.spread, 0, 360);
                preset.maxDistance = EditorGUILayout.FloatField("Max Distance", preset.maxDistance);
                preset.minDistance = EditorGUILayout.FloatField("Min Distance", preset.minDistance);
                preset.simulateAcousticLatency = EditorGUILayout.Toggle("Simulate Acoustic Latency", preset.simulateAcousticLatency);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical("box");

            // 6D SOUND SETTINGS
            show6D = EditorGUILayout.Foldout(show6D, "6D Sound Settings", true);

            EditorGUILayout.EndVertical();


            if (show6D)
            {
                EditorGUILayout.BeginVertical("box");

                preset.forwardFactor = EditorGUILayout.Slider("Forward Factor", preset.forwardFactor, -1, 1);
                preset.backwardFactor = EditorGUILayout.Slider("Backward Factor", preset.backwardFactor, -1, 1);
                preset.rightFactor = EditorGUILayout.Slider("Right Factor", preset.rightFactor, -1, 1);
                preset.leftFactor = EditorGUILayout.Slider("Left Factor", preset.leftFactor, -1, 1);
                preset.upFactor = EditorGUILayout.Slider("Above Factor", preset.upFactor, -1, 1);
                preset.downFactor = EditorGUILayout.Slider("Below Factor", preset.downFactor, -2, 2);
                preset._6DSoundCurve = EditorGUILayout.CurveField("6D Sound Curve", preset._6DSoundCurve);

                EditorGUILayout.EndVertical();
            }




            EditorGUI.indentLevel--;

            if (preset.spread == 0)
            {
                EditorGUILayout.HelpBox("6D Sounds require 'Spread' to be greater than 1. 6D sounds won't be used.", MessageType.Info);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Event Timeline changes during playmode will not apply until next time entering play mode.", MessageType.Warning);
            }

            GUILayout.Space(10);

            DrawTimeline();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(preset);
        }


        #region Timeline
        private const float TimelineHeight = 25;
        private const float MarkerSize = 8f;
        private const float TickHeightMajor = 10f;
        private const float TickHeightMinor = 5f;
        private const float TimeLabelHeight = 14f;

        private readonly Color _timelineColor = new Color(0.3f, 0.3f, 0.3f);
        private readonly Color _markerColor = Color.gray;
        private readonly Color _selectedColor = Color.white;

        private int _selectedMarkerIndex = -1;

        [SerializeField] private float cursorTime = 0f;
        private bool _isDragging;

        private void DrawTimeline()
        {
            AudioPreset track = (AudioPreset)target;
            serializedObject.Update();

            track.audioLayersDuration = Mathf.Clamp(track.audioLayersDuration, 0, float.MaxValue);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Event Timeline", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            float buttonSize = TimelineHeight;
            float spacing = 5f;

            Rect fullRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(TimelineHeight));

            Rect buttonRect = new Rect(fullRect.x, fullRect.y, buttonSize, buttonSize);
            Rect timelineRect = new Rect(buttonRect.xMax + spacing, fullRect.y, fullRect.width - buttonSize - spacing, TimelineHeight);

            EditorGUI.BeginDisabledGroup(track.audioLayersDuration == 0);

            if (GUI.Button(buttonRect, "+"))
            {
                track.audioLayers.Add(new AudioPreset.CustomAudioLayer { time = cursorTime });
                EditorUtility.SetDirty(track);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.DrawRect(timelineRect, _timelineColor);
            DrawTimeLabels(timelineRect, track.audioLayersDuration);
            DrawMarkers(timelineRect, track);
            DrawTimeCursor(timelineRect, track);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(14);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Duration:", GUILayout.Width(60));
            track.audioLayersDuration = EditorGUILayout.FloatField(track.audioLayersDuration, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            // Keyframe Inspector Panel
            if (track.audioLayersDuration > 0f && _selectedMarkerIndex >= 0 && _selectedMarkerIndex < track.audioLayers.Count)
            {
                AudioPreset.CustomAudioLayer selectedEvent = track.audioLayers[_selectedMarkerIndex];

                EditorGUILayout.LabelField("Keyframe", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                // Audio Clip Field
                selectedEvent.audioClip = (AudioClip)EditorGUILayout.ObjectField(
                    "Audio Clip",
                    selectedEvent.audioClip,
                    typeof(AudioClip),
                    false
                );

                // Time Slider
                selectedEvent.time = EditorGUILayout.Slider("Time", selectedEvent.time, 0f, Mathf.Max(0.01f, track.audioLayersDuration));

                if (EditorGUI.EndChangeCheck())
                {
                    selectedEvent.time = Mathf.Clamp(selectedEvent.time, 0f, track.audioLayersDuration);
                    EditorUtility.SetDirty(track);
                }
            }
            else if (track.audioLayersDuration > 0f && track.audioLayers.Count > 0)
            {
                EditorGUILayout.HelpBox("No keyframe selected. Click a keyframe on the timeline to view and edit its properties.", MessageType.Info);
            }

            if (track.audioLayersDuration > 0f && track.audioLayers.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "There are no keyframes. To add one, drag the red playhead to a position on the timeline, then click the '+' button.",
                    MessageType.Info
                );
            }


            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeLabels(Rect rect, float duration)
        {
            if (duration <= 0f || rect.width <= 0f)
            {
                GUIStyle centered = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                GUI.Label(rect, "Set duration > 0 to view timeline", centered);
                return;
            }

            Handles.color = Color.gray;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.UpperCenter
            };

            float totalMilliseconds = duration * 1000f;
            float minLabelSpacing = 40f;
            float pixelsPerSecond = rect.width / duration;
            if (pixelsPerSecond <= 0f) return;

            int secondsStep = Mathf.CeilToInt(minLabelSpacing / pixelsPerSecond);

            for (int ms = 0; ms <= totalMilliseconds; ms += 100)
            {
                float t = ms / 1000f;
                float normalized = Mathf.Clamp01(t / duration);
                float x = Mathf.Lerp(rect.x, rect.xMax, normalized);

                bool isMajor = ms % 1000 == 0;
                float tickHeight = isMajor ? TickHeightMajor : TickHeightMinor;

                Handles.DrawLine(
                    new Vector3(x, rect.yMax - tickHeight),
                    new Vector3(x, rect.yMax)
                );

                if (isMajor && ((int)t % secondsStep == 0))
                {
                    GUI.Label(new Rect(x - 15, rect.yMax, 30, TimeLabelHeight), $"{t:0.#}s", labelStyle);
                }
            }
        }
        private void DrawMarkers(Rect rect, AudioPreset track)
        {
            Event evt = Event.current;
            float yCenter = rect.y + rect.height / 2f;

            for (int i = 0; i < track.audioLayers.Count; i++)
            {
                var e = track.audioLayers[i];
                float normalizedTime = Mathf.Clamp01(e.time / track.audioLayersDuration);
                float x = rect.x + normalizedTime * rect.width;

                Vector2 center = new Vector2(x, yCenter);
                float halfSize = MarkerSize / 2f;

                Vector3[] diamondPoints = new Vector3[4];
                diamondPoints[0] = new Vector3(center.x, center.y - halfSize);
                diamondPoints[1] = new Vector3(center.x + halfSize, center.y);
                diamondPoints[2] = new Vector3(center.x, center.y + halfSize);
                diamondPoints[3] = new Vector3(center.x - halfSize, center.y);

                Color fill = (_selectedMarkerIndex == i) ? _selectedColor : _markerColor;
                Handles.DrawSolidRectangleWithOutline(diamondPoints, fill, Color.black);

                Rect clickRect = new Rect(center.x - 8, center.y - 8, 16, 16);

                if (evt.type == EventType.MouseDown && evt.button == 0 && clickRect.Contains(evt.mousePosition))
                {
                    _selectedMarkerIndex = i;
                    _isDragging = true;
                    evt.Use();
                }

                if (evt.type == EventType.ContextClick && clickRect.Contains(evt.mousePosition))
                {
                    int index = i;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        track.audioLayers.RemoveAt(index);
                        _selectedMarkerIndex = -1;
                        EditorUtility.SetDirty(track);
                    });
                    menu.ShowAsContext();
                    evt.Use();
                }
            }

            if (_isDragging && _selectedMarkerIndex >= 0 && evt.type == EventType.MouseDrag)
            {
                float newNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                float newTime = Mathf.Clamp01(newNormalized) * track.audioLayersDuration;

                track.audioLayers[_selectedMarkerIndex].time = newTime;
                EditorUtility.SetDirty(track);
                evt.Use();
            }

            if (evt.type == EventType.MouseUp && _isDragging)
            {
                _isDragging = false;
                evt.Use();
            }
        }


        private void DrawTimeCursor(Rect rect, AudioPreset track)
        {
            Event evt = Event.current;

            float normalized = track.audioLayersDuration > 0 ? Mathf.Clamp01(cursorTime / track.audioLayersDuration) : 0f;
            float x = rect.x + normalized * rect.width;

            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(x, rect.y), new Vector3(x, rect.y + rect.height));

            if (evt.type == EventType.MouseDown && evt.button == 0 && rect.Contains(evt.mousePosition))
            {
                float clickedNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                cursorTime = Mathf.Clamp(clickedNormalized * track.audioLayersDuration, 0f, track.audioLayersDuration);
                evt.Use();
            }

            if (evt.type == EventType.MouseDrag && rect.Contains(evt.mousePosition))
            {
                float draggedNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                cursorTime = Mathf.Clamp(draggedNormalized * track.audioLayersDuration, 0f, track.audioLayersDuration);
                evt.Use();
            }
        }
        #endregion
    }
}

#endif