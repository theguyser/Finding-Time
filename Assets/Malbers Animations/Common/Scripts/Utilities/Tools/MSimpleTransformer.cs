﻿using MalbersAnimations.Scriptables;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Based on 3DKit Controller from Unity
    /// </summary>

    public abstract class MSimpleTransformer : MonoBehaviour
    {
        [Tooltip("This is the object to move. Must be child of this gameobject")]
        [RequiredField] public Transform Object;

        public LoopType loopType;

        public FloatReference StartDelay = new FloatReference();
        public FloatReference EndDelay = new FloatReference();
        public FloatReference duration = new FloatReference(1);
        public AnimationCurve m_Curve = new AnimationCurve(MTools.DefaultCurve);



        [Header("Events")]
        [FormerlySerializedAs("OnReachStart")]
        public UnityEvent OnStart = new UnityEvent();
        [FormerlySerializedAs("OnReachEnd")]
        public UnityEvent OnEnd = new UnityEvent();

        public UnityEvent onEnable = new UnityEvent();
        public UnityEvent onDisable = new UnityEvent();


        [Range(0, 1)]
        public float previewPosition;

        private float time = 0f;
        private float position = 0f;
        private float lasPos = 0f;
        private bool Waiting = false;

        private WaitForSeconds StartWaitSeconds;
        private WaitForSeconds EndWaitSeconds;


        protected virtual void OnEnable()
        {
            Restart();
            SetStartWait(StartDelay);
            SetEndWait(EndDelay);
            onEnable?.Invoke();
            DoWaitStart();
        }

        protected virtual void OnDisable()
        {
            onDisable?.Invoke();
        }

        private void SetStartWait(float delay) => StartWaitSeconds = new WaitForSeconds(delay);
        private void SetEndWait(float delay) => EndWaitSeconds = new WaitForSeconds(delay);

        protected virtual void Restart()
        {
            Waiting = false;
            time = 0f;
            position = 0f;
            lasPos = 0f;
            forward = true;
            Evaluate(position);
            StopAllCoroutines();
        }

        private IEnumerator C_WaitStart()
        {
            if (StartDelay > 0)
            {
                Waiting = true;
                yield return StartWaitSeconds;    
            }

            OnStart.Invoke();
            Waiting = false;
        }

        private IEnumerator C_WaitEnd()
        {
            OnEnd.Invoke();

            if (EndDelay > 0)
            {
                Waiting = true;
                yield return EndWaitSeconds;
            }

            if (loopType == LoopType.PingPong)
            {
                OnStart.Invoke();
            }

            Waiting = false;

            if (loopType == LoopType.Once)
            {
                enabled = false;
            }
        }


        private IEnumerator C_WaitRepeat()
        {
            OnEnd.Invoke();

            if (EndDelay > 0)
            {
                Waiting = true;
                yield return EndWaitSeconds;
            }
            Waiting = false;
            position = 0;
            Evaluate(position);


            if (StartDelay > 0)
            {
                Waiting = true;
                yield return StartWaitSeconds;
            }
            OnStart.Invoke();
            yield return null;

            Waiting = false;
        }


        public void Activate()
        {
            if (enabled)
            {
                OnEnable(); //Meaning it has not finished the last animation so start over
            }
            else
            {
                enabled = true;
            }
        }

        private void FixedUpdate()
        {
            if (!Waiting)
            {
                time += Time.fixedDeltaTime / duration;
                switch (loopType)
                {
                    case LoopType.Once:
                        LoopOnce();
                        break;
                    case LoopType.PingPong:
                        LoopPingPong();
                        break;
                    case LoopType.Repeat:
                        LoopRepeat();
                        break;
                }
                Evaluate(position);
            }
        }


        public abstract void Evaluate(float curveValue);

        void LoopPingPong()
        {
            lasPos = position;

            position = Mathf.PingPong(time, 1f);


            if (forward && lasPos > position)
            {
                OnEnd?.Invoke();
                forward ^= true;
                DoWaitEnd();

            }
            else if (!forward && lasPos < position)
            {
                OnEnd?.Invoke();
                forward ^= true;
                DoWaitStart();

            }
        }

        bool forward;

        void LoopRepeat()
        {
            lasPos = position;
            position = Mathf.Repeat(time, 1f);
            if (lasPos > position)
            {
                position = 1;
                WaitRepeat();
            }
        }

        void DoWaitEnd() => StartCoroutine(C_WaitEnd());
        void DoWaitStart() => StartCoroutine(C_WaitStart());
        void WaitRepeat() => StartCoroutine(C_WaitRepeat());

        void LoopOnce()
        {
            position = Mathf.Clamp01(time);
            if (position >= 1)
            {
                DoWaitEnd();
            }
        }


        protected virtual void Reset()
        {
            if (transform.childCount > 0)
            { Object = transform.GetChild(0); }
            else
            {
                Object = transform;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MSimpleTransformer), true)]
    public class MSimpleTransformerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (cc.changed)
                {
                    var pt = target as MSimpleTransformer;
                    pt.Evaluate(pt.previewPosition);
                }
            }
        }
    }
#endif
}