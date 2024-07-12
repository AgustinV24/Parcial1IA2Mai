using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Client : MonoBehaviour
{
    public int dni;
    public string nombre;

    public List<Product> purchases;
    public int totalPurchases;
    public int money;

    public Transform bag;
}
