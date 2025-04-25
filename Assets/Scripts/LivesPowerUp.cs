using UnityEngine;

public class LivesPowerUp : MonoBehaviour {

    public int ComputeLives(int amountOfLives)
    {
        switch(amountOfLives)
        {
            case 0:
                return amountOfLives + 1;  
            case 1:
                amountOfLives += 2;
                Debug.Log($"You have {amountOfLives} lives");
                return amountOfLives;
            case 2:
                return amountOfLives + 1;
            case 3:
                return amountOfLives;
            default:                      
                return amountOfLives;     
        }
    }

    
}
