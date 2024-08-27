using FPSTemplate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseManager
{
    GameObject uiBlackBackground;
    GameObject uiGameOver;
    GameObject uiCrosshair;
    GameObject uiTips;
    GameObject uiRodsListing;
    GameObject uiObjective;

    TextMeshProUGUI tipText;

    public class UIRod
    {
        TextMeshProUGUI rodName;
        Slider slider;

        private int hp;
        public int hpMax { get; private set; }

        public int Hp
        {
            get => hp;
            set
            {
                hp = value;

                if (hp <0 ) hp = 0;

                if (hpMax == 0)
                    return;

                slider.value = (float)hp / hpMax;

                if (hp < 1)
                    hideBar();        
            }
        }

        private void hideBar()
        {
            var go = slider.GetComponent<Transform>().gameObject;
            go.SetActive(false);

            rodName.color = new Color(0.56f, 0.1f, 0.14f, 1f);
        }

        public UIRod(string name, int baseHP, string TMPName, string sliderName)
        {
            rodName = GameObject.Find(TMPName).GetComponent<TextMeshProUGUI>();

            slider = GameObject.Find(sliderName).GetComponent<Slider>();

            hp = baseHP;
            hpMax = baseHP;

            rodName.text = "Rod: " + name;
            slider.value = 1f;
        }
    }

    public UIRod[] RodList;

    public bool RodsDetroyed()
    {
        for(int i = 0; i < RodList.Length; i++)
        {
            if (RodList[i].Hp > 0)
                return false;
        }

        return true;
    }
   
    public override void Init()
    {
        // Rods
        RodList = new UIRod[5];

        RodList[0] = new UIRod("Alpha", 100, "RodName1", "RodHP1");
        RodList[1] = new UIRod("Beta", 100, "RodName2", "RodHP2");
        RodList[2] = new UIRod("Gamma", 100, "RodName3", "RodHP3");
        RodList[3] = new UIRod("Delta", 100, "RodName4", "RodHP4");
        RodList[4] = new UIRod("Epsilon", 100, "RodName5", "RodHP5");

        // UI
        uiBlackBackground = GameObject.Find("BlackBackground");
        uiGameOver = GameObject.Find("GameOver");

        uiBlackBackground.SetActive(false);
        uiGameOver.SetActive(false);


        uiCrosshair = GameObject.Find("CrossHair");
        uiTips = GameObject.Find("Tips");
        uiRodsListing = GameObject.Find("RodList");
        uiObjective = GameObject.Find("Objective");

        tipText = uiTips.GetComponent<TextMeshProUGUI>();
    }

    public void HideObjective()
    {
        //uiTips.SetActive(false);
        uiObjective.SetActive(false);
    }

    public void ShowGameOver()
    {
        uiBlackBackground.SetActive(true);
        uiGameOver.SetActive(true);
        uiCrosshair.SetActive(false);
        uiRodsListing.SetActive(false);
        
        //uiTips.SetActive(true);
        tipText.text = "Press enter to return";
    }

    public override void Dispose()
    {
    }

}
