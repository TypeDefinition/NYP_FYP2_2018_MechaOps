using UnityEngine;
using System.Collections;

public enum TrackSide
{
    Left,
    Right
}


public class TrackAnimator : MonoBehaviour
{
    public float speed;
    public Transform[] wheels;
    float actualSpeed = 0;

    public Renderer tracks;
    public float trackSpeedMod = 1;


    Material trackMaterial;
    Vector2 textureOffset;

    void Start ()
    {
        trackMaterial = tracks.material;
        textureOffset = trackMaterial.mainTextureOffset;
    }
    
    void Update ()
    {
        
        foreach (var w in wheels) {
            w.Rotate (new Vector3 (actualSpeed * Time.deltaTime, 0, 0), Space.Self);
        }
        textureOffset.x += actualSpeed * Time.deltaTime * trackSpeedMod;

        trackMaterial.mainTextureOffset = textureOffset;
        actualSpeed = Mathf.Lerp (actualSpeed, speed, Time.deltaTime);
        
    }
}
