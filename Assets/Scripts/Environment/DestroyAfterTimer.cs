using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyAfterTimer : MonoBehaviour
{
    [SerializeField]
    private int waitMsBetweenDestroys = 20;
    [SerializeField]
    private int waitMsBeforeBeginDestroy = 100;
    [SerializeField]
    private int deleteByPackOf = 8;

    private float m_msBeforeDestroy = 0;
    private bool m_beginToDestroy = false;

    private int m_childToDestroy = 0;

    void Start()
    {
        m_childToDestroy = transform.childCount - 1;
        StartCoroutine(ExecuteAfterTime(waitMsBeforeBeginDestroy));
    }

    IEnumerator ExecuteAfterTime(int time)
    {
        yield return new WaitForSeconds(time / 1000.0f);
        m_beginToDestroy = true;
    }

    void Update()
    {
        if(m_beginToDestroy && transform.childCount > 0)
        {
            float deltaTime = Time.deltaTime;
            m_msBeforeDestroy += deltaTime;

            if ((m_msBeforeDestroy * 1000) > waitMsBetweenDestroys)
            {
                m_msBeforeDestroy = 0.0f;

                int childrenToDestroy = deleteByPackOf;
                while(childrenToDestroy > 0 && m_childToDestroy > 0)
                {
                    childrenToDestroy--;
                    DestroyOneChild();
                }

                if (m_childToDestroy - 1 <= 0)
                    Destroy(gameObject);
            }
        }
    }

    void DestroyOneChild()
    {
        Destroy(transform.GetChild(m_childToDestroy).gameObject);
        m_childToDestroy--;
    }
}
