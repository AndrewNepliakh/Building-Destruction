using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class LevelUI : MonoBehaviour, ILevelUI
    {
        public int Stars { get; private set; }
        public int Index => _index;
        public Button SelectButton => _selectButton;

        public Action<int> OnSelectLevel;

        [SerializeField] private int _index;
        [SerializeField] private ButtonScalable _selectButton;

        private const int STARS_AMOUNT = 3;

        [SerializeField] private GameObject[] _stars = new GameObject[STARS_AMOUNT];
        [SerializeField] private GameObject[] _greyStars = new GameObject[STARS_AMOUNT];
        [SerializeField] private GameObject _glow;

        private IUserManager _userManager;

        public void Init(LevelUIArgs args)
        {
            _userManager = args.UserManager;
            Stars = _userManager.CurrentUser.UserProgress.Levels[_index].Stars;

            SetStarts();
        }

        private void OnEnable()
        {
            _selectButton.AddListener(OnSelectButtonClicked);
        }

        private void SetStarts()
        {
            if (Stars == 0)
            {
                for (var i = 0; i < STARS_AMOUNT; i++)
                {
                    _stars[i].SetActive(false);
                    _greyStars[i].SetActive(false);
                }
                _glow.SetActive(false);
                return;
            }

            _glow.SetActive(true);

            for (var i = 0; i < STARS_AMOUNT; i++)
            {
                _stars[i].SetActive(false);
                _greyStars[i].SetActive(true);
            }

            for (var i = 0; i < Stars; i++)
            {
                _stars[i].SetActive(true);
                _greyStars[i].SetActive(false);
            }
        }

        private void OnSelectButtonClicked()
        {
            OnSelectLevel?.Invoke(_index);
        }

        private void OnDisable()
        {
            _selectButton.RemoveListener(OnSelectButtonClicked);
        }
    }

    public class LevelUIArgs
    {
        public IUserManager UserManager;
    }
}