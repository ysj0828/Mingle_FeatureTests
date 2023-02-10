using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhotonMoveTest : MonoBehaviour
{
    [SerializeField] NavMeshAgent NMA;

    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;

    private void OnEnable()
    {
        PointA = GameObject.Find("PointA").transform;
        PointB = GameObject.Find("PointB").transform;

        StartCoroutine("MoveToB");
    }

    IEnumerator MoveToA()
    {
        NMA.SetDestination(PointA.position);
        yield return new WaitForSeconds(4f);
        StartCoroutine("MoveToB");

        yield break;
    }

    IEnumerator MoveToB()
    {
        NMA.SetDestination(PointB.position);
        yield return new WaitForSeconds(4f);
        StartCoroutine("MoveToA");

        yield break;
    }
}
