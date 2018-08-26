using UnityEngine;
using System.Collections;

namespace Blendy
{
	public enum CubemapFace
	{
		Left,
		Front,
		Right,
		Back,
		Top,
		Bottom,
		Count
	}

	//[ExecuteInEditMode]
	public class BlendyCameraRig : MonoBehaviour
	{
		public Camera[] cameras;

		[Range(1,360)]
		public float horizon = 180f;

		float _horizon = -1f;

		void Awake ()
		{
		}

		void Update ()
		{
			if (_horizon != horizon)
			{
				_horizon = horizon;

BlendyController.instance.horizon = horizon;

				float a = Lib.Map(horizon,0f,360f,-90f,90f) * -1f;
				Debug.Log("HOR h["+horizon+"] a["+a+"]");


				// Higher then top
				if (a > 45f)
				{
					for (var face = CubemapFace.Left ; face < CubemapFace.Count ; ++face)
						if (face != CubemapFace.Top)
							cameras[(int)face].gameObject.SetActive(false);
				}
				else
				{
					// No Bottom?
					cameras[(int)CubemapFace.Bottom].gameObject.SetActive(a <= -45f);

					a = Mathf.Clamp(a, -45f, 0f);
					float s45 = Mathf.Sin(45f * Mathf.Deg2Rad);
					float op = Mathf.Tan(a * Mathf.Deg2Rad) * s45;
					float y = Lib.Map(op, -s45, s45, 0f, 1.0f);

					float aspect = 1f / (1f - y);
					float shift = Lib.Map(aspect, 1f, 2f, 0f, 1f);

					float fovH = 90f * Mathf.Deg2Rad;
					float fovV = 2f * Mathf.Atan(Mathf.Tan(fovH*0.5f) / aspect);
					fovH *= Mathf.Rad2Deg;
					fovV *= Mathf.Rad2Deg;

					int fboWidth = 1024;
					int fboHeight = (int)((float)fboWidth/aspect);

					Debug.Log("FBO wh["+fboWidth+"/"+fboHeight+"] aspect["+aspect+"] fovH["+fovH+"] fovV["+fovV+"]");

					for (var face = CubemapFace.Left ; face <= CubemapFace.Back ; ++face)
					{
						Camera cam = cameras[(int)face];
						cam.gameObject.SetActive(true);

						Rect r = cam.rect;
						r.y = y;
						r.height = 1f - y;
						cam.rect = r;

						//					ResizeFace(face, fboWidth, fboHeight);

						cam.fieldOfView = fovV;

						cam.ResetAspect();
						cam.ResetProjectionMatrix();

						// https://docs.unity3d.com/Manual/ObliqueFrustum.html
						Matrix4x4 mat  = cam.projectionMatrix;
						//mat[0, 2] = amt;	// horizontal
						mat[1, 2] = shift;	// vertical
						cam.projectionMatrix = mat;

						//Debug.Log("Cam ("+face+") FBO wh ["+fboWidth+"/"+fboHeight+"] aspect ["+cam.aspect+"]");

					}
				}
			}
		}

		void ResizeFace( CubemapFace face, int w, int h )
		{
			Camera cam = cameras[(int)face];
			RenderTexture fbo = new RenderTexture( w, h, 24 );
			cam.targetTexture = fbo;
			BlendyController.instance.SetFaceTexture( face, fbo );
		}
	}
}
