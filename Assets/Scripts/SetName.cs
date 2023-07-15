using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetName : MonoBehaviour
{
    public TMP_InputField NameInput;

    
    // Start is called before the first frame update
    void Start()
    {
        NameInput = GetComponent<TMP_InputField>();
        
    }
    public void GetName()
    {
        ScoreManager.Instance.playerName = NameInput.text;
        
    }
}
