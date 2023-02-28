/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Apple Basket
 * Creation Date: 2/27/2023 11:28:01 AM
 * 
 * Description: TODO
*********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class DepthSensorDisplay : MonoBehaviour
{
    #region Fields
    private MultiSourceManager multiSourceManager;
    private RawImage depthImage;
    #endregion

    #region Functions
    #region MesaureDepth
    private Texture2D depthTexture = null;
    private ushort[] depthData = null;
    private CameraSpacePoint[] cameraSpacePoints = null;
    private ColorSpacePoint[] colorSpacePoints = null;

    private KinectSensor sensor = null;
    private CoordinateMapper mapper = null;

    private readonly Vector2Int depthResolution = new Vector2Int(512, 424);

    private void Awake()
    {
        multiSourceManager = FindObjectOfType<MultiSourceManager>();
        sensor = KinectSensor.GetDefault();

        if(sensor != null)
        {
            mapper = sensor.CoordinateMapper;

            int arraySize = depthResolution.x * depthResolution.y;

            cameraSpacePoints = new CameraSpacePoint[arraySize];
            colorSpacePoints = new ColorSpacePoint[arraySize];
        }
    }

    private void DepthToColor()
    {
        depthData = multiSourceManager.GetDepthData();

        mapper.MapDepthFrameToCameraSpace(depthData, cameraSpacePoints);
        mapper.MapDepthFrameToColorSpace(depthData, colorSpacePoints);

        depthTexture = CreateTexture();
    }

    private Texture2D CreateTexture()
    {
        Texture2D newTexture = new Texture2D(1, 1, TextureFormat.Alpha8, false);

        newTexture.SetPixel(0, 0, Color.clear);

        foreach(ColorSpacePoint colorSpacePoint in colorSpacePoints)
        {
            newTexture.SetPixel((int)colorSpacePoint.X, (int)colorSpacePoint.Y, Color.green);
        }

        newTexture.Apply();

        return newTexture;
    }
    #endregion

    private void Start()
    {
        depthImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        //depthImage.texture = multiSourceManager.GetColorTexture();
        DepthToColor();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DepthToColor();
        }
        //
        depthImage.texture = depthTexture;
        
    }
    #endregion
}
