using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestGame : MonoBehaviour
{
    public HarvestGameState gameState;

    private void Update() {
        if(gameState)
            gameState.gameUpdate();    
    }
}
