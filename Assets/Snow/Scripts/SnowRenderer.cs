using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowRenderer : MonoBehaviour {
	//Plane size: default Unity's plane is (10,10), local scale should be considered
	public Vector2 size = new Vector2 (10, 10);
	//Collapse map resolution
	public int resolution = 2048;
	//The plane's maximum height and depth camera's view distance, if you are using this component for the snow on a terrain. Please transform this value to the maximum height difference of this terrain, this value should not be less than 1
	public float planeHeight = 1;
	//Collapsible object mask
	public LayerMask mask;
	//Height map's vertex displacement scale
	public float vertexScale = 1;
	//Height map's vertex offset
	public float vertexOffset = 0.5f;
	//Height map
	public Texture heightMap;
	Camera cam;
	Camera normalCam;
	RenderTexture compare1Tex;		//Camera render target
	RenderTexture targetTex;
	RenderTexture normalTex;

	int depthTexID;
	int snowNormalID;
	int worldPosID;
	int compareID;
	int vertexID;
	int heightMapID;
	void Awake () {
		var camGo = new GameObject ("snow cam", typeof(Camera));
		camGo.hideFlags = HideFlags.HideAndDontSave;
		float aspect = size.x / size.y;
		compare1Tex = new RenderTexture ((int)(resolution * aspect), resolution, 24, RenderTextureFormat.RFloat);
		targetTex = new RenderTexture (compare1Tex.descriptor);
		cam = camGo.GetComponent<Camera> ();
		camGo.transform.position = transform.position - Vector3.up * planeHeight;
		camGo.transform.eulerAngles = new Vector3 (-90, 0);
		camGo.transform.SetParent (transform);
		cam.renderingPath = RenderingPath.Forward;
		cam.orthographic = true;
		cam.orthographicSize = size.y / 2;
		cam.aspect = aspect;
		cam.targetTexture = compare1Tex;
		cam.clearFlags = CameraClearFlags.Depth;
		cam.enabled = false;
		cam.cullingMask = mask;
		cam.backgroundColor = Color.black;
		cam.allowMSAA = false;
		cam.useOcclusionCulling = false;
		cam.farClipPlane = planeHeight * 2;
		cam.allowHDR = false;
		normalCam = (Instantiate (camGo, transform) as GameObject).GetComponent<Camera> ();
		normalCam.gameObject.hideFlags = HideFlags.HideAndDontSave;
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
		vertexID = Shader.PropertyToID ("_VertexInfo");
		heightMapID = Shader.PropertyToID ("_PlaneHeightMap");
		var rd = GetComponent<Renderer> ();
		MaterialPropertyBlock block = new MaterialPropertyBlock ();
		block.SetTexture (depthTexID, targetTex);
		block.SetTexture (snowNormalID, normalTex);
		if (heightMap) {
			block.SetTexture ("_HeightMap", heightMap);
			block.SetVector (vertexID, new Vector4 (vertexScale, vertexOffset));

		} else {
			vertexScale = 0;
			vertexOffset = 0;
			block.SetVector (vertexID, new Vector4 (vertexScale, vertexOffset));
		}
		rd.SetPropertyBlock (block);
	}

	void OnWillRenderObject(){
		Shader.SetGlobalVector (worldPosID, new Vector4(vertexScale, vertexOffset, transform.position.y));
		Shader.SetGlobalTexture (compareID, targetTex);
		Shader.SetGlobalTexture (heightMapID, heightMap);
		cam.Render ();
		normalCam.Render ();
		Graphics.Blit (compare1Tex, targetTex);
	}
}
