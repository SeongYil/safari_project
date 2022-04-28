using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources.Scripts
{

    //여러 게임 판 생성 및 필요 리소스 전담 클래스
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        //This is Main Camera in the Scene
        Camera m_MainCamera;


        public Material DefaultLineMaterial;
        public Material DefaultSpriteMaterial;


        private Sprite[] NumberSprite = null;
        private Dictionary<string, Sprite> number_sprite_dictionary = null;

        public int EnvironmentCount = 1;

        private AnimalRuleManager ControllerEnvrionment = null;
        private void Awake()
        {
            instance = this;

            PieceManager.CreatePieceManager();
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

            //This gets the Main Camera from the Scene
            m_MainCamera = Camera.main;

            NumberSprite = UnityEngine.Resources.LoadAll<Sprite>("images/number");
            number_sprite_dictionary = new Dictionary<string, Sprite>();

            for (int i = 0; i < NumberSprite.Length; i++)
            {

                number_sprite_dictionary[NumberSprite[i].name] = NumberSprite[i];

            }

            BoardManager.InitializeBoard();

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


                GameObject environment = new GameObject("Environment");
                environment.transform.parent = animalGame.transform;
                environment.transform.localPosition = new Vector3(0, 0, 0);

                Environment env = new Environment();
                ruleManager.Initialize(env);

                //Agent 생성
                ruleManager.InitializeAgent();

                env.Initialize(ruleManager, environment.transform);

                if( i == 0)
                {
                    ControllerEnvrionment = ruleManager;
                }
                

            }




        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                ControllerEnvrionment.ResetGame();
            }
        }
    }

}
