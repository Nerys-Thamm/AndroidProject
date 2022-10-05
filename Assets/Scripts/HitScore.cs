using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///  Class for displaying a hit effect.
/// </summary>
public class HitScore : MonoBehaviour
{
    public Texture dmgTex, healTex, buffTex, debuffTex; // Textures for the different hit effects
    public Color dmgColor, healColor, buffColor, debuffColor; // Colors for the different hit effects
    public float lifetime = 1f; // How long the hit effect should last
    public AnimationCurve scaleCurve, alphaCurve; // Animation curves for the hit effect
    public float spawnHeight = 1f; // How high the hit effect should spawn
    public float spawnHeightJitter = 0.5f; // How much the hit effect should jitter
    public float spawnHorizontalJitter = 0.5f;  // How much the hit effect should jitter

    [SerializeField] RawImage image;
    [SerializeField] TMP_Text text;

    /// <summary>
    ///  The type of hit effect.
    /// </summary>
    public enum Type
    {
        Damage,
        Heal,
        Buff,
        Debuff
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.position += Vector3.up * (spawnHeight + Random.Range(-spawnHeightJitter, spawnHeightJitter));
        transform.position += Vector3.right * Random.Range(-spawnHorizontalJitter, spawnHorizontalJitter);
        StartCoroutine(Animate());
    }

    /// <summary>
    ///  Sets the hit effect's text.
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        this.text.text = text;
    }

    /// <summary>
    ///  Sets the hit effect's type.
    /// </summary>
    /// <param name="type"></param>
    public void SetType(Type type)
    {
        switch (type)
        {
            case Type.Damage:
                image.texture = dmgTex;
                text.color = dmgColor;
                break;
            case Type.Heal:
                image.texture = healTex;
                text.color = healColor;
                break;
            case Type.Buff:
                image.texture = buffTex;
                text.color = buffColor;
                break;
            case Type.Debuff:
                image.texture = debuffTex;
                text.color = debuffColor;
                break;
        }
    }

    /// <summary>
    ///  Animates the hit effect.
    /// </summary>
    /// <returns></returns>
    IEnumerator Animate()
    {
        float t = 0f;
        while (t < lifetime)
        {
            t += Time.deltaTime;
            float scale = scaleCurve.Evaluate(t / lifetime);
            float alpha = alphaCurve.Evaluate(t / lifetime);
            transform.localScale = Vector3.one * scale;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
