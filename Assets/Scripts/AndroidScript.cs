using UnityEngine;
using System.Collections;

using OpenCVForUnity;

namespace OpenCVForUnitySample
{
		/// <summary>
		/// WebCamTexture to mat sample.
		/// </summary>
		public class AndroidScript : MonoBehaviour
		{
	
				/// <summary>
				/// The web cam texture.
				/// </summary>
				WebCamTexture webCamTexture;

				/// <summary>
				/// The web cam device.
				/// </summary>
				WebCamDevice webCamDevice;

				/// <summary>
				/// The colors.
				/// </summary>
				Color32[] colors;

				/// <summary>
				/// The is front facing.
				/// </summary>
				public bool isFrontFacing = false;

				/// <summary>
				/// The width.
				/// </summary>
				int width = 480;

				/// <summary>
				/// The height.
				/// </summary>
				int height = 640;

				/// <summary>
				/// The rgba mat.
				/// </summary>
				Mat rgbaMat;
				Mat rotatedrgbaMat;

				/// <summary>
				/// The texture.
				/// </summary>
				Texture2D texture;
				Texture2D rotatedtexture;	

				/// <summary>
				/// The init done.
				/// </summary>
				bool initDone = false;

	
				// Use this for initialization
				void Start ()
				{
						
						StartCoroutine (init ());

				}

				private IEnumerator init ()
				{
						if (webCamTexture != null) {
								webCamTexture.Stop ();
								initDone = false;
				
								rgbaMat.Dispose ();
						}

						// Checks how many and which cameras are available on the device
						for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
				
				
								if (WebCamTexture.devices [cameraIndex].isFrontFacing == isFrontFacing) {
					
					
										Debug.Log (cameraIndex + " name " + WebCamTexture.devices [cameraIndex].name + " isFrontFacing " + WebCamTexture.devices [cameraIndex].isFrontFacing);

										webCamDevice = WebCamTexture.devices [cameraIndex];

										webCamTexture = new WebCamTexture (webCamDevice.name, width, height);
										
										break;
								}
				
				
						}
			
						if (webCamTexture == null) {
								webCamDevice = WebCamTexture.devices [0];
								webCamTexture = new WebCamTexture (webCamDevice.name, width, height);
						}
			
						Debug.Log ("width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
			
			
			
						// Starts the camera
						webCamTexture.Play ();


						while (true) {
								//If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/)
								#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
				                if (webCamTexture.width > 16 && webCamTexture.height > 16) {
								#else
								if (webCamTexture.didUpdateThisFrame) {
										#endif

										Debug.Log ("width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
										Debug.Log ("videoRotationAngle " + webCamTexture.videoRotationAngle + " videoVerticallyMirrored " + webCamTexture.videoVerticallyMirrored + " isFrongFacing " + webCamDevice.isFrontFacing);
					
										colors = new Color32[webCamTexture.width * webCamTexture.height];
					
										rgbaMat = new Mat (webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
										rotatedrgbaMat = new Mat(webCamTexture.width, webCamTexture.height, CvType.CV_8UC3);

										//texture = new Texture2D (webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
										texture = new Texture2D (webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
										
										
										gameObject.transform.eulerAngles = new Vector3 (0, 0, 0);
										#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
										gameObject.transform.eulerAngles = new Vector3 (0, 0, -90);
										#endif

//										gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.AngleAxis (webCamTexture.videoRotationAngle, Vector3.back);

										//gameObject.transform.localScale = new Vector3 (webCamTexture.width, webCamTexture.height, 1);
										gameObject.transform.localScale = new Vector3 (webCamTexture.width, webCamTexture.height, 1);

//										bool videoVerticallyMirrored = webCamTexture.videoVerticallyMirrored;
//										float scaleX = 1;
//										float scaleY = videoVerticallyMirrored ? -1.0f : 1.0f;
//										if (webCamTexture.videoRotationAngle == 270)
//												scaleY = -1.0f;
//										gameObject.transform.localScale = new Vector3 (scaleX * gameObject.transform.localScale.x, scaleY * gameObject.transform.localScale.y, 1);


										gameObject.GetComponent<Renderer> ().material.mainTexture = texture;

										#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
										Camera.main.orthographicSize = webCamTexture.width / 2;
										#else
										Camera.main.orthographicSize = webCamTexture.height / 2;
										#endif

										initDone = true;
					
										break;
								} else {
										yield return 0;
								}
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (!initDone)
								return;
		
						#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
				        if (webCamTexture.width > 16 && webCamTexture.height > 16) {
						#else
						if (webCamTexture.didUpdateThisFrame) {
								#endif
								
								
								Utils.webCamTextureToMat (webCamTexture, rgbaMat);

								if (webCamTexture.videoVerticallyMirrored) {
										if (webCamDevice.isFrontFacing) {
												if (webCamTexture.videoRotationAngle == 0) {
														Core.flip (rgbaMat, rgbaMat, 1);
												} else if (webCamTexture.videoRotationAngle == 90) {
														Core.flip (rgbaMat, rgbaMat, 0);
												} else if (webCamTexture.videoRotationAngle == 270) {
														Core.flip (rgbaMat, rgbaMat, 1);
												}
										} else {
												if (webCamTexture.videoRotationAngle == 90) {
									
												} else if (webCamTexture.videoRotationAngle == 270) {
														Core.flip (rgbaMat, rgbaMat, -1);
												}
										}
								} else {
										if (webCamDevice.isFrontFacing) {
												if (webCamTexture.videoRotationAngle == 0) {
														Core.flip (rgbaMat, rgbaMat, 1);
												} else if (webCamTexture.videoRotationAngle == 90) {
														Core.flip (rgbaMat, rgbaMat, 0);
												} else if (webCamTexture.videoRotationAngle == 270) {
														Core.flip (rgbaMat, rgbaMat, 1);
												}
										} else {
												if (webCamTexture.videoRotationAngle == 90) {
									
												} else if (webCamTexture.videoRotationAngle == 270) {
														Core.flip (rgbaMat, rgbaMat, -1);
												}
										}
								}
								
								
						rotatedrgbaMat = rgbaMat.t();
						Core.flip (rotatedrgbaMat, rotatedrgbaMat, 1);

						Core.flip (rotatedrgbaMat, rotatedrgbaMat, 1);
						rgbaMat = rotatedrgbaMat.t();
								
								Utils.matToTexture2D (rgbaMat, texture);
		
								gameObject.GetComponent<Renderer> ().material.mainTexture = texture;

						}

				}
	
				void OnDisable ()
				{
						webCamTexture.Stop ();
				}
	
				void OnGUI ()
				{
						float screenScale = Screen.width / 240.0f;
						Matrix4x4 scaledMatrix = Matrix4x4.Scale (new Vector3 (screenScale, screenScale, screenScale));
						GUI.matrix = scaledMatrix;
		
		
						GUILayout.BeginVertical ();
						if (GUILayout.Button ("back")) {
								Application.LoadLevel ("OpenCVForUnitySample");
						}
						if (GUILayout.Button ("change camera")) {
								isFrontFacing = !isFrontFacing;
								StartCoroutine (init ());
						}
						GUILayout.Label ("Height= " + webCamTexture.height + " Width= " + webCamTexture.width);
						
						Vector3 objsize = new Vector3 ();
						//objsize = gameObject.renderer.bounds.size;
					GUILayout.Label ("Height= " + objsize.y + " Width= " + objsize.x);
					GUILayout.Label ("Height= " + rotatedrgbaMat.height() + " Width= " + rotatedrgbaMat.width());
						GUILayout.EndVertical ();
				}
		}
}
