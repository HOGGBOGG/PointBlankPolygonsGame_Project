using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image ForegroundImage;
    public TextMeshProUGUI healthText;

    public Health health;
    public void SetHealthBarPercentage(float dmg) // SIMILAR SIGNAture TO THAT OF GAME EVENT
    {
        //Debug.Log("Setting health bar percentage");
        float percentage = (health.CurrentHealth * 1.0f) / health.MaxHealth;
        float ParentWidth = GetComponent<RectTransform>().rect.width;
        float width = ParentWidth * percentage;
        ForegroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        healthText.text = ((int)health.CurrentHealth).ToString();
    }

    public void Start()
    {
        healthText.text = health.CurrentHealth.ToString();
        health.OnTakeDamage += SetHealthBarPercentage;
        health.InvokeOnTakeDamage();
    }

}
