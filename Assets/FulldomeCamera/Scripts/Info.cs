using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Blendy
{
	public class Info : MonoBehaviour
	{
		public Text InfoText;
		public Text FrameRateText;
		public float FrameRateRefreshSeconds = 0.5f;

		void Start ()
		{
			if (InfoText != null)
			{
				BlendyController blendy = GetComponentInParent<BlendyController>();
				if (blendy != null)
					InfoText.text = "Screen: "+blendy.screenResolution+"\nSyphon: "+blendy.syphonResolution;
			}
		}

		void Update ()
		{
			if (FrameRateText != null)
			{
				CalcFramerate();
				FrameRateText.text = ((int)m_lastFramerate).ToString();
			}
		}

		//
		// Framerate
		// http://answers.unity3d.com/questions/46745/how-do-i-find-the-frames-per-second-of-my-game.html
		int m_frameCounter = 0;
		float m_timeCounter = 0.0f;
		float m_lastFramerate = 0.0f;
		void CalcFramerate()
		{
			if( m_timeCounter < FrameRateRefreshSeconds )
			{
				m_timeCounter += Time.deltaTime;
				m_frameCounter++;
			}
			else
			{
				//This code will break if you set your m_refreshTime to 0, which makes no sense.
				m_lastFramerate = (float)m_frameCounter/m_timeCounter;
				m_frameCounter = 0;
				m_timeCounter = 0.0f;
			}
		}
	}
}
