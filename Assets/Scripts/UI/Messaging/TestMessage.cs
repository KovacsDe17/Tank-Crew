using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMessage : MonoBehaviour
{
    public void ShowTestMessage()
    {
        //Message.Show("Test Title", "This is just a test text, which should be long enough to be truncated, but I hope this isn't that long, so I can read it.");
        Message.Show("Error!", "Couldn't connect to host! Please try again!");
    }
}
