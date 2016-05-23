using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CoinHud : MonoBehaviour
{
    private int _totalCoins;
    private GameObject _body;
    
	void Start ()
    {
        _totalCoins = GetCoins().Length;
        _body = GameObject.Find("Body");
    }
	
	void Update ()
    {
        var coins = GetCoins().Where(coin => coin.GetComponent<MeshRenderer>().enabled);
        if (_body != null)
        {
            if (coins.Count() > 0)
            {
                GameObject closest = coins.OrderBy(coin => (coin.transform.position - _body.transform.position).sqrMagnitude).First();
                float closestDist = (closest.transform.position - _body.transform.position).magnitude;
                GetComponent<Text>().text = string.Format("Coins ({0}/{1}) Next: {2}", _totalCoins - coins.Count(), _totalCoins, closestDist);
            }
            else
            {
                GetComponent<Text>().text = "Epic Win!";
            }
        }
	}

    GameObject[] GetCoins()
    {
        return GameObject.FindGameObjectsWithTag("Coin");
    }
}
