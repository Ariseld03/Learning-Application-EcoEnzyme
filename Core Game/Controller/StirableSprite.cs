using UnityEngine;
using System.Collections;


public class StirableSprite : MonoBehaviour
{
    [SerializeField] GameChapter2 game;
    public StirActionType actionType = StirActionType.BuahDalamCairan;

    [Header("Sprite Renderer")]
    public SpriteRenderer buahRenderer;
    public SpriteRenderer cairanRenderer;

    [Header("Sprite Buah & Cairan")]
    public Sprite buahAwal;
    public Sprite buahAkhir;

    public Sprite cairanAwal;
    public Sprite cairanAkhir;

    public float transitionDelay = 0.2f;
    public float sfxCooldown = 0.5f;
    private float lastSFXTime;

    public void RotateSprite(float angle)
    {
        transform.Rotate(0, 0, angle); // Umum

        // Mainkan SFX jika cooldown lewat
        if (Time.time - lastSFXTime > sfxCooldown)
        {            
            game.PlaySFX(game.stirringSFX);
            lastSFXTime = Time.time;
        }
    }

    public void SwitchToFinalState()
    {
        switch (actionType)
        {
            case StirActionType.BuahDalamCairan:
                StartCoroutine(SwitchSpritesSmooth());
                break;
        }
    }

    IEnumerator SwitchSpritesSmooth()
    {
        yield return new WaitForSeconds(transitionDelay);

        if (buahRenderer != null && buahAkhir != null)
            buahRenderer.sprite = buahAkhir;

        if (cairanRenderer != null && cairanAkhir != null)
            cairanRenderer.sprite = cairanAkhir;

        Debug.Log("Buah & cairan berubah.");
    }
}
