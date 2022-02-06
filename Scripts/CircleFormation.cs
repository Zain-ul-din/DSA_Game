using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using UnityEngine;
using CustomContainer;


// Circle Formation Code
public class CircleFormation : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private int count;
    [SerializeField] private float offSet;
    [SerializeField]private bool lookAtCenter = true;
    [SerializeField] private Transform circleRef;
    [SerializeField] private float circleCenterOffSet = 0.5f;

    
    [Space(10)] [Header(" Duration to kill enemy")] [SerializeField]
    private float killduration ;
    
    private Vector3 pos;
    public Vector3 center;
    private List<GameObject> list;

    private GameObject centerObj;

    private CircularLinkedList<GameObject> soliderList;
    private int order = 0;

    private bool isKiller = true;

    public static CircleFormation instance;

    public event EventHandler<GameObject> OnGameEnd;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        list = new List<GameObject>();
        pos = circleRef.position;
        soliderList = new CircularLinkedList<GameObject>();

        // Draws circle
        for (int i = 0; i < count; i += 1) {
            float angle = i * (2 * 3.14159f / count);
            float x = Mathf.Cos(angle) * offSet;
            float z = Mathf.Sin(angle) * offSet;
            pos = new Vector3(pos.x + x, .04f, pos.z + z);
            var obj = Instantiate(gameObject,pos , Quaternion.identity) as GameObject;
           // obj.transform.position = pos;
            list.Add(obj);
            obj.name = obj.name + i;
        }

        center = new Vector3(list[0].transform.position.x , 0f,   list[0].transform.position.z +circleCenterOffSet);
        centerObj = Instantiate(new GameObject("__CricleCenter__"), center, Quaternion.identity ) as GameObject;

        
        
        // makes solider look at each other
        if(lookAtCenter)
            foreach (var obj in list)
                obj.transform.LookAt(centerObj.transform.position);
        
        // Add soliders to Actual Circular Linked List
        foreach (var obj in list) {
            soliderList.Push_Back(obj);
        }
        
        

        // Make first player killer in list
        soliderList.ForEach(data => {
            if (isKiller) data.GetComponent<Solider>().target = false;
            else data.GetComponent<Solider>().target = true;
            isKiller = false;
        });
        
        // Set Enemies
        soliderList.For_Each(node => {
            node.data.GetComponent<Solider>().enemy = node.next.data.GetComponent<Solider>().transform.position;
        });
        

        soliderList.OnRemove += OnRemove;

        StartCoroutine(CheckList(killduration));
    }

    private void OnRemove(ref Node<GameObject> obj ) {
        Destroy(obj.data.GetComponent<Solider>(), .5f);
    }
    
    
    private IEnumerator CheckList(float time) {
        while (true && soliderList.GetCount() > 1) {
            yield return new WaitForSeconds(time);
          //  Debug.Log(soliderList.GetCount());

            
            // Remove Died Player From the List
            soliderList.For_Each(node => {
                if (node.data.GetComponent<Solider>().isDie) {
                    // Sets the next and prev
                    node.prev.data.GetComponent<Solider>().SetTargetState(true);
                    node.next.data.GetComponent<Solider>().SetTargetState(false);
                    soliderList.Remove(node.data);
                }
            });
            
            // Re check Killer in list
           /* soliderList.ForEach(data => {
                if (isKiller)
                    data.GetComponent<Solider>().target = false;
                isKiller = false;
            });*/
            
           // Reset Set Enemies
           soliderList.For_Each(node => {
               node.data.GetComponent<Solider>().enemy = node.next.data.GetComponent<Solider>().transform.position;
           });
        }
        
        StopAllCoroutines();
        OnGameEnd?.Invoke(this,soliderList.head.data); // send last node
    }
    
}
