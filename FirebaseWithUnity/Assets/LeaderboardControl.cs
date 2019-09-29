using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeaderboardControl : MonoBehaviour {
    public SavePerson savePerson;
    
    private void OnEnable() {
        savePerson.OrderUsersByParam(0);
    }
}
