using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Class
public class Player {

    public Player(int order, GameObject player, bool isDied) {
        this.order = order;
        this.player = player;
        this.isDied = isDied;
    }
    
    public int order;
    public GameObject player;
    public bool isDied;
    
}
