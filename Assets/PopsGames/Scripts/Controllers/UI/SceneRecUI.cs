using Pops.Controllers.Levels;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Pops.Controllers.UI
{
    public class SceneRecUI : MonoBehaviour
    {
        public Text headerText;
        public Text storyText;
        public GameObject lockSymbol;
        public GameObject loadScreen;
        public Slider loadSlider;

        private GameObject parent;
        private CanvasGroup canvasGroup;

        public SceneRecord SceneRecord
        {
            get { return sceneRecord; }
            set
            {
                if (sceneRecord != null)
                    sceneRecord.IsLockedChanged -= SceneRecord_IsLockedChanged;
                sceneRecord = value;
                if (sceneRecord != null)
                    sceneRecord.IsLockedChanged += SceneRecord_IsLockedChanged;
                DisplaySceneRecord();
            }
        }

        private void SceneRecord_IsLockedChanged(SceneRecord sender, bool isLocked)
        {
            DisplaySceneRecord();
        }

        [SerializeField]
        private SceneRecord sceneRecord;

        private Image ButtonImage
        {
            get
            {
                if (_buttonImage == null)
                    _buttonImage = GetComponent<Image>();

                return _buttonImage;
            }
        }

        private Image _buttonImage;

        private void Awake()
        {
            if (SceneRecord != null)
            {
                DisplaySceneRecord();
                sceneRecord.IsLockedChanged += SceneRecord_IsLockedChanged;
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }

            parent = transform.parent.gameObject;
            if (parent != null)
                canvasGroup = parent.GetComponent<CanvasGroup>();
        }

        private void DisplaySceneRecord()
        {
            storyText.text = SceneRecord.StoryText;
            headerText.text = SceneRecord.HeaderText;
            lockSymbol.SetActive(SceneRecord.IsLocked);

            ButtonImage.color = SceneRecord.backgroundColor;
        }

        public void LoadScene()
        {
            if (SceneRecord == null)
                return;

            if (sceneRecord.IsLocked)
            {
                //TODO: Display a warning
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneRecord.SceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var asyncState = GameManager.LoadSceneAsync(sceneName);
            if (loadScreen != null)
                loadScreen.SetActive(true);

            while (!asyncState.isDone)
            {
                float progress = Mathf.Clamp01(asyncState.progress / 0.9f);
                if (loadSlider != null)
                    loadSlider.value = progress;

                if (canvasGroup != null)
                    canvasGroup.alpha = 01f - progress;

                yield return null;
            }
        }
    }
}