using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCScript : MonoBehaviour
{
    NavMeshAgent agentSelf;
    Animator anim;

    string[] animatorParam = new string[] {"isIdle1", "isIdle2" , "isIdle3" , "isInteraction", "isAngry", "isBlushing", "isCool", "isCrying", "isCussing", "isHeart", "isLaughing", "isSilly", "isSleeping", "isUWU"};

    string[] prevAnim = new string[5];

    public GameObject[] listOfNPCs;

    NPCWalkScript npcScript;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Randomise", 0, 5);

        //StartCoroutine(RandomFunction());
    }



    public void Randomise()
    {
        int i = Random.Range(0, animatorParam.Length);

        for (int j = 0; j < listOfNPCs.Length; j++)
        {
            agentSelf = listOfNPCs[j].transform.GetComponent<NavMeshAgent>();
            anim = listOfNPCs[j].transform.GetComponent<Animator>();
            npcScript = listOfNPCs[j].gameObject.GetComponent<NPCWalkScript>();


            anim.SetBool(prevAnim[j], false);

            

            //agentSelf.destination = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));

            

            npcScript.currentAnimation = animatorParam[i];
            prevAnim[j] = animatorParam[i];
        }
    }

    //public void RandomFunction()
    //{
    //    Randomise();

    //    yield return new WaitForSeconds(7);
    //    StartCoroutine(RandomFunction());
    //}
}
