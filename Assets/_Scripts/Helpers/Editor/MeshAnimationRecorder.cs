using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Misc
{
    public class MeshAnimationRecorder : MonoBehaviour
    {
        public AnimationClip clip;
        public bool record = false;

        private GameObjectRecorder _recorder;

        private void Start()
        {
            _recorder = new GameObjectRecorder(gameObject);
            _recorder.BindComponentsOfType<Transform>(gameObject, true);
        }

        private void LateUpdate()
        {
            if (clip == null) return;

            if (record)
            {
                _recorder.TakeSnapshot(Time.deltaTime);
            }
            else if (_recorder.isRecording)
            {
                _recorder.SaveToClip(clip);
                _recorder.ResetRecording();
            }
        }

        private void OnDisable()
        {
            if(clip == null) return;

            if (_recorder.isRecording)
            {
                _recorder.SaveToClip(clip);
            }
        }
    }
}