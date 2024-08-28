using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveTextEffect : MonoBehaviour
{
    //Note : TMP_Text 는 TextMeshProUGUI 와 TextMeshPro 의 부모클래스.
    // TextMeshProUGUI -> UI Canvas에서 쓰일 때 사용
    // TextMeshPro -> Canvas 밖, 씬에서 쓰일 때 사용
    [SerializeField] TMP_Text tmpText;
    public float amplitude = 5f;
    public float frequency = 10f;

    private TMP_TextInfo textInfo;

    /**********************************************
    *                 Unity Event
    ***********************************************/

    void Start()
    {
        textInfo = tmpText.textInfo;
    }

    private void OnDisable()
    {
       ResetMesh();
    }

    void Update()
    {
        UpdateMesh();
    }

    /**********************************************
    *                 Mesh Control
    ***********************************************/

    private void UpdateMesh()
    {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.unscaledTime * frequency + orig.x * 0.01f) * amplitude, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private void ResetMesh()
    {
        if (tmpText == null) return;

        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                vertices[charInfo.vertexIndex + j] = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + j];
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}

