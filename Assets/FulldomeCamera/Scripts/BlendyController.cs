using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

namespace Blendy
{
	//[ExecuteInEditMode]
	public class BlendyController : MonoBehaviour
	{
		public enum SyphonResolution
		{
			Disabled = 0,
			_720 = 720,
			_768 = 768,
			_1080 = 1080,
			_1024 = 1024,
			_1200 = 1200,
			_1536 = 1536,
			_2048 = 2048,
			_2560 = 2560,
			_3072 = 3072,
			_3584 = 3594,
			_4096 = 4096,
		};

		public BlendyCameraRig cameraRig;
		public SyphonResolution screenResolution = SyphonResolution._720;
		public SyphonResolution syphonResolution = SyphonResolution._2048;
		[Range(1,360)]
		public float horizon = 180f;
		[Range(0,1)]
		public float grid = 0f;
		public bool displayDebugLayer = true;
		public bool displayCubemaps = true;

		Funnel.Funnel _funnel;
		Transform _debugCanvas;
		Transform _cubemaps;
		ShFisheye _shader;

		static BlendyController _instance;
		public static BlendyController instance { get { return _instance; } }

		void Awake ()
		{
			_instance = this;

			// Make the game run even when in background
			Application.runInBackground = true;

			// Get first rig
			if (cameraRig == null)
				cameraRig = FindObjectOfType<BlendyCameraRig>();
			Assert.IsTrue(cameraRig != null, "BlendyController needs at least one BlendyCameraRig in your scene. Drop the prefab!");

			// GetShader
			_shader = GetComponentInChildren<ShFisheye>();
			Assert.IsTrue(_shader != null);

			// Get Funnel
			_funnel = GetComponentInChildren<Funnel.Funnel>();
			if (_funnel != null && _funnel.GetType().GetProperty("isDummy") != null)
			{
				Debug.Log("Funnel not installed!");
				_funnel = null;
			}
			if (_funnel != null)
			{
				if (syphonResolution == SyphonResolution.Disabled)
				{
					_funnel.enabled = false;
				}
				else
				{
					_funnel.screenWidth = (int)syphonResolution;
					_funnel.screenHeight = (int)syphonResolution;
				}
			}

			// Get Debug layer
			Canvas canvas = GetComponentInChildren<Canvas>();
			if (canvas != null)
			{
				_debugCanvas = canvas.transform;
				_cubemaps = _debugCanvas.Find("Cubemaps");
			}
		}

		void Start ()
		{
			// Fixed window resolution
			if (_funnel != null && !Application.isEditor)
				Screen.SetResolution( (int)screenResolution, (int)screenResolution, false );
		}

		void Update ()
		{
			if (_debugCanvas != null)
				_debugCanvas.gameObject.SetActive(displayDebugLayer);
			if (_cubemaps != null)
				_cubemaps.gameObject.SetActive(displayCubemaps);
			
			cameraRig.horizon = horizon;

			_shader.SetHorizon(horizon);
			_shader.SetGrid(grid);
		}

		public void SetFaceTexture( CubemapFace face, RenderTexture fbo )
		{
			_shader.SetFaceTexture(face, fbo);
			if (_cubemaps != null)
			{
				RawImage[] imgs = _cubemaps.GetComponentsInChildren<RawImage>();
				if ((int)face < imgs.Length)
					imgs[(int)face].texture = fbo;
			}
		}
	}
}
