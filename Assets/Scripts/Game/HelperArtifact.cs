using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperArtifact : MonoBehaviour
{
    [SerializeField]
    private GameObject firstPoint;
    [SerializeField]
    private GameObject secondPoint;
    [SerializeField]
    private GameObject lastPoint;
    
    private Stack<Vector3> m_path = new Stack<Vector3>();
    public void Start()
    {
        m_path.Push(lastPoint.transform.position);
        m_path.Push(secondPoint.transform.position);
        m_path.Push(firstPoint.transform.position);
    }

    public void ToNextPoint()
    {
        if (m_path.Count <= 0)
            Destroy(gameObject);
        else
            gameObject.transform.position = m_path.Pop();
    }
}
