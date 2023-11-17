using System.Threading.Tasks;
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

        Win();
    }

    /// <summary>
    /// When the Player destroys the objective.
    /// </summary>
    private void OnObjectiveDestroyed()
    {
        Debug.Log("The Objective has been destroyed!");

        Win();
    }

    /// <summary>
    /// When the Player captures the objective.
    /// </summary>
    private void OnObjectiveCaptured()
    {
        Debug.Log("The Objective has been captured!");

        Win();
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

    /// <summary>
    /// When the player wins, wait for two seconds before firing the event.
    /// </summary>
    private async void Win()
    {
        await Task.Delay(2000); //Wait for two seconds

        GameManager.Instance.InvokeOnGameEnd(true);
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
        if(_objectiveType == ObjectiveType.Destroy)
            OnObjectiveDestroyed();
    }
}
