using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public int dni { get; set; }
    public string nombre { get; set; }
    public string correo { get; set; }

    public List<Product> purschases;
    public int money;

    public Transform bag;
}
