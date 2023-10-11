using UnityEngine;

/// <summary>
/// The UI handler for the hatch object of the turret.
/// </summary>
public class HatchHandler : MonoBehaviour
{
    private bool _isOpen;

    private Turret _turret;
    private Animator _animator;
    private Ammo _loadedAmmo;

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Assign the RectTransform to the object, and set the rotation to the closed state
    /// </summary>
    private void Initialize()
    {
        _isOpen = false;
        _loadedAmmo = null;

        _animator = transform.parent.gameObject.GetComponent<Animator>();
        _turret = PlayerTank.Instance.GetComponentInChildren<Turret>();
    }

    /// <summary>
    /// Check if the hatch is open
    /// </summary>
    /// <returns>True when the hatch is open</returns>
    public bool IsOpen()
    {
        return _isOpen;
    }

    /// <summary>
    /// Set the ammunition to be loaded
    /// </summary>
    /// <param name="ammo">The UI element which represents the ammunition</param>
    public void LoadAmmo(Ammo ammo)
    {
        if (_loadedAmmo != null)
            return;

        _loadedAmmo = ammo;

        SwitchEasyGrab(false);
    }

    /// <summary>
    /// Discard the currently loaded ammunition
    /// </summary>
    public void UnloadAmmo()
    {
        SwitchEasyGrab(true);
        _loadedAmmo = null;
    }

    /// <summary>
    /// If it is possible to shoot, remove the head of the ammunitions UI representation and fire the turret
    /// </summary>
    public void InitiateShooting()
    {
        if (_loadedAmmo == null || !_loadedAmmo.IsReady() || IsOpen())
            return;

        string clip = "Shoot";
        _animator.Play("Base Layer." + clip, 0, 0);
        _loadedAmmo.Fire();
        _turret.FireProjectileServerRPC();
    }

    public void SwitchOpenState()
    {
        string clip = _isOpen ? "CloseHatch" : "OpenHatch";

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clip))
            return;

        _animator.Play("Base Layer." + clip, 0, 0);

        SwitchEasyGrab(_isOpen);

        _isOpen = !_isOpen;
    }

    private void SwitchEasyGrab(bool activate)
    {
        if (_loadedAmmo == null)
            return;

        GameObject easyGrab = _loadedAmmo.transform.Find("EasyDrag").gameObject;
        easyGrab.SetActive(!activate);
    }
}
