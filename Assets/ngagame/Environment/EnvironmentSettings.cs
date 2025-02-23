using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSettings : MonoBehaviour
{
	[Header("Fog")]
	[SerializeField] bool fog = false;
	[SerializeField] Color fogColor;
	[SerializeField] FogMode fogMode = FogMode.Linear;
	[SerializeField] float fogStart = 50;
	[SerializeField] float fogEnd= 200;
	[SerializeField] float fogDensity = 0.004f;
	[Header("Camera")]
	[SerializeField] bool overrideCanmeraSettings;
	Camera cameraConfig;
	[Header("Lighting")]
	[SerializeField] bool overrideLightSettings;
	Light lightConfig;

	private void Start()
	{
		ApplyFog();
		ApplyCameraSettings();
	}

	[ContextMenu("Apply")]
	public void ApplyFog()
	{
		RenderSettings.fog = fog;
		RenderSettings.fogMode = fogMode;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.fogStartDistance = fogStart;
		RenderSettings.fogEndDistance = fogEnd;
		RenderSettings.fogColor = fogColor;
	}

	public void ApplyLightSettings(Light light)
	{
		if(!overrideLightSettings || light == null)
		{
			return;
		}
		lightConfig = GetComponentInChildren<Light>(true);
		
		if (lightConfig == null)
		{
			return;
		}

		light.transform.localRotation = lightConfig.transform.localRotation;
		light.color = lightConfig.color;
		light.intensity = lightConfig.intensity;
	}

	public void ApplyCameraSettings(Camera cam = null)
	{
		if (!overrideCanmeraSettings)
		{
			return;
		}
		cameraConfig = GetComponentInChildren<Camera>(true);
		if(cam == null)
		{
			cam = Camera.main;
		}
		if (cameraConfig == null || cam == null)
		{
			return;
		}

		cam.transform.position = cameraConfig.transform.position;
		cam.transform.localRotation = cameraConfig.transform.localRotation;
		cam.clearFlags = cameraConfig.clearFlags;
		cam.backgroundColor = cameraConfig.backgroundColor;
		cam.fieldOfView = cameraConfig.fieldOfView;
		cam.nearClipPlane = cameraConfig.nearClipPlane;
		cam.farClipPlane = cameraConfig.farClipPlane;
	}
}
