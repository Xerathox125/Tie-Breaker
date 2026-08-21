using UnityEngine;

[CreateAssetMenu(fileName = "FragmentoHistoria", menuName = "Juego/Fragmento de Historia")]
public class StoryFragment : ScriptableObject
{
    [TextArea(3, 10)]
    public string textoLore;
    public Sprite imagenFondo;
}