using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndFollow : MonoBehaviour {

    [SerializeField] private float m_Damping = 0.2f;        // Used to smooth the rotation of the transform.
    [SerializeField] private float m_MaxYRotation = 20f;    // The maximum amount the transform can rotate around the y axis.
    [SerializeField] private float m_MinYRotation = -20f;   // The maximum amount the transform can rotate around the y axis in the opposite direction.
    [SerializeField] private GameObject m_targetToFollow = null;
    [SerializeField] private float m_desiredDistanceX = 2.0f;
    [SerializeField] private float m_desiredDistanceY = 0.0f;

    private Vector3 m_desiredPosition = Vector3.zero;
    private float m_hoverHeight = 1.69f;
    private const float k_ExpDampCoef = -20f;               // Coefficient used to damp the rotation.

    private void Start()
    {
        m_desiredPosition = m_targetToFollow.transform.position + (m_targetToFollow.transform.forward * 5);
        transform.position = m_desiredPosition;
    }

    private void Update()
    {
        // Store the Euler rotation of the gameobject.
        var eulerRotation = transform.rotation.eulerAngles;

        // Set the rotation to be the same as the user's in the y axis.
        eulerRotation.x = 0;
        eulerRotation.z = 0;
        eulerRotation.y = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head).eulerAngles.y;

        // Add 360 to the rotation so that it can effectively be clamped.
        if (eulerRotation.y < 270)
            eulerRotation.y += 360;

        // Clamp the rotation between the minimum and maximum.
        eulerRotation.y = Mathf.Clamp(eulerRotation.y, 360 + m_MinYRotation, 360 + m_MaxYRotation);

        // Smoothly damp the rotation towards the newly calculated rotation.
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerRotation),
            m_Damping * (1 - Mathf.Exp(k_ExpDampCoef * Time.deltaTime)));
        transform.LookAt(m_targetToFollow.transform.position, Vector3.up);
        transform.Rotate(new Vector3(30, 180, 0));

        m_desiredPosition = m_targetToFollow.transform.position + (m_targetToFollow.transform.forward * m_desiredDistanceX) + (Vector3.up * m_desiredDistanceY);
        if (m_desiredPosition != transform.position)
        {
            //sprint("lerping position");
            Vector3 targetpos = new Vector3(m_desiredPosition.x, m_hoverHeight, m_desiredPosition.z);
            transform.position = Vector3.Lerp(transform.position, targetpos, m_Damping * (1 - Mathf.Exp(k_ExpDampCoef * Time.deltaTime)));
        }
    }
}
