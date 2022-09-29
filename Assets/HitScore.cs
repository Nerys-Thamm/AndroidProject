using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitScore : MonoBehaviour
{
    public Texture dmgTex, healTex, buffTex, debuffTex;
    public Color dmgColor, healColor, buffColor, debuffColor;
    public float lifetime = 1f;
    public AnimationCurve scaleCurve, alphaCurve;
    public float spawnHeight = 1f;
    public float spawnHeightJitter = 0.5f;
    public float spawnHorizontalJitter = 0.5f;

    [SerializeField] RawImage image;
    [SerializeField] TMP_Text text;

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

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

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
