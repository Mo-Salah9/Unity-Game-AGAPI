using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

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
        isAnimating = true;

        transform.DORotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() =>
        {
            cardImage.sprite = frontSprite;
            transform.DORotate(new Vector3(0, 0, 0), 0.15f).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    public void FlipBack()
    {
        if (isAnimating || !IsFlipped || IsMatched) return;

        IsFlipped = false;
        isAnimating = true;

        transform.DORotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() =>
        {
            cardImage.sprite = backSprite;
            transform.DORotate(new Vector3(0, 0, 0), 0.15f).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    public void SetMatched()
    {
        IsMatched = true;
        cardImage.DOFade(0.6f, 0.3f);
    }

    public void SetFlippedState(bool flipped)
    {
        IsFlipped = flipped;
        cardImage.sprite = flipped ? frontSprite : backSprite;
    }

    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClicked);
        transform.DOKill();
    }
}