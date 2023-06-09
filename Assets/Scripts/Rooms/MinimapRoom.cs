using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRoom : MonoBehaviour
{
    [SerializeField] private GameObject roomTypeImage;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color visibleColor;
    [SerializeField] private Color roomEnteredColor;
    [SerializeField] private SpriteRenderer roomMinimap;


    // Start is called before the first frame update
    void Start()
    {
        roomMinimap.color = defaultColor;
    }

    
    public void VisibleRoom()
    {
        roomMinimap.color = visibleColor;
        roomTypeImage.SetActive(true);
    }

    public void EnteredRoom()
    {
        roomTypeImage.SetActive(false);
        roomMinimap.color = roomEnteredColor;
    }


}
