using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Blendy.BlendyCameraRig))]
public class BlendyCameraRigEditor : GenericEditor
{
	protected override bool AddProperty(SerializedProperty prop)
	{
		if ( prop.name == "cameras" )
		{
			EnumeratorList.Show( prop, typeof(Blendy.CubemapFace) );
			return true;
		}
		return false;
	}
}
