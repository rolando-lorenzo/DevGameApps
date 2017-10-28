using UnityEditor;
using UnityEngine;

public class SystemEditor: EditorWindow{
    private const float WINDOW_MIN_WIDTH = 800.0f;
    private const float WINDOW_MIN_HEIGHT = 640.0f;

    private LanguagesManager managerGlobal;
    private int managerindex;

    [MenuItem("Window/System Editor")]	
    public static void GetWindow()
    {
        SystemEditor window = EditorWindow.GetWindow<SystemEditor>("System", true);
        window.minSize = new Vector2(WINDOW_MIN_WIDTH, WINDOW_MIN_HEIGHT);
    }

    private void OnEnable()
    {
        //load
        managerGlobal = LanguagesManager.Load();
        managerindex = 0;
    }

    private void OnDisable()
    {
        managerGlobal.Save();
        //save
    }

    private void OnGUI()
    {
        //draw stuff
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if(managerGlobal.Translations.Count > 0)
        {
            EditorGUILayout.LabelField(string.Format("Translation {0} out of {1}", managerindex + 1 , managerGlobal.Translations.Count));
            managerGlobal.Translations[managerindex].key = EditorGUILayout.TextField("key:", managerGlobal.Translations[managerindex].key);
            managerGlobal.Translations[managerindex].Default = EditorGUILayout.TextField("Default:", managerGlobal.Translations[managerindex].Default);
            managerGlobal.Translations[managerindex].English = EditorGUILayout.TextField("English:", managerGlobal.Translations[managerindex].English);
            managerGlobal.Translations[managerindex].Spanish = EditorGUILayout.TextField("Spanish:", managerGlobal.Translations[managerindex].Spanish);
            managerGlobal.Translations[managerindex].Dutch = EditorGUILayout.TextField("Dutch:", managerGlobal.Translations[managerindex].Dutch);
            managerGlobal.Translations[managerindex].French = EditorGUILayout.TextField("French:", managerGlobal.Translations[managerindex].French);
            managerGlobal.Translations[managerindex].Italian = EditorGUILayout.TextField("Italian:", managerGlobal.Translations[managerindex].Italian);
            managerGlobal.Translations[managerindex].Mandarin = EditorGUILayout.TextField("Mandarin:", managerGlobal.Translations[managerindex].Mandarin);
            managerGlobal.Translations[managerindex].German = EditorGUILayout.TextField("German:", managerGlobal.Translations[managerindex].German);
        }
        else
        {
            EditorGUILayout.LabelField("No translations Found...");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Save All"))
        {
            EditorGUI.FocusTextInControl(null);
            managerGlobal.Save();
        }
        if (GUILayout.Button("New Entry"))
        {
            EditorGUI.FocusTextInControl(null);

            managerGlobal.Add(new Translation());
            managerindex = managerGlobal.Translations.Count - 1;
        }
        if (GUILayout.Button("Delete Entry"))
        {
            EditorGUI.FocusTextInControl(null);

            managerGlobal.Remove(managerindex);
            managerindex = Mathf.Clamp(managerindex, 0 , managerGlobal.Translations.Count -1 );
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Prev"))
        {
            EditorGUI.FocusTextInControl(null);

            managerindex--;
            if (managerindex < 0)
                managerindex = 0;
        }
        if (GUILayout.Button("Next"))
        {
            EditorGUI.FocusTextInControl(null);

            managerindex++;
            if (managerindex >= managerGlobal.Translations.Count)
                managerindex = managerGlobal.Translations.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
    }
}
