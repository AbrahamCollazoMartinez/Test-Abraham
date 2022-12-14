using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class CardBrain : MonoBehaviour
{
    [Header("Components Needed")]
    [SerializeField] private RawImage ImageCard;
    [SerializeField] private TMP_Text Atack_Text;
    [SerializeField] private TMP_Text HL_Text;
    [SerializeField] private TMP_Text Mana_Text;
    [SerializeField] private TMP_Text Name_Text;
    [SerializeField] private GameObject CardObject;
    [Header("Data")]
    [SerializeField] private int Atack = 0;
    [SerializeField] private int HL = 0;
    [SerializeField] private int Mana = 0;
    [SerializeField] private int SpeedToUpdateValues = 2;

    //Cache--------------------
    private int Atack_Cache = 0;
    private int HL_Cache = 0;
    private int Mana_Cache = 0;

    bool PreventHang_Atack = false;
    bool PreventHang_HL = false;
    bool PreventHang_Mana = false;


    async void Start()
    {
        await RandomInitImage();
        RandomValues();

        NamesGeneratorManager.UpdateName?.Invoke(SetName);
    }

    public void SetName(string newName)
    {
        Name_Text.text = newName;

    }

    private async Task RandomInitImage()
    {
        var _texture = await ImageLoader.GetRemoteTexture("https://picsum.photos/300/200");
        ImageCard.texture = _texture;
    }

    private void RandomValues()
    {
        Atack = GetRandomValues();
        HL = GetRandomValues();
        Mana = GetRandomValues();
    }

    private int GetRandomValues()
    {
        var RandomValue = UnityEngine.Random.RandomRange(-2, 10);
        return RandomValue;

    }

    private void Update()
    {
        DetectChangesInValues();
    }

    private void DetectChangesInValues()
    {
        if (Atack_Cache != Atack && !PreventHang_Atack)
        {
            PreventHang_Atack = true;
            UpdateValue(Atack_Cache, Atack, true, false, false, Atack_Text);
        }
        else if (HL_Cache != HL && !PreventHang_HL)
        {
            PreventHang_HL = true;
            UpdateValue(HL_Cache, HL, false, true, false, HL_Text);
        }
        else if (Mana_Cache != Mana && !PreventHang_Mana)
        {

            PreventHang_Mana = true;
            UpdateValue(Mana_Cache, Mana, false, false, true, Mana_Text);
        }
    }

    private void UpdateValue(int oldNumber, int newNumber, bool IsAtack, bool IsHL, bool IsMana, TMP_Text Number)
    {

        //detect when value change during interpolation
        var DetectChanges = oldNumber;

        //interpolates value
        DOTween.To(() => oldNumber, x => oldNumber = x, newNumber, SpeedToUpdateValues)
    .OnUpdate(() =>
    {
        if (DetectChanges != oldNumber)
        {
            DetectChanges = oldNumber;

                        //Assign new value
                        Number.text = oldNumber.ToString();

                        //Animation every time text updates
                        Number.gameObject.transform.DOScale(1.5f, 0.2f).OnComplete(() =>
            {

                Number.gameObject.transform.DOScale(1f, 0.2f);

            });

        }


    }).OnComplete(() =>
    {

                    //Prevent Multiple calls from update
                    if (IsAtack)
        {
            Atack_Cache = DetectChanges;
            PreventHang_Atack = false;
        }
        else if (IsHL)
        {
            HL_Cache = DetectChanges;
            PreventHang_HL = false;
        }
        else if (IsMana)
        {
            Mana_Cache = DetectChanges;
            PreventHang_Mana = false;
        }


    });

    }

    public void SetHandThisCard(GameObject Hand)
    {

        this.gameObject.transform.SetParent(Hand.transform);
        CardObject.transform.Rotate(0, 0, -90);
    }

    public void RegenerateValues(Action NextCard, bool IsTheLastCard,Action ResetHand)
    {
        CardObject.transform.DOScale(1, 1).SetEase(Ease.InOutQuint).OnComplete(()=> {

            RandomValues();

            if (HL <1)
            {
                CardObject.transform.DOScale(1.2f, 2f).SetEase(Ease.InOutQuint).OnComplete(() => {

                    CardObject.transform.DOScale(0, 0.5f).SetEase(Ease.InOutQuint).OnComplete(()=> {
                        NextCard?.Invoke();
                        if (IsTheLastCard)
                        {
                            ResetHand?.Invoke();
                        }

                        Destroy(this.gameObject);
                    });
                    

                });
            }
            else
            {
                CardObject.transform.DOScale(1.2f, 2f).SetEase(Ease.InOutQuint).OnComplete(() => {

                    HoldingCard();
                    NextCard?.Invoke();
                    if (IsTheLastCard)
                    {
                        ResetHand?.Invoke();
                    }

                });

            }

           
        });

    }

    public void HoldingCard()
    {
        CardObject.transform.DOScale(0.3f, 1).SetEase(Ease.InOutQuint);

    }
    public void ResetHolding()
    {
        CardObject.transform.DOScale(1, 1).SetEase(Ease.InOutQuint);

    }
}
