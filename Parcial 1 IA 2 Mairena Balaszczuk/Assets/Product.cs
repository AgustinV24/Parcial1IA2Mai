using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Product:MonoBehaviour
{
    public int id { get; set; }
    public string nombre;
    public string category { get; set; }
    public int price { get; set; }
    public int cant;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI cantText;
    public List<string> categoryProducts;
    public List<Sprite> productSprite;

    public SpriteRenderer myRend;
    public int totalStock { get; set; }
    public bool onSale;
}
