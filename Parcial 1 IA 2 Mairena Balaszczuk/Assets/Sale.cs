using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Sale : MonoBehaviour
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ProductoId { get; set; }
    public DateTime date{ get; set; }
    public int cantidad { get; set; }
}
