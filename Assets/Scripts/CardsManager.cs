using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardsManager : MonoBehaviour
{

    [SerializeField] private GameObject PrefabCards;
    [SerializeField] private GameObject Hand, StartPointReferenceToShow, EndPointReferenceToShow, Canvas, ButtonForNewValues;
    [SerializeField] private float VelocityToShowCard,VelocityToPutInHandCard,ScaleToShow,ScaleToPutInHand;

    //Cache---------------------------
    int PassArrayInitialAnimation = 0;
    int NumberInitialCardsCalculated = 0;
    int PassRegenerateValueArray = 0;
    List<CardBrain> CardsHand = new List<CardBrain>();
    bool AvailableRandomize = true;

    private void Start()
    {
        PassArrayInitialAnimation = 0;

        NumberInitialCardsCalculated = Random.RandomRange(4, 7);
        SetInitialAnimationCards();
    }


    private void SetInitialAnimationCards()
    {
        if (PassArrayInitialAnimation > NumberInitialCardsCalculated)
        {
            ButtonForNewValues.transform.DOScale(1, 1).SetEase(Ease.OutBounce);
            return;
        }
        var ObjectCard = Instantiate(PrefabCards, Canvas.transform);
        ObjectCard.transform.position = StartPointReferenceToShow.transform.position;
        ObjectCard.transform.DOMove(EndPointReferenceToShow.transform.position, VelocityToShowCard).SetEase(Ease.InOutCirc).OnComplete(() =>
        {
            ObjectCard.transform.DOMove(Hand.transform.position, VelocityToPutInHandCard).SetEase(Ease.InOutCirc);

        }
        );
        ObjectCard.transform.DOScale(ScaleToShow, VelocityToShowCard).SetEase(Ease.InOutCirc).OnComplete(() =>
        {
           
            ObjectCard.transform.DOScale(ScaleToPutInHand, VelocityToPutInHandCard).SetEase(Ease.InOutCirc).OnComplete(()=> {
                var CardBrain = ObjectCard.GetComponent<CardBrain>();
                CardBrain.SetHandThisCard(Hand);
                CardsHand.Add(CardBrain);
            });
            PassArrayInitialAnimation = PassArrayInitialAnimation + 1;
            SetInitialAnimationCards();

        }
        );
    }

    public void RandomizeCardValues()
    {
        if (AvailableRandomize)
        {

            AvailableRandomize = false;
            PassRegenerateValueArray = NumberInitialCardsCalculated;
            HoldCards();
            TriggerRandomizeValuesToCards();
        }

    }

    private void HoldCards()
    {
        foreach (var card in CardsHand)
        {
            if (card)
            {
                card.HoldingCard();
            }
            

        }
    }

    private void TriggerRandomizeValuesToCards()
    {
        var IsTheLastCard = false;
       

        if (PassRegenerateValueArray< 0)
        {
            return;
        }
        if (PassRegenerateValueArray == 0)
        {
            IsTheLastCard = true;
        }
        if (CardsHand[PassRegenerateValueArray])
        {
            CardsHand[PassRegenerateValueArray].RegenerateValues(TriggerRandomizeValuesToCards, IsTheLastCard, ResetHand);
        }
        else
        {
            PassRegenerateValueArray = PassRegenerateValueArray - 1;
            TriggerRandomizeValuesToCards();
            return;
        }
        
        PassRegenerateValueArray = PassRegenerateValueArray - 1;
    }

    private void ResetHand()
    {

        foreach (var card in CardsHand)
        {
            if (card)
            {
                card.ResetHolding();
            }
           

        }
        AvailableRandomize = true;
    }
}
