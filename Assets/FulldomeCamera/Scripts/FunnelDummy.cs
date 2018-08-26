//
// Remove this file after installing Fubbel from:
// https://github.com/keijiro/Funnel
//
using UnityEngine;

namespace FunnelDummy
{
	public class FunnelDummy : MonoBehaviour
    {
		public int screenWidth;
		public int screenHeight;
		public int antiAliasing;
		public bool discardAlpha;
		public RenderMode renderMode;
        public enum RenderMode { SendOnly, RenderToTarget, PreviewOnGameView }
		bool dummy { get { return true; } }
    }
}
