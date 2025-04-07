using UnityEngine;

public class DisappearingPlattform : MonoBehaviour
{
    public float timeToTogglePlatform = 2;
    public float currentTime = 0;
    public new bool enabled = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= Time.deltaTime){
            currentTime = 0;
            TogglePlatform();
        }
    }

    void TogglePlatform(){
        enabled = !enabled;
        foreach(Transform child in gameObject.transform){
            if(child.tag != "Player"){
                child.gameObject.SetActive(enabled);
            }
        }
    }
}
