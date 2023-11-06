using UnityEngine;

/// <summary>
/// An objective point for the Player.
/// </summary>
public class Objective : MonoBehaviour
{
    /// <summary>
    /// Wether to reach, destroy or capture this objective.
    /// </summary>
    public enum ObjectiveType { Reach, Destroy, Capture}

    //The Objective Type of this objective
    [SerializeField] private ObjectiveType _objectiveType;

    /// <summary>
    /// When the Player reaches the objective.
    /// </summary>
    private void OnObjectiveReached()
    {
        Debug.Log("The Objective has been reached!");
        GameManager.Instance.OnGameWin();
    }

    /// <summary>
    /// When the Player destroys the objective.
    /// </summary>
    private void OnObjectiveDestroyed()
    {
        Debug.Log("The Objective has been destroyed!");
        GameManager.Instance.OnGameWin();
    }

    /// <summary>
    /// When the Player captures the objective.
    /// </summary>
    private void OnObjectiveCaptured()
    {
        Debug.Log("The Objective has been captured!");
        GameManager.Instance.OnGameWin();
    }

    /// <summary>
    /// When the Player starts capturing the objective.
    /// </summary>
    private void StartCapturing()
    {
        Debug.Log("Started capturing objective!");
        //TODO: implement -> start timer
    }

    /// <summary>
    /// When the Player is capturing the objective.
    /// </summary>
    private void IsCapturing()
    {
        Debug.Log("Capturing objective...");
        //TODO: implement -> check timer
        //If timer is up -> OnObjectiveCaptured();
    }

    /// <summary>
    /// When the Player ends capturing the objective.
    /// </summary>
    private void EndCapturing()
    {
        Debug.Log("Ended capturing objective!");
        //TODO: implement -> reset timer
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(_objectiveType)
        {
            case ObjectiveType.Reach: OnObjectiveReached(); break;
            case ObjectiveType.Capture: StartCapturing(); break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_objectiveType == ObjectiveType.Capture)
            IsCapturing();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_objectiveType == ObjectiveType.Capture)
            EndCapturing();
    }

    private void OnDestroy()
    {
        OnObjectiveDestroyed();
    }
}
