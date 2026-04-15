using UnityEngine;

public class CameraDepth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera myCamera;

    private void OnEnable()
    {
        myCamera.depthTextureMode |= DepthTextureMode.Depth;
    }
}
