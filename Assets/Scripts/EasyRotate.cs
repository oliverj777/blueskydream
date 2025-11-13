using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OllieJones
{
    public class EasyRotate : MonoBehaviour
    {
        [Header("Rotation")]
        public float rotationSpeed = 50f;

        [Header("Scale Pulse")]
        public float minScale = 0.8f;
        public float maxScale = 1.2f;
        public float pulseSpeed = 2f;

        private RectTransform rect;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            // Rotation
            rect.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            // Pulsating scale
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // 0..1
            float scale = Mathf.Lerp(minScale, maxScale, t);

            rect.localScale = new Vector3(scale, scale, 1f);
        }
    }

}

