using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickFirst()
    {
        Debug.Log("FirstCLick");

        Assets.Resources.Scripts.GameManager.instance.SetHumanFirst();

    }

    public void OnClickSecond()
    {
        Debug.Log("SecondCLick");
        Assets.Resources.Scripts.GameManager.instance.SetHumanSecond();
    }

    public void SetResultWinText()
    {

        Debug.Log("Win");
    }

    public void SetResultLoseText()
    {
        Debug.Log("Lose");
    }
}
