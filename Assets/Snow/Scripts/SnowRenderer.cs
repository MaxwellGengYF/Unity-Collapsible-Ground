using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowRenderer : MonoBehaviour {
	public float height;
	public float width;
	public int resolution = 2048;
	public float planeHeight = 1;
	public LayerMask mask;
	Camera cam;
	Camera normalCam;
	RenderTexture compare1Tex;		//Camera render target
	RenderTexture compare2Tex;
	RenderTexture targetTex;
	RenderTexture normalTex;
	Material postProcessMat;
	int depthTexID;
	int snowNormalID;
	int worldPosID;
	int compareID;
	Material snowMat;
	void Awake () {
		var camGo = new GameObject ("snow cam", typeof(Camera));
		float aspect = width / height;
		compare1Tex = new RenderTexture ((int)(resolution * aspect), resolution, 24, RenderTextureFormat.RFloat);
		compare2Tex = new RenderTexture (compare1Tex.descriptor);
		targetTex = new RenderTexture (compare1Tex.descriptor);
		cam = camGo.GetComponent<Camera> ();
		camGo.transform.position = transform.position - Vector3.up * planeHeight;
		camGo.transform.eulerAngles = new Vector3 (-90, 0);
		camGo.transform.SetParent (transform);
		cam.renderingPath = RenderingPath.Forward;
		cam.orthographic = true;
		cam.orthographicSize = height / 2;
		cam.aspect = aspect;
		cam.targetTexture = compare1Tex;
		cam.enabled = false;
		cam.cullingMask = mask;
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.black;
		cam.allowMSAA = false;
		cam.useOcclusionCulling = false;
		cam.farClipPlane = planeHeight * 2;
		cam.allowHDR = false;
		normalCam = (Instantiate (camGo, transform) as GameObject).GetComponent<Camera> ();
		normalCam.CopyFrom (cam);
		normalCam.clearFlags = CameraClearFlags.Depth;
		normalTex = new RenderTexture ((int)(resolution * aspect), resolution, 24, RenderTextureFormat.ARGBFloat);
		normalCam.targetTexture = normalTex;
		cam.SetReplacementShader (Shader.Find ("Hidden/SnowWorldPos"), "");
		normalCam.SetReplacementShader (Shader.Find ("Hidden/SnowNormal"), "");
		compareID = Shader.PropertyToID ("_CompareTex");
		depthTexID = Shader.PropertyToID ("_SnowDepthTex");
		snowNormalID = Shader.PropertyToID ("_SnowNormalTex");
		worldPosID = Shader.PropertyToID ("_WorldYPos");
		postProcessMat = new Material (Shader.Find ("Hidden/SnowRenderer"));
		snowMat = GetComponent<Renderer> ().sharedMaterial;
		snowMat.SetTexture (depthTexID, targetTex);
		snowMat.SetTexture (snowNormalID, normalTex);
	}
	void Update(){
		Shader.SetGlobalFloat (worldPosID, transform.position.y);
		Shader.SetGlobalTexture (compareID, compare2Tex);
		cam.Render ();
		normalCam.Render ();
		Graphics.Blit (compare1Tex, targetTex, postProcessMat);
		Graphics.Blit (targetTex, compare2Tex);

	}
}
