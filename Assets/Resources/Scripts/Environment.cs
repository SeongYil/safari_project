using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Resources.Scripts;

//보드판 
namespace Assets.Resources.Scripts
{

    public class Environment
    {
        public AnimalRuleManager ruleManager = null;
        public Board board = null;


        // first player( white )
        public const int L1 = 1;  // Lion
        public const int E1 = 2;  // Elephant
        public const int G1 = 3;  // Giraph
        public const int P1 = 4;  // Chick  (Piyo Piyo! or Pawn)
        public const int C1 = 5;  // Chicken

        //second player( black )
        public const int L2 = 6;
        public const int E2 = 7;
        public const int G2 = 8;
        public const int P2 = 9;
        public const int C2 = 10;
        public const int kind = 11;

        private Transform parentTransform;
        public BoardSlot[,] BoardSlots = null;
        public BoardStock[,] BoardStocks = null;
        //    
        //y:0  흰코끼리    흰기린   흰 병아리
        //y:1  검코끼리    검기린   검 병아리
        public const int X = 3;
        public const int Y = 4;

        int elephant = (int)SharedDataType.EStockType.Elephant;
        int giraph = (int)SharedDataType.EStockType.Giraph;
        int chick = (int)SharedDataType.EStockType.Chick;

        int black = (int)SharedDataType.EColor.Black;
        int white = (int)SharedDataType.EColor.White;





        public void InitializePiece()
        {
            

            //게임 오브젝트에 피스 세팅
            //오브젝트 생성
            for (int y = 0; y < Y; ++y)
            {
                for (int x = 0; x < X; ++x)
                {
                    if (BoardManager.InitAnimalBoard[y, x] == 0)
                    {
                        continue;
                    }

                    int pieceID = BoardManager.InitAnimalBoard[y, x];

                    GameObject pieceObj = PieceManager.instance.CreatePiece(pieceID);
                    BoardSlot slot = BoardSlots[y, x].GetComponent<BoardSlot>();

                    pieceObj.transform.parent = BoardSlots[y, x].transform;
                    pieceObj.transform.localPosition = Vector3.zero;

                    Piece piece = pieceObj.GetComponent<Piece>();
                    slot.SetPiece(piece);

                }
            }
        }

        public void ResetEnvironment()
        {
            //보드판 초기화
            for (int y = 0; y < Y; ++y)
            {
                for (int x = 0; x < X; ++x)
                {
                    //일단 전부 삭제
                    BoardSlot slot = BoardSlots[y, x].GetComponent<BoardSlot>();
                    slot.DestoryHasPiece();


                    //원래 없는 곳은 곳은 다음
                    if (BoardManager.InitAnimalBoard[y, x] == 0)
                    {
                        continue;
                    }

                    int pieceID = BoardManager.InitAnimalBoard[y, x];

                    //새로 생성
                    GameObject pieceObj = PieceManager.instance.CreatePiece(pieceID);
                    pieceObj.transform.parent = BoardSlots[y, x].transform;
                    pieceObj.transform.localPosition = Vector3.zero;

                    //슬롯에 넣어줌
                    Piece piece = pieceObj.GetComponent<Piece>();
                    slot.SetPiece(piece);


                }
            }

            //스톡판 초기화
            BoardStocks[black, elephant].SetCount(BoardManager.InitStocks[black, elephant]);
            BoardStocks[black, giraph].SetCount(BoardManager.InitStocks[black, giraph]);
            BoardStocks[black, chick].SetCount(BoardManager.InitStocks[black, giraph]);


            BoardStocks[white, elephant].SetCount(BoardManager.InitStocks[black, giraph]);
            BoardStocks[white, giraph].SetCount(BoardManager.InitStocks[black, giraph]);
            BoardStocks[white, chick].SetCount(BoardManager.InitStocks[black, giraph]);

        }



        public void Initialize(AnimalRuleManager animalRuleManager, Transform parentTransform)
        {

            ruleManager = animalRuleManager;
            board = new Board();
            board.Initialize(parentTransform);
            this.parentTransform = parentTransform;


            //자료구조 생성 
            BoardSlots = new BoardSlot[Y, X];
            BoardStocks = new BoardStock[(int)SharedDataType.EColor.Count, (int)SharedDataType.EStockType.Count];

            //보드판 생성
            CreateBoardSlots();

            //기물 초기화
            InitializePiece();

            //잡은 기물 초기화
            InitializeStock();






        }


        public GameObject GetEnvironmentObj()
        {
            return parentTransform.gameObject;
        }

        public int[,] GetCurrentBoardState()
        {
            int[,] boardStates = new int[Y, X];

            for(int y = 0; y < Y; ++y)
            {
                for(int x = 0; x < X; ++x)
                {
                    if(BoardSlots[y, x].GetPiece() == null)
                    {
                        boardStates[y, x] = 0;
                        continue;
                    }
                    boardStates[y, x] = BoardSlots[y,x].GetPiece().pieceID;


                }
            }
            return boardStates;
        }





        public int[,] GetCurrentStockState()
        {
            int[,] stockStates = new int[(int)SharedDataType.EColor.Count, (int)SharedDataType.EStockType.Count];

            for(int y = 0; y < (int)SharedDataType.EColor.Count; ++y)
            {
                for(int x = 0; x < (int)SharedDataType.EStockType.Count; ++x)
                {
                    stockStates[y, x] = BoardStocks[y,x].Count;
                }
            }


            return stockStates;
        }


        public BoardStock CreateStockObject(string objName , int pieceID, int count,  float localPositionY, Transform parent)
        {
            GameObject stockObj = new GameObject(objName);
            stockObj.transform.parent = parent.transform;
            stockObj.transform.localPosition = new Vector3(0, localPositionY, 0f);
            stockObj.AddComponent<BoxCollider2D>();

            float x = 0f;
            //black
            if (pieceID > 5)
            {
                x = -1f;
            }
            else
            {
                x = 1f;
            }


            GameObject stockPieceObj = PieceManager.instance.CreatePiece(pieceID);
            stockPieceObj.transform.parent = stockObj.transform;
            stockPieceObj.transform.localPosition = Vector3.zero;
            stockPieceObj.transform.localScale = new Vector3(0.7f, 0.7f, 0);

            GameObject countObj = new GameObject("count");
            countObj.transform.parent = stockObj.transform;
            countObj.transform.localPosition = new Vector3(x,0,0);
            countObj.transform.localScale = new Vector3(0.07f, 0.07f, 0);
            SpriteRenderer number_renderer = countObj.AddComponent<SpriteRenderer>();
            number_renderer.sprite = GameManager.instance.GetSpriteNumber(count);
            number_renderer.material = GameManager.instance.DefaultSpriteMaterial;
            number_renderer.sortingLayerName = "Stock";
            number_renderer.sortingOrder = 0;

            
            BoardStock stockScript = stockObj.AddComponent<BoardStock>();
            stockScript.Initialize(pieceID, count, ruleManager , number_renderer , stockPieceObj);




            return stockScript;
        }

        


        public void InitializeStock()
        {
            

            //Black, White (2) {0,1} x 기린 코끼리 병아리 (3) {0,1,2}
            GameObject blackStocks = new GameObject("Black_Stocks");
            GameObject whiteStocks = new GameObject("White_Stocks");


            blackStocks.transform.parent = parentTransform;
            whiteStocks.transform.parent = parentTransform;

            blackStocks.transform.localPosition = new Vector3(-4f, 3.5f, 0);
            whiteStocks.transform.localPosition = new Vector3(4f, -0.5f, 0);

            int elephant = (int)SharedDataType.EStockType.Elephant;
            int giraph = (int)SharedDataType.EStockType.Giraph;
            int chick = (int)SharedDataType.EStockType.Chick;

            int black = (int)SharedDataType.EColor.Black;
            int white = (int)SharedDataType.EColor.White;   


            BoardStocks[black, elephant] = CreateStockObject("elephant", E2, BoardManager.InitStocks[1, elephant], 0f, blackStocks.transform);
            BoardStocks[black, giraph] = CreateStockObject("giraph", G2, BoardManager.InitStocks[1, giraph], -1.5f, blackStocks.transform);
            BoardStocks[black, chick] = CreateStockObject("chick", P2, BoardManager.InitStocks[1, chick], -3.0f, blackStocks.transform);


            BoardStocks[white, elephant] = CreateStockObject("elephant", E1, BoardManager.InitStocks[white, elephant], 0f, whiteStocks.transform);
            BoardStocks[white, giraph] = CreateStockObject("giraph", G1, BoardManager.InitStocks[white, giraph], -1.5f, whiteStocks.transform);
            BoardStocks[white, chick] = CreateStockObject("chick" , P1, BoardManager.InitStocks[white, chick], -3.0f, whiteStocks.transform);
            
            
            






        }

        // 콜리전 체크 및 렌더링 관련
        public void CreateBoardSlots()
        {
            GameObject boardSlots = new GameObject("Board_Slots");
            boardSlots.transform.parent = parentTransform;
            boardSlots.transform.localPosition = new Vector3(0f, 0f, 0f);


            for (int y = 0; y < Y; ++y)
            {
                for (int x = 0; x < X; ++x)
                {
                    //string pos_str = ruleManager.GetStringFromPosition(x, y);
                    string pos_str = GameManager.intPosToStringPos[(y, x)];
                    GameObject boardSlotObj = new GameObject(pos_str);
                    boardSlotObj.transform.parent = boardSlots.transform;
                    boardSlotObj.transform.localPosition = board.GetPieceRenderPositions(x, y); ;

                    //Create nonPermission 
                    GameObject nonPermissionSquare = new GameObject("permissionSquare");
                    nonPermissionSquare.transform.parent = boardSlotObj.transform;

                    nonPermissionSquare.transform.localPosition = new Vector3(0, 0, 0);
                    SpriteRenderer sprite = nonPermissionSquare.AddComponent<SpriteRenderer>();

                    sprite.material = GameManager.instance.DefaultSpriteMaterial;
                    sprite.sprite = UnityEngine.Resources.Load<Sprite>("images/square");
                    sprite.sortingLayerName = "NonPermission";
                    sprite.color = new Color32(0, 0, 0, 130);

                    nonPermissionSquare.transform.localScale = new Vector3(0.8f, 0.8f, 0);
                    nonPermissionSquare.SetActive(false);

                    BoardSlot boardSlotScript = boardSlotObj.AddComponent<BoardSlot>();
                    boardSlotScript.Initialize(ruleManager, nonPermissionSquare,  x, y);

                    BoxCollider2D collider = boardSlotObj.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(Board.size, Board.size);




                    BoardSlots[y, x] = boardSlotScript;

                }
            }
        }


    }
}