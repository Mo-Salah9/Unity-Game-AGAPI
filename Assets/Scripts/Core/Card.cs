using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Card : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;

    public event Action<Card> OnCardFlipped;

    public int CardId { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsMatched { get; private set; }

    private Sprite frontSprite;
    private Sprite backSprite;
    private bool isAnimating;

    private void Awake()
    {
        if (cardButton == null)
            cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardClicked);
    }

    public void Initialize(int id, Sprite front, Sprite back)
    {
        CardId = id;
        frontSprite = front;
        backSprite = back;
        cardImage.sprite = backSprite;
        IsFlipped = false;
        IsMatched = false;
        isAnimating = false;
    }

    private void OnCardClicked()
    {
        if (!IsFlipped && !IsMatched && !isAnimating)
        {
            OnCardFlipped?.Invoke(this);
        }
    }

    public void Flip()
    {
        if (isAnimating || IsFlipped) return;
        IsFlipped = true;
        StartCoroutine(FlipAnimation(frontSprite));
    }

    public void FlipBack()
    {
        if (isAnimating || !IsFlipped || IsMatched) return;
        IsFlipped = false;
        StartCoroutine(FlipAnimation(backSprite));
    }

    private IEnumerator FlipAnimation(Sprite targetSprite)
    {
        isAnimating = true;
        float duration = 0.15f;
        float elapsed = 0f;

        Vector3 startRotation = transform.eulerAngles;
        Vector3 midRotation = new Vector3(0, 90, 0);

        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.eulerAngles = Vector3.Lerp(startRotation, midRotation, t);
            yield return null;
        }

        transform.eulerAngles = midRotation;

        
        cardImage.sprite = targetSprite;

        
        elapsed = 0f;
        Vector3 endRotation = new Vector3(0, 0, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.eulerAngles = Vector3.Lerp(midRotation, endRotation, t);
            yield return null;
        }

        transform.eulerAngles = endRotation;
        isAnimating = false;
    }

    public void SetMatched()
    {
        IsMatched = true;
        StartCoroutine(FadeAnimation(0.6f, 0.3f));
    }

    private IEnumerator FadeAnimation(float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color startColor = cardImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cardImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        cardImage.color = endColor;
    }

    public void SetFlippedState(bool flipped)
    {
        IsFlipped = flipped;
        cardImage.sprite = flipped ? frontSprite : backSprite;
    }

    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClicked);
        StopAllCoroutines();
    }
}