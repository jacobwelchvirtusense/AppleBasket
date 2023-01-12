/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:22:42 AM
 * 
 * Description: Handles movement of the basket based on player input.
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BasketMovement : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The size of the player object.
    /// </summary>
    private float playerSize = 10;

    #region Movement Type
    [Range(0.0f, 15.0f)]
    [Tooltip("The max distance reaches when at or past the max body angle")]
    [SerializeField] private float maxPos = 8;

    /// <summary>
    /// The types of input for moving the basket.
    /// </summary>
    private enum MovementType { LEAN, CATCH, TWOHANDS, MOVE }

    [Tooltip("The current type of input to use for moving the basket")]
    [SerializeField] private MovementType currentMovementType = MovementType.LEAN;

    [Header("Move Mode")]
    [Range(0.0f, 10.0f)]
    [Tooltip("The max position to check for move movement")]
    [SerializeField] private float maxMovePos = 1.0f;

    [Range(0.0f, 30.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float moveMovementSmoothing = 10;

    [Header("Catch Mode")]
    [Range(0.0f, 10.0f)]
    [Tooltip("The max hand positions to check for catch movement")]
    [SerializeField] private float maxCatchPos = 1.0f;

    [Range(0.0f, 30.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float catchMovementSmoothing = 10;
    #endregion

    [Header("Lean Mode")]
    [Range(0.0f, 30.0f)]
    [Tooltip("The max angle needed for the max distance of the basket")]
    [SerializeField] private float maxAngle = 10;

    [Range(0.0f, 30.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float leanMovementSmoothing = 10;

    #region Kinnect
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager bodyManager;

    private List<JointType> leanJoints = new List<JointType>
    {
        JointType.SpineBase,
        JointType.Neck
    };

    private List<JointType> moveJoints = new List<JointType>
    {
        JointType.SpineBase,
    };

    private List<JointType> twoHandsJoints = new List<JointType>
    {
        JointType.HandLeft,
        JointType.HandRight
    };

    private List<JointType> catchJoints = new List<JointType>
    {
        JointType.HandLeft,
        JointType.HandRight
    };
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Performs all actions in the awake event.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes all components for the player.
    /// </summary>
    private void InitializeComponents()
    {
        bodyManager = FindObjectOfType<BodySourceManager>();
    }
    #endregion

    #region Input Handling
    /// <summary>
    /// Updates game from Kinnect data.
    /// </summary>
    private void FixedUpdate()
    {
        #region Get Kinect Data
        if (bodyManager == null) return;

        Body[] _data = bodyManager.GetData();

        if (_data == null) return;

        List<ulong> _trackedIds = new List<ulong>();

        foreach (var body in _data)
        {
            if (body == null) continue;

            if (body.IsTracked) _trackedIds.Add(body.TrackingId);
        }
        #endregion

        #region Delete Untracked Bodies
        List<ulong> _knownIds = new List<ulong>(_Bodies.Keys);

        foreach (ulong trackingId in _knownIds)
        {
            if (!_trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create & Refresh Kinect Bodies
        foreach (var body in _data)
        {
            if (body == null) continue;

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body);
            }
        }
        #endregion
    }

    /// <summary>
    /// Creates the body in the scene.
    /// </summary>
    /// <param name="id">The id of the body to be created.</param>
    /// <returns></returns>
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jointObj.GetComponent<Renderer>().enabled = false;

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        return body;
    }

    /// <summary>
    /// Refreshes the events from body movement input.
    /// </summary>
    /// <param name="body">The body being checked.</param>
    private void RefreshBodyObject(Body body)
    {
        // Calculates the angle of the users spine

        //print("X: " + dir);
        //print("Angle: " + angle);

        var targetPos = 0.5f;
        var movementSmoothing = 10.0f;

        switch (currentMovementType)
        {
            case MovementType.MOVE:
                targetPos = CalculateMoveTargetPosition(body.Joints[moveJoints[0]].Position.X);
                movementSmoothing = moveMovementSmoothing;
                break;
            case MovementType.CATCH:
                targetPos = CalculateCatchTargetPosition(body.Joints[moveJoints[0]].Position.X, body.Joints[moveJoints[1]].Position.X);
                movementSmoothing = catchMovementSmoothing;
                break;
            case MovementType.TWOHANDS:
                break;
            default:
            case MovementType.LEAN:
                var dir = ((Vector2)(GetVector3FromJoint(body.Joints[leanJoints[0]]) - GetVector3FromJoint(body.Joints[leanJoints[1]]))).normalized;
                var angle = (180 - Vector2.Angle(dir, Vector2.up)) * -Mathf.Sign(dir.x);
                targetPos = CalculateLeanTargetPosition(angle);
                movementSmoothing = leanMovementSmoothing;
                break;
        }

        UpdateBasketPosition(targetPos, movementSmoothing);
    }

    /// <summary>
    /// Gets the vector3 position of the joint object.
    /// </summary>
    /// <param name="joint">The joint whose position is being coverted to Vector3.</param>
    /// <returns></returns>
    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * playerSize, joint.Position.Y * playerSize, joint.Position.Z * playerSize);
    }
    #endregion

    #region Calculate Target Position
    private float CalculateLeanTargetPosition(float angle)
    {
        var targetPositionLerp = Mathf.InverseLerp(-maxAngle, maxAngle, angle); // Calculates the lerp of the angle

        return targetPositionLerp;
    }

    private float CalculateMoveTargetPosition(float jointXPos)
    {
        print("Joint Pos: " + jointXPos);
        var targetPositionLerp = Mathf.InverseLerp(-maxMovePos, maxMovePos, jointXPos);

        return targetPositionLerp;
    }

    private float CalculateCatchTargetPosition(float hand1Pos, float hand2Pos)
    {
        var targetPosition1 = Mathf.InverseLerp(-maxCatchPos, maxCatchPos, hand1Pos);
        var targetPosition2 = Mathf.InverseLerp(-maxCatchPos, maxCatchPos, hand2Pos);
        var targetPositionLerp = (targetPosition1 + targetPosition2) / 2;

        return targetPositionLerp;
    }

    private float CalculateTwoHandsTargetPosition(float angle)
    {
        var targetPositionLerp = 0.0f;

        return targetPositionLerp;
    }
    #endregion

    /// <summary>
    /// Updates the position of the basket based on the user's spine angle.
    /// </summary>
    /// <param name="angle">The angle of the users spine.</param>
    private void UpdateBasketPosition(float targetPositionLerp, float movementSmoothing)
    {
        var pos = transform.position;
        var targetPosition = Mathf.Lerp(-maxPos, maxPos, targetPositionLerp);  // Calculates desired position based on angle

        // Sets the positions of the basket
        pos.x = Mathf.Lerp(pos.x, targetPosition, Time.fixedDeltaTime*movementSmoothing);
        transform.position = pos;
    }
    #endregion
}
