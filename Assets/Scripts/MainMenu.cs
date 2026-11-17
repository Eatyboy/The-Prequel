using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Vector2 unhoveredLerpRange;
    [SerializeField] private Vector2 hoveredLerpRange;
    [SerializeField] private Vector2 speedRange;
    [SerializeField] private Button rhombus;
    public void StartGame()
    {
        Persistent.GoToGame();
    }

    public void Hover(bool isHovered)
    {
        Vector2 hoverRange = isHovered ? hoveredLerpRange : unhoveredLerpRange;
        rhombus.GetComponent<Image>().material.SetFloat("_Min", hoverRange.x);
        rhombus.GetComponent<Image>().material.SetFloat("_Max", hoverRange.y);
        rhombus.GetComponent<Image>().material.SetFloat("_Speed", isHovered ? speedRange.y : speedRange.x);
    }
}
