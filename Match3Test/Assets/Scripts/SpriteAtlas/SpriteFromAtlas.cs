using UnityEngine;
using UnityEngine.U2D;

public class SpriteFromAtlas : MonoBehaviour
{
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private string spriteName;
    
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = atlas.GetSprite(spriteName);
    }

}
