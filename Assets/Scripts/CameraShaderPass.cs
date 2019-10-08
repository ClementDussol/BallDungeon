using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaderPass : MonoBehaviour
{
	public Material[] materials;
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		foreach (Material mat in materials)
		{
			Graphics.Blit(src, dest, mat);
		}
	}
}
