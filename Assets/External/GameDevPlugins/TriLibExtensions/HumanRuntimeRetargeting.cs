using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanRuntimeRetargeting : MonoBehaviour
{
    public GameObject src;
    HumanPoseHandler m_srcPoseHandler;
    HumanPoseHandler m_destPoseHandler;
    private Animator animator;
    void Start()
    {
        m_srcPoseHandler = new HumanPoseHandler(src.GetComponent<Animator>().avatar, src.transform);
        m_destPoseHandler = new HumanPoseHandler(GetComponent<Animator>().avatar, transform);
    }

    void LateUpdate()
    {
        HumanPose m_humanPose = new HumanPose();

        m_srcPoseHandler.GetHumanPose(ref m_humanPose);
        m_destPoseHandler.SetHumanPose(ref m_humanPose);
    }
}