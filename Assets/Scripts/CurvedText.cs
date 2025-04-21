using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextArch : MonoBehaviour
{
    public TMP_Text textComponent; // Drag your TextMeshPro object here in the inspector
    public float archRadius = 200f;

    void Update()
    {
        if (textComponent == null)
        {
            Debug.LogWarning("TextArch: No TMP_Text assigned!");
            return;
        }

        textComponent.ForceMeshUpdate();

        var textInfo = textComponent.textInfo;
        if (textInfo.characterCount == 0) return;

        Mesh mesh = textComponent.mesh;
        Vector3[] vertices = mesh.vertices;

        float centerX = textComponent.bounds.center.x;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            for (int j = 0; j < 4; j++)
            {
                int index = charInfo.vertexIndex + j;
                Vector3 orig = vertices[index];
                float x = orig.x - centerX;

                float angle = x / archRadius;
                float yOffset = -Mathf.Cos(angle) * archRadius + archRadius;

                vertices[index] = new Vector3(orig.x, orig.y + yOffset, orig.z);
            }
        }

        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }
}
