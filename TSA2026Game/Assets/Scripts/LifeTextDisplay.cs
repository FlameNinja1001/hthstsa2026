using TMPro;
using UnityEngine;

public class LifeTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    void Start()
    {
        tmpText.text = "x0" + SceneLoadManager.lives;
    }
}