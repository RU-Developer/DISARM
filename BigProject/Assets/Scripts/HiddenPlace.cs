using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenPlace : MonoBehaviour
{
    private Tilemap sprite;
    private void OnEnable()
    {
        sprite = GetComponent<Tilemap>();   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && sprite.color.a>0)
        {
            sprite.color = new Color(255, 255, 255, sprite.color.a - 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && sprite.color.a<255)
        {
            sprite.color = new Color(255, 255, 255, sprite.color.a + 1);
        }
    }
}
