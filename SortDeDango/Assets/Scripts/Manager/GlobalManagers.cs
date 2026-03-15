using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagers : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
