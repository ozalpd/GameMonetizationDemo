using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Pops.Controllers
{
    public class AdsController : MonoBehaviour
    {
        [Tooltip("Number of extra life ads can be shown")]
        [Range(0, 1000)]
        public int extraLifeAds = 1;

        [Tooltip("Time span to tolerate for failed ad videos, in seconds")]
        [Range(2, 1000)]
        public float enduranceDuration = 20f;

        private float rewardedAdStart = -1f;

        public bool CanShowExtraLifeAd
        {
            get
            {
                return extraLifeAds > 0 && CanShowRewardedAd;
            }
        }

        public bool CanShowRewardedAd
        {
            get
            {
                return Advertisement.IsReady(Placements.RewardedAd);
            }
        }

        public void ShowExtraLifeAd(Action<ShowResult> resultCallback)
        {
            if (CanShowExtraLifeAd)
            {
                ShowRewardedAd(resultCallback);
                extraLifeAds--;
            }
        }

        public void ShowRewardedAd(Action<ShowResult> resultCallback)
        {
            if (CanShowRewardedAd)
            {
                rewardedAdCallback = resultCallback;
                var options = new ShowOptions { resultCallback = RewardAdCallback };
                Advertisement.Show(Placements.RewardedAd, options);
                rewardedAdStart = Time.realtimeSinceStartup;
            }
        }
        private Action<ShowResult> rewardedAdCallback;

        public bool HasRewardedAdShown
        {
            get
            {
                return rewardedAdStart > 0
                    && Time.realtimeSinceStartup > rewardedAdStart + enduranceDuration;
            }
        }

        private void RewardAdCallback(ShowResult result)
        {
            rewardedAdCallback.Invoke(result);
            rewardedAdStart = -1f;
        }


        public static class Placements
        {
            public const string RewardedAd = "rewardedVideo";
        }
    }
}