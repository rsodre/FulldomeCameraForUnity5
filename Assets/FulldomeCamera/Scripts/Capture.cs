using UnityEngine;
using System.Collections;
using System.IO;
using Blendy;

public class Capture : MonoBehaviour
{
	public string path = "";
	public string filePrefix = "Capture";
	public int framerate = 30;
	public float totalSecods = 1;
	public bool capturing = false;

	Funnel.Funnel _funnel;
	Texture2D _tex;

	bool _captureNow;
	int _frameNumber;
	int _w;
	int _h;

	void Awake()
	{
		if (capturing)
			Time.captureFramerate = framerate;
	}

	void Start ()
	{
		_funnel = FindObjectOfType<Funnel.Funnel>();
		_w = _funnel.screenWidth;
		_h = _funnel.screenHeight;
		_tex = new Texture2D(_w, _h);
	}
	
	void Update ()
	{
		if (capturing)
		{
			if (Time.time > totalSecods)
			{
				capturing = false;
				Debug.Log("Finished Capture!!!");
			}
			else
				_captureNow = true;
		}
		else if (Input.GetKeyDown (KeyCode.Space))
			_captureNow = true;
	}

	void OnPostRender()
	{
		if (_captureNow)
		{
			_tex.ReadPixels(new Rect(0, 0, _w, _h), 0, 0);
			_tex.Apply();

			//string pathName = Application.dataPath + "../CAPTURE/";
//			string pathName = "/Volumes/HDD/CAPTURE/";
			string pathName = path;
			string fileName = filePrefix + "_" + Lib.ToStringZeroes(_frameNumber,4) + ".png";
			string fullName = pathName+fileName;
			byte[] bytes = _tex.EncodeToPNG();
			File.WriteAllBytes(fullName, bytes);

			Debug.Log("Captured ["+fullName+"]");

			_frameNumber++;
			_captureNow = false;
		}
	}

}
