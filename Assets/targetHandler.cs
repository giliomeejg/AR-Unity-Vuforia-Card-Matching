using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/* Create a Root Object to store the returned json data in */
[System.Serializable]
public class Quotes  
{
	public Quote[] values;
}

[System.Serializable]
public class Quote
{
    public string package_name;
    public string sum_assured;
    public int base_premium;
    public string suggested_premium;
    public string created_at;
    public string quote_package_id;
	public QuoteModule module;
}

[System.Serializable]
public class QuoteModule
{
    public string type;
    public string make;
    public string model;
}




public class targetHandler : MonoBehaviour {

	public string api_key = "yout api key here";

	// Store the GameObjects for referencing
	// Note that the actual objects are mapped to this array through the Unity interface
	public GameObject[] targets;

	// Use targetQuoted to determine whether a api call has already been made for a specific GameObject
	public IDictionary<string, bool> targetQuoted = new Dictionary<string, bool>();


	// Use this for initialization
	void Start () {
		foreach (GameObject child in targets)
		{	
			// Initialize targetQuoted as false for all GameObjects
			targetQuoted.Add(child.name, false);
		}
		
	}

	// Updates fires at 60fps
	void Update() {
		foreach (GameObject child in targets)
		{

			if (child.GetComponentInChildren<MeshRenderer>().isVisible && !targetQuoted[child.name]) {
				targetQuoted[child.name] = true;
				Debug.Log("Lookup " + child.name);

				if (child.name == "ImageTargetOxygen") {
					// Pretend ImageTargetOxygen is an iPhone 6s 64GB LTE
					String modelNr = "iPhone 6s 64GB LTE";
					// Debug.Log("modelNr " + modelNr);
					
					child.GetComponentInChildren<TextMesh>().text = "Find a quote for " + modelNr;
					CreateQuote(modelNr, child);

				} else if (child.name == "ImageTargetDrone") {
					// Pretend ImageTargetDrone is an iPhone 5
					String modelNr = "iPhone 5";

					child.GetComponentInChildren<TextMesh>().text = "Find a quote for " + modelNr;
					CreateQuote(modelNr, child);
				}
			}
		}
	}
	


	public void CreateQuote(String modelNr, GameObject target) {
		StartCoroutine(CreateQuoteCoroutine(modelNr, target));
  }


  IEnumerator CreateQuoteCoroutine(String modelNr, GameObject target) {

      string auth = api_key + ":";
      auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
      auth = "Basic " + auth;


		WWWForm form = new WWWForm();
        form.AddField("type", "root_gadgets");
        form.AddField("model_name", modelNr);
 
        UnityWebRequest www = UnityWebRequest.Post("https://sandbox.root.co.za/v1/insurance/quotes", form);
		www.SetRequestHeader("AUTHORIZATION", auth);
        yield return www.Send();

      if(www.isNetworkError || www.isHttpError) {
		  target.GetComponentInChildren<TextMesh>().text = www.downloadHandler.text;
          Debug.Log(www.downloadHandler.text);
      }
      else {
		Quotes json = JsonUtility.FromJson<Quotes>("{\"values\":"+www.downloadHandler.text+"}");

		String text = "Make: " + json.values[0].module.make + "\nPremium: R" + (json.values[0].base_premium / 100);
		Debug.Log(modelNr + ": " + json.values[0].module.make);
		target.GetComponentInChildren<TextMesh>().text = text;

		Debug.Log("Form upload complete!");
      }
	yield return true;
  }


}
