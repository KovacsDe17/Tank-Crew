using System;
using UnityEngine;

/// <summary>
/// Movement script for UI Ammunition elements (for loading the turret)
/// </summary>
public class AmmoMovement : MonoBehaviour
{
    #region Placement class
    private class Placement
    {
        public Vector2 position;
        public Quaternion rotation;

        public Placement(Vector2 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    #endregion

    #region Variables
    private RectTransform _rectTransform, _chamberRectTransform, _hatchRectTransform;
    private Rigidbody2D _rigidbody;
    private Placement _origin, _previous;
    private Ammo _ammo;
    private Animator _animator;
    private HatchHandler _hatchHandler;

    private float _defaultDistanceFromChamber, _gapBetween, _layingOut;
    
    private Vector2 _placeInChamber;

    private bool _isSnappedToPlace;
    private bool _firstMove;
    private bool _canMoveToHolder;

    #endregion

    #region Initalization

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Setup variables before the scene starts
    /// </summary>
    private void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _ammo = GetComponent<Ammo>();
        _animator = GetComponent<Animator>();
        _hatchHandler = FindObjectOfType<HatchHandler>();
        _hatchRectTransform = _hatchHandler.GetComponent<RectTransform>();

        _chamberRectTransform = GameObject.FindGameObjectWithTag("Chamber").GetComponent<RectTransform>();
        SavePrevious();

        _gapBetween = 250f;
        float xbeforeChamber = _chamberRectTransform.position.x - _rectTransform.rect.height - _gapBetween;
        //_placeBeforeChamber = new Vector2(xbeforeChamber, _chamber.position.y);

        _layingOut = 50f;
        //_placeInChamber = new Vector2(_chamber.position.x - _layingOut, _chamber.position.y);
        _placeInChamber = GameObject.FindGameObjectWithTag("PlaceInChamber").transform.position;


        _isSnappedToPlace = false;
        _firstMove = true;
        _canMoveToHolder = true;
    }

    #endregion

    #region Drag methods

    /// <summary>
    /// When the ammo is grabbed
    /// </summary>
    public void OnBeginDrag()
    {
        if (_firstMove) {
            SaveOrigin();
            _firstMove = false;
        }

        if (_ammo.IsShot())
            _canMoveToHolder = false;

        SavePrevious();
    }

    /// <summary>
    /// When the ammunition is being moved (after being grabbed)
    /// </summary>
    public void OnDrag()
    {
        if (CloseTo(_origin.position, 0) && !_canMoveToHolder)
            return;

        if (CloseTo(_placeInChamber, _rectTransform.rect.width) && !_hatchHandler.IsOpen())
            return;

        if (!_isSnappedToPlace && !_ammo.IsShot())
        {
            if (CloseTo(_placeInChamber, 100f))
            {
                SnapToPlace(_placeInChamber);
                _hatchHandler.LoadAmmo(_ammo);
                _ammo.SetReady(true);
                return;
            }

            if (CloseTo(_origin.position, 100f))
            {
                SnapToPlace(_origin.position);
                _hatchHandler.UnloadAmmo();
                SavePrevious();
                return;
            }
        } else
        {
            ReleaseFromPlace();
        }

        SetAmmoPosition(GetClosestTouchPosition());    
        
        if(!_ammo.IsShot())
            FlipBasedOnDistance(0.65f);
    }

    /// <summary>
    /// When the ammunition is released (after being moved)
    /// </summary>
    public void OnEndDrag() 
    {
        if (_ammo.IsShot())
        {
            Discard();
            return;
        }

        SnapToPrevious();
    }

    #endregion

    #region Positioning
    /// <summary>
    /// Save last position and rotation
    /// </summary>
    private void SavePrevious()
    {
        _previous = new Placement(_rectTransform.position, _rectTransform.rotation);
    }

    /// <summary>
    /// Set position and rotation to the last saved ones
    /// </summary>
    private void SnapToPrevious()
    {
        ReleaseFromPlace();

        bool toChamber = !(_previous.position == _placeInChamber);

        Flip(toChamber);

        _rectTransform.position = _previous.position;
        _rectTransform.rotation = _previous.rotation;
    }

    /// <summary>
    /// Save the origin position and rotation
    /// </summary>
    private void SaveOrigin()
    {
        _origin = new Placement(_rectTransform.position, _rectTransform.rotation);
        _defaultDistanceFromChamber = Vector2.Distance(_chamberRectTransform.position, _rectTransform.position);
    }


    /// <summary>
    /// Set the position of the UI ammunition object
    /// </summary>
    /// <param name="position">The desired position</param>
    private void SetAmmoPosition(Vector2 position)
    {
        _rigidbody.MovePosition(position);
    }

    private void SnapToPlace(Vector2 place)
    {
        _rectTransform.position = place;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        SavePrevious();
        _isSnappedToPlace = true;
    }

    private void ReleaseFromPlace()
    {
        if(_rigidbody.constraints != RigidbodyConstraints2D.None)
        {
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            _isSnappedToPlace = false;
        }    
    }

    /// <summary>
    /// Drop the ammo
    /// </summary>
    private void Discard()
    {
        _hatchHandler.UnloadAmmo();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        //TODO: Add force depending on movement speed
        _rigidbody.gravityScale = 250;
        ReleaseFromPlace();
        Destroy(gameObject, 3f);
        //TODO: load a new ammo to this ammo's origin
    }

    #endregion

    #region Rotating
    //TODO: based on direction ((lastPos - currentPos) > threshold)
    private void FlipBasedOnDistance(float threshold)
    {
        float distance = Vector2.Distance(_chamberRectTransform.position, _rectTransform.position);
        float distancePercentage = distance / _defaultDistanceFromChamber;

        if (distancePercentage <= threshold)
        {
            Flip(false);
        }
        else
        {
            Flip(true);
        }
    }

    private void Flip(bool toHolder)
    {
        if (toHolder && NormalizeAngle(_rigidbody.rotation) == 0)
            return;

        if (!toHolder && NormalizeAngle(_rigidbody.rotation) == 180)
            return;

        string clip = toHolder ? "FlipToHolder" : "FlipToChamber";

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clip))
            return;


        _animator.Play("Base Layer." + clip,0,0);
    }

    #endregion

    #region Helper methods

    /// <summary>
    /// Clamps the angle between 0 and 360 degrees
    /// </summary>
    /// <param name="angle">The angle to be normalized</param>
    /// <returns>The angle set between 0 and 360 degrees</returns>
    private float NormalizeAngle(float angle)
    {
        while (angle < 0)
            angle += 360;

        while (angle > 360)
            angle -= 360;

        return angle;
    }

    /// <summary>
    /// Decide if the ammunition is close enough to the given position
    /// </summary>
    /// <param name="position">The position to be compared to</param>
    /// <param name="threshold">The distance which is close enough</param>
    /// <returns>True when the ammunition is within the threshold</returns>
    private bool CloseTo(Vector2 position, float threshold)
    {
        float distance = Vector2.Distance(GetClosestTouchPosition(), position);

        return distance <= threshold;
    }

    //TODO: outsource to a unified input class?
    /// <summary>
    /// Get the position of the closest touch (or the mouse) to the UI ammunition
    /// </summary>
    /// <returns>The position of the closest touch (or the mouse)</returns>
    private Vector2 GetClosestTouchPosition()
    {
        if (Application.isEditor)
            return Input.mousePosition;

        Touch closest = Input.GetTouch(0);

        foreach (Touch touch in Input.touches)
        {
            float currentTouchDistance = Vector2.Distance(touch.position, Camera.main.WorldToScreenPoint(transform.position));
            float closestTouchDistance = Vector2.Distance(closest.position, Camera.main.WorldToScreenPoint(transform.position));

            if (currentTouchDistance < closestTouchDistance)
            {
                closest = touch;
            }
        }

        return closest.position;
    }

    #endregion
}
