using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Serialization;

public class Store : MonoBehaviour
{
    public List<Product> productos = new List<Product>();
    public List<Product> productPrfabs;
    public List<Client> clientes = new List<Client>();
    public List<Product> shownProducts;
    public Client currentClient;
    public List<string> currentCategories = new List<string>();
    int lastProductId = 0;

    public Transform firstPos;

    public TextMeshProUGUI text;
    public GameObject endCanvas;
    public GameObject mainCanvas;
    public TextMeshProUGUI mostSoldProduct;
    public TextMeshProUGUI totalRevenue;
    
    private int _clientIndex = 0;
    
    private void Start()
    {
        CreateProducts();
        currentClient = GetCurrentClient();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                Mathf.Infinity);
            if (rayHit.collider != null && rayHit.transform.TryGetComponent<Product>(out Product prod))
            {
                BuyProduct(prod);
            }
        }
    }

    public void UpdateUI()
    {
        foreach (var item in productos)
        {
            if (!shownProducts.Contains(item))
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < shownProducts.Count(); i++)
        {
            if (i >= 20 / 2)
            {
                shownProducts[i].transform.position =
                    firstPos.position + Vector3.right * (i - 20 / 2) * 2 - Vector3.up * 2;
            }
            else
            {
                shownProducts[i].transform.position = firstPos.position + Vector3.right * i * 2;
            }
        }
    }

    public void CreateProducts()
    {
        foreach (var item in productPrfabs)
        {
            for (int i = 0; i < item.categoryProducts.Count(); i++)
            {
                Product p = Instantiate(item);

                p.id = lastProductId + 1;
                lastProductId = p.id;
                p.nombre = item.categoryProducts[i];
                p.name = item.categoryProducts[i];
                p.myRend.sprite = item.productSprite[i];
                p.cant = 1;
                p.cantText.text = "1";
                p.totalStock = 1;
                p.category = item.category;
                int rand = UnityEngine.Random.Range(100, 1000);
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    p.onSale = true;
                    rand /= 2;
                }

                p.price = rand;
                p.priceText.text = "$ " + rand.ToString();
                productos.Add(p);
            }

            for (int i = 0; i < 20; i++)
            {
                string prodName = item.categoryProducts[UnityEngine.Random.Range(0, item.categoryProducts.Count())];


                productos.Where(x => x.nombre == prodName).First().cant++;
                productos.Where(x => x.nombre == prodName).First().cantText.text =
                    productos.Where(x => x.nombre == prodName).First().cant.ToString();

                productos.Where(x => x.nombre == prodName).First().totalStock++;
            }
        }

        shownProducts = productos;

        UpdateUI();
    }

    public void BuyProduct(Product product)
    {
        if (productos.Contains(product) && productos.Find(x => x == product).cant > 0 &&
            productos.Find(x => x == product).price < currentClient.money)
        {
            productos.Find(x => x == product).cant -= 1;
            productos.Find(x => x == product).cantText.text = productos.Find(x => x == product).cant.ToString();
            currentClient.money -= productos.Find(x => x == product).price;
            if (currentClient.purchases.Where(x => x.nombre == product.nombre).Any())
            {
                currentClient.purchases.Find(x => x.nombre == product.nombre).cant += 1;
                currentClient.totalPurchases++;
            }
            else
            {
                Product newProd = Instantiate(product, currentClient.bag.position, Quaternion.identity);
                newProd.cant = 1;
                currentClient.purchases.Add(newProd);
                currentClient.totalPurchases++;
            }
        }

        if (currentClient.totalPurchases >= 3 && _clientIndex < clientes.Count - 1)
            NextClient();
        else if (_clientIndex >= clientes.Count - 1)
            ShowEndScreen();
    }

    private void ShowEndScreen()
    {
        var productos = FindObjectsOfType<Product>();
        foreach (var producto in productos)
        {
            producto.gameObject.SetActive(false);
        }
        mainCanvas.SetActive(false);
        endCanvas.SetActive(true);
        mostSoldProduct.text = MostSoldProduct();
        totalRevenue.text = CalculateDailyRevenue().ToString();
    }

    public void NextClient()
    {
        _clientIndex++;
        currentClient = clientes[_clientIndex];
        text.text = "Cliente: " + currentClient.nombre + "       DNI: " + currentClient.dni;
    }

    public Client GetCurrentClient()
    {
        clientes = clientes.OrderBy(x => x.nombre).ThenBy(x => x.dni).ToList();
        var client = clientes.First();
        text.text = "Cliente: " + client.nombre + "       DNI: " + client.dni;
        return client;
    }
    
    public void OrdeyByPrice()
    {
        shownProducts = OrderByPrice();
        UpdateUI();
    }

    public List<Product> OrderByPrice()
    {
        return shownProducts.OrderBy(x => x.price).ToList();
    }

    public string MostSoldProduct()
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        var col = clientes.SelectMany(x => x.purchases).Select(x => new { nombre = x.nombre, cant = x.cant }).Aggregate(
            dic, (acum, current) =>
            {
                if (acum.ContainsKey(current.nombre))
                {
                    acum[current.nombre] += current.cant;
                }
                else
                {
                    acum.Add(current.nombre, current.cant);
                }

                return acum;
            });

        string greatest = "";
        int maxCant = 0;
        foreach (var item in col)
        {
            if (item.Value > maxCant)
            {
                maxCant = item.Value;
                greatest = item.Key;
            }
        }
        return greatest;
    }
    
    public void ChangeShownProducts()
    {
        shownProducts = ObtenerProductosPorCategoria(currentCategories);
        UpdateUI();
    }

    public void AddCategory(string cat)
    {
        currentCategories.Add(cat);
        if (cat == "None")
        {
            currentCategories.Clear();
        }

        ChangeShownProducts();
    }
    
    public List<Product> ObtenerProductosPorCategoria(List<string> categoria)
    {
        List<Product> sProducts = new List<Product>();
        if (categoria.Contains("None"))
        {
            return null;
        }
        else if (categoria.Contains("All"))
        {
            currentCategories.Clear();
            return productos;
        }

        if (categoria.Contains("Electronicos"))
        {
            sProducts = productos.OfType<Electronicos>().OrderBy(x => x.nombre).Concat(sProducts).ToList();
        }

        if (categoria.Contains("Hogar"))
        {
            sProducts = productos.OfType<Hogar>().OrderBy(x => x.nombre).Concat(sProducts).ToList();
        }

        if (categoria.Contains("Alimentos"))
        {
            sProducts = productos.OfType<Alimentos>().OrderBy(x => x.nombre).Concat(sProducts).ToList();
        }

        if (categoria.Contains("Limpieza"))
        {
            sProducts = productos.OfType<Limpieza>().OrderBy(x => x.nombre).Concat(sProducts).ToList();
        }

        if (categoria.Contains("Mascotas"))
        {
            sProducts = productos.OfType<Mascotas>().OrderBy(x => x.nombre).Concat(sProducts).ToList();
        }

        return sProducts;
    }


    public void HighlightPurchasableObjects()
    {
        shownProducts = HighlightPurchasableProducts();
        UpdateUI();
    }

    public List<Product> HighlightPurchasableProducts()
    {
        return productos.Where(x => x.price < currentClient.money && x.cant > 0).OrderBy(x => x.id).ToList();
    }

    public float CalculateDailyRevenue()
    {
        return productos.Select(x => new { price = x.price, cant = x.totalStock - x.cant })
            .Aggregate(0f, (acum, current) => { return acum + current.price * current.cant; });
    }

    public void HighlightSaleObjects()
    {
        shownProducts = HightlightOnSaleProducts();
        UpdateUI();
    }

    public List<Product> HightlightOnSaleProducts()
    {
        return productos.Where(x => x.onSale).ToList();
    }

    public void GetCheapProductsVoid()
    {
        shownProducts = GetCheapProducts();
        UpdateUI();
    }

    public List<Product> GetCheapProducts()
    {
        return productos.OrderBy(x => x.price).Take(3).ToList();
    }

    public void GetExpensiveProductsVoid()
    {
        shownProducts = GetExpensiveProducts();
        UpdateUI();
    }

    public List<Product> GetExpensiveProducts()
    {
        return productos.OrderByDescending(x => x.price).TakeWhile(x => x.price >= 600).ToList();
    }
}