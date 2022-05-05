using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Scripts
{

    //여러 게임 판 생성 및 필요 리소스 전담 클래스
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public enum GameState
        {
            PLAY,
            PAUSE,
        }

        GameState eGameState = GameState.PAUSE;

        //This is Main Camera in the Scene
        Camera m_MainCamera;

        [System.NonSerialized]
        public Material DefaultLineMaterial;

        [System.NonSerialized]
        public Material DefaultSpriteMaterial;

        public bool ReleaseMode = false;

        public GameObject MenuBoardObj = null;
        public GameObject MenuButtonObj = null;
        public GameObject TurnBoardObj = null;

        public SharedDataType.EColor HumanColor = SharedDataType.EColor.White;

        public Unity.Barracuda.NNModel BlackModel;
        public Unity.Barracuda.NNModel WhiteModel;

        private Sprite[] NumberSprite = null;
        private Dictionary<string, Sprite> number_sprite_dictionary = null;

        public int error = 0;

        public bool isTraining = false;
        public int EnvironmentCount = 1;

        static public Dictionary<(int y, int x), string> intPosToStringPos = new Dictionary<(int y, int x), string>();
        static public Dictionary<string, (int y, int x)> StringPosToIntPos = new Dictionary<string, (int y, int x)>();

        private AnimalRuleManager ControllerEnvrionment = null;
        private void Awake()
        {
            instance = this;

            PieceManager.CreatePieceManager();

            
            DefaultLineMaterial = UnityEngine.Resources.Load<Material>("Material/DefaultLine");
            DefaultSpriteMaterial = UnityEngine.Resources.Load<Material>("Material/DefaultSprite");

            intPosToStringPos.Add((0, 0), "1a");
            intPosToStringPos.Add((0, 1), "2a");
            intPosToStringPos.Add((0, 2), "3a");

            intPosToStringPos.Add((1, 0), "1b");
            intPosToStringPos.Add((1, 1), "2b");
            intPosToStringPos.Add((1, 2), "3b");

            intPosToStringPos.Add((2, 0), "1c");
            intPosToStringPos.Add((2, 1), "2c");
            intPosToStringPos.Add((2, 2), "3c");

            intPosToStringPos.Add((3, 0), "1d");
            intPosToStringPos.Add((3, 1), "2d");
            intPosToStringPos.Add((3, 2), "3d");

            StringPosToIntPos.Add("1a", (0, 0));
            StringPosToIntPos.Add("2a", (0, 1));
            StringPosToIntPos.Add("3a", (0, 2));

            StringPosToIntPos.Add("1b", (1, 0));
            StringPosToIntPos.Add("2b", (1, 1));
            StringPosToIntPos.Add("3b", (1, 2));

            StringPosToIntPos.Add("1c", (2, 0));
            StringPosToIntPos.Add("2c", (2, 1));
            StringPosToIntPos.Add("3c", (2, 2));

            StringPosToIntPos.Add("1d", (3, 0));
            StringPosToIntPos.Add("2d", (3, 1));
            StringPosToIntPos.Add("3d", (3, 2));

            //리소스 로드
            m_MainCamera = Camera.main;

            NumberSprite = UnityEngine.Resources.LoadAll<Sprite>("images/number");
            number_sprite_dictionary = new Dictionary<string, Sprite>();

            for (int i = 0; i < NumberSprite.Length; i++)
            {

                number_sprite_dictionary[NumberSprite[i].name] = NumberSprite[i];

            }

            BoardManager.InitializeBoard();


            //만약 Release 모드라면 선후공 페이지 생성  
            if (ReleaseMode == true)
            {

                //TurnImage = TurnBoardObj.GetComponent<Image>();

                // 선 후공 메뉴 생성 
                //MenuBoardObj.SetActive(true);
                
                // 컨트롤하고 있는 게임룰 매니저의 Agent의 모드 변경

            }



        }

        public void SetHumanFirst()
        {
            HumanColor = SharedDataType.EColor.White;

            ControllerEnvrionment.ResetGame();

            MenuBoardObj.SetActive(false);
        }

        public void SetHumanSecond()
        {
            HumanColor = SharedDataType.EColor.Black;
            ControllerEnvrionment.ResetGame();

            MenuBoardObj.SetActive(false);

        }


        public Sprite GetSpriteNumber(int number)
        {
            string key = "number_";

            if (number < 0 || number > 10)
            {
                return null;
            }

            if (number == 0)
            {
                key += "9";
            }
            else
            {
                int number_key = number - 1;
                key = key + number_key;
            }

            return number_sprite_dictionary[key];
        }

        // Start is called before the first frame update
        void Start()
        {
            //Initialize
            for (int i = 0; i < EnvironmentCount; i++)
            {
                GameObject animalGame = new GameObject("AnimalGame_" + i);
                animalGame.transform.position = new Vector3((i % 4) * 17, (i / 4) * -10, 0);

                GameObject background = new GameObject("Background");
                background.transform.parent = animalGame.transform;
                background.transform.localPosition = new Vector3(0, 0, 0);

                SpriteRenderer backgroundSprite = background.AddComponent<SpriteRenderer>();
                backgroundSprite.sprite = UnityEngine.Resources.Load<Sprite>("images/bg");
                backgroundSprite.material = DefaultSpriteMaterial;

                AnimalRuleManager ruleManager = animalGame.AddComponent<AnimalRuleManager>();


                if (i == 0)
                {
                    ControllerEnvrionment = ruleManager;

                    if(ReleaseMode)
                    {
                        //ControllerEnvrionment.delegateChange = SetTurnColor;
                    }
                    

                }
                ruleManager.gameID = i;


            }

        }

    }

}
