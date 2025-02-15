using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

namespace UI
{
    public class Upgrade : Interactable
    {
        public UpgradeData data;
        public UpgradeWindow upgradeWindow;

        public void Update()
        {
            if (!Focus || !Input.GetMouseButtonDown(0) || !IsActive())
            {
                return;
            }

            if (!CheckCosts())
            {
                upgradeWindow.infoNotEnoughResources.SetActive(true);
                StartCoroutine(HideNotEnoughResourcesInfo());

                return;
            }

            RemoveResources();
            HandleUpgrade();
            RemoveOldUpgrade();
            UnlockNewUpgrades();
            SendAnalytics();
        }

        private void SendAnalytics()
        {
            Analytics.CustomEvent("upgrade", new Dictionary<string, object>
            {
                {
                    "type", data.type
                },
                {
                    "score", DayNight.Instance.GetNightsSurvived()
                }
            });
        }

        private IEnumerator HideNotEnoughResourcesInfo()
        {
            yield return new WaitForSeconds(2);
            upgradeWindow.infoNotEnoughResources.SetActive(false);
        }

        private void UnlockNewUpgrades()
        {
            upgradeWindow.upgrades.AddRange(data.unlocks);
            upgradeWindow.ReRenderUpgrades();
        }

        private void RemoveOldUpgrade()
        {
            upgradeWindow.upgrades.Remove(data);
        }

        private void HandleUpgrade()
        {
            switch (data.type)
            {
                case UpgradeType.Axe:
                    Player.Instance.GainAxe();
                    break;
                case UpgradeType.WoodenHouse:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.SetLevel(BaseLevel.WoodenHouse);
                    break;
                case UpgradeType.StoneHouse:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.SetLevel(BaseLevel.StoneHouse);
                    break;
                case UpgradeType.Fence:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.EnableFence();
                    break;
                case UpgradeType.Pickaxe:
                    Player.Instance.GainPickaxe();
                    break;
                case UpgradeType.Gun:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.EnableGun1();
                    break;
                case UpgradeType.Gun2:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.EnableGun2();
                    break;
                case UpgradeType.Gun3:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.EnableGun3();
                    break;
                case UpgradeType.Gun4:
                    Base.Instance.PlayBuildSound();
                    Base.Instance.EnableGun4();
                    break;
                default:
                    Debug.LogError("Upgrade not implemented!");
                    break;
            }
        }

        private bool CheckCosts() => data.costs.All(cost => Resources.Instance.Get(cost.type) >= cost.amount);

        private void RemoveResources()
        {
            foreach (var cost in data.costs)
            {
                Resources.Instance.Sub(cost.type, cost.amount);
            }
        }
    }
}

public enum UpgradeType
{
    Axe,
    WoodenHouse,
    StoneHouse,
    Fence,
    Gun,
    Pickaxe,
    Gun2,
    Gun3,
    Gun4
}