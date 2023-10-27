using UnityEngine;
using UnityEngine.UI;

public class CameraSize : MonoBehaviour
{
    private Camera mainCamera;
    public Slider CameraSizeSlider;
    
    private float xOffset = 2.2375f;
    private float yOffset = 2.2375f;
    private float BorderSize;

    private void Start()
    {
        mainCamera = Camera.main;
        SetupCamera(8, 8);
        CameraSizeSlider.value = mainCamera.orthographicSize;
    }

    private void SetupCamera(byte x, byte y)
    {
        mainCamera.transform.position = new Vector3((x / 2) * xOffset, (y / 2) * yOffset, -10f);
            
        float aspect = Screen.width / (float) (Screen.height + 1) ;
        if (aspect > 9 / 16f) aspect = 8.5f / 16f;

        BorderSize = xOffset * x;
        mainCamera.orthographicSize = (  BorderSize * .6f ) / aspect;
    }
    
    public void CameraSizeChange() //called slider 
    {
        mainCamera.orthographicSize = CameraSizeSlider.value;
    }
    
}
