using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Blendy
{
	public class ShFisheye : MonoBehaviour
	{
		Material _material;

		void Start ()
		{
			// Find material in mesh renderers
			MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
			Assert.IsTrue(mesh != null);
			_material = mesh.material;
		}
		
		void Update ()
		{
			if (_material == null)
				return;

	//		if (texMain != AnimTex.None)
	//			_material.SetTexture("_MainTex", ShaderManager.Instance.GetTexture(texMain) );
	//		if (texMask != AnimTex.None)
	//			_material.SetTexture("_MaskTex", ShaderManager.Instance.GetTexture(texMask) );
	//		if (texFill != AnimTex.None)
	//			_material.SetTexture("_FillTex", ShaderManager.Instance.GetTexture(texFill) );
	//		_material.SetFloat("_FillIntensity", texFill != AnimTex.None ? fillIntensity : 0f );
	//		_material.SetColor("_InlineColor", inlineColor );
		}

		public void SetHorizon(float h)
		{
			_material.SetFloat("_Horizon", h );
		}

		public void SetGrid(float g)
		{
			_material.SetFloat("_Grid", g );
		}

		public void SetFaceTexture(Blendy.CubemapFace face, RenderTexture fbo)
		{
			if (_material != null)
			{
				string propName = "_"+face.ToString()+"Tex";
				_material.SetTexture(propName, fbo );
			}
		}
	}
}