using UnityEngine;
using UnityEditor;

public class GenericEditor : Editor
{

	private SerializedObject    so;
	private SerializedProperty  prop;

	public virtual void OnEnable()
	{
		so = new SerializedObject(target);
	}

	override public void OnInspectorGUI() 
	{
		so.Update();
		prop = so.GetIterator();
		while ( prop.NextVisible(true) )
		{
			// Draw only first depth. All others are drawn as PropertyField children
			if ( prop.depth != 0 )
				continue;
			if ( ! AddProperty(prop) )
				// Draw default
				EditorGUILayout.PropertyField(prop, true);
		}

		if (UnityEngine.GUI.changed)
		{
			so.ApplyModifiedProperties();
		}
	}

	virtual protected bool AddProperty(SerializedProperty prop)
	{
		return false;
	}
}
