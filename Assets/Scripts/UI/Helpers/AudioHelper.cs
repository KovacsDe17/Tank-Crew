using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A helper class for the Audio Manager.
/// </summary>
public class AudioHelper : MonoBehaviour
{
    public AudioManager.Category audioCategory;
    public List<AudioHelper> containingList;

    public void SetDontDestroyOnLoad(bool dontDestroyOnLoad)
    {
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(this);
        else
            transform.parent = null;
    }

    private void OnDestroy()
    {
        if(containingList != null && containingList.Contains(this))
            containingList.Remove(this);
    }

    public override string ToString()
    {
        return "Name: " + gameObject.name + ", Cat: " + audioCategory;
    }
}