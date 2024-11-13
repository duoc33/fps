using UnityEngine;

// Enables a behaviour when a rigidbody settles movement
// otherwise disables the behaviour
public class EnableIffSleeping : MonoBehaviour
{
    public Behaviour m_Behaviour;
    Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    //在Unity的物理引擎中，当一个Rigidbody物体的速度非常低且几乎不再移动时，
    //m_Rigidbody物理引擎会将它置于睡眠状态，以节省性能。调用这个方法可以判断物体当前是否正在休眠。
    void Update()
    {
        if (m_Rigidbody == null || m_Behaviour == null)
            return;

        //休眠的时候，才开启Navmesh
        if (m_Rigidbody.IsSleeping() && !m_Behaviour.enabled)
            m_Behaviour.enabled = true;

        if (!m_Rigidbody.IsSleeping() && m_Behaviour.enabled)
            m_Behaviour.enabled = false;
    }
}
