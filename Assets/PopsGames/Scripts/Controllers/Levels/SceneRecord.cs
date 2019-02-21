using UnityEngine;

namespace Pops.Controllers.Levels
{
    [CreateAssetMenu(fileName = "SceneRec01", menuName = "Scene Record")]
    public class SceneRecord : ScriptableObject
    {
        [Tooltip("File name of the scene without \".scene\" suffix.")]
        [SerializeField]
        private string sceneName;

        [SerializeField]
        private string headerText;

        [TextArea(14, 10)]
        [SerializeField]
        private string storyText;

        [SerializeField]
        private bool locked;

        [SerializeField]
        private SceneRecord nextScene;

        [Header("Button Properties")]
        public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        public delegate void LockedChange(SceneRecord sender, bool isLocked);

        public string HeaderText { get { return headerText; } }

        public bool IsLocked
        {
            get { return locked; }
            set
            {
                if (locked != value)
                {
                    locked = value;
                    if (IsLockedChanged != null)
                        IsLockedChanged(this, locked);
                }
            }
        }
        public event LockedChange IsLockedChanged;

        public SceneRecord NextScene { get { return nextScene; } }
        public string SceneName { get { return sceneName; } }
        public string StoryText { get { return storyText; } }
    }
}