using UnityEditor;
using UnityEngine;
using System;

public static class EnumeratorList {

    public static void Show (SerializedProperty list, Type enumType) {
        EditorGUILayout.PropertyField(list);
        if (list.isArray)
        {
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                for (int i = 0; i < list.arraySize; i++)
                {
                    // Use enum name as label
					string label = i.ToString () + ": " + GetEnumNameByValue(enumType, i);
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i),new GUIContent(label),true);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }

	static string GetEnumNameByValue(Type enumType, int value)
	{
		foreach (var v in Enum.GetValues(enumType))
			if ( (int)v == value )
				return v.ToString();
		return "???";
	}
}
