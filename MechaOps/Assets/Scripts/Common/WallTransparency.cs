using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WallTransparency : MonoBehaviour {

    [SerializeField]
    private float minAlpha = 0.25f;
    [SerializeField]
    private float maxAlpha = 1.0f;
    [SerializeField]
    private float alphaChangeSpeed = 1.0f;
    private bool turnTransparent = false;

    public void SetMinAlpha(float _minAlpha) {
        minAlpha = Mathf.Clamp(_minAlpha, 0.0f, maxAlpha);
    }

    public float GetMinAlpha() {
        return minAlpha;
    }

    public void SetMaxAlpha(float _maxAlpha) {
        maxAlpha = Mathf.Clamp(_maxAlpha, 0.0f, 1.0f);
        minAlpha = Mathf.Min(maxAlpha, minAlpha);
    }

    public float GetMaxAlpha() {
        return maxAlpha;
    }
    
    public void SetAlphaChangeSpeed(float _alphaChangeSpeed) {
        alphaChangeSpeed = Mathf.Max(0.0f, _alphaChangeSpeed);
    }

    public float GetAlphaChangeSpeed() {
        return alphaChangeSpeed;
    }
    
    public void TurnTransparent() {
        turnTransparent = true;
    }
	
	// Update is called once per frame
	void Update () {
        // Get this GameObject's MeshRenderer.
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        
        if (meshRenderer == null) {
            return;
        }
        //Assert.AreNotEqual(meshRenderer, null);

        Material[] materials = meshRenderer.materials;

        if (turnTransparent) {
            for (uint i = 0; i < materials.Length; ++i) {
                if (materials[i].color.a != minAlpha) {
                    Color color = materials[i].color;
                    color.a = Mathf.Max(minAlpha, color.a - alphaChangeSpeed * Time.deltaTime);
                    materials[i].color = color;
                }
            }

            turnTransparent = false;
        } else {
            for (uint i = 0; i < materials.Length; ++i) {
                if (materials[i].color.a != maxAlpha) {
                    Color color = materials[i].color;
                    color.a = Mathf.Min(maxAlpha, color.a + alphaChangeSpeed * Time.deltaTime);
                    materials[i].color = color;
                }
            }
        }
	}

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetMaxAlpha(maxAlpha);
        SetMinAlpha(minAlpha);
        SetAlphaChangeSpeed(alphaChangeSpeed);
    }
#endif
}