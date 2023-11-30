using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnackManager : MonoBehaviour
{
    static SnackManager Singleton { get; set; }

    public GameObject SnackPrefab;

    private class SnackItem
    {
        public TMPro.TMP_Text Text;
        public float TimeDestroy = float.MaxValue;
    }

    List<SnackItem> items = new List<SnackItem>();

    void Awake()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (Time.time > item.TimeDestroy)
            {
                Destroy(item.Text.gameObject);
                items.RemoveAt(i);
                --i;
            }
        }
    }

    public static void AddSnack(string text)
    {
        if (!Singleton)
            return;

        var go = Instantiate(Singleton.SnackPrefab, Singleton.transform);
        var element = go.GetComponent<TMPro.TMP_Text>();
        element.text = text;

        Singleton.items.Add(new SnackItem()
        {
            Text = element,
            TimeDestroy = Time.time + 3f,
        });
    }
}
