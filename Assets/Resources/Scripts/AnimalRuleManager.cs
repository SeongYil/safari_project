using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Resources.Scripts;
using System.Diagnostics;
using System;
using System.Text;
//전체적인 룰을 전담하는 클래스


//mcts --> 룰 매니저 현재턴에서 가능한 액션 알려줘




namespace Assets.Resources.Scripts
{


    public class AnimalRuleManager : MonoBehaviour
    {

        public Environment env = null;


        //# move direction
        (int dirY, int dirX) UL = (-1, -1);
        (int dirY, int dirX) UU = (-1, 0);
        (int dirY, int dirX) UR = (-1, 1);
        (int dirY, int dirX) ML = (0, -1);
        (int dirY, int dirX) MR = (0, 1);
        (int dirY, int dirX) DL = (1, -1);
        (int dirY, int dirX) DD = (1, 0);
        (int dirY, int dirX) DR = (1, 1);


        Dictionary<int, (int dirY, int dirX)[]> ALLOWED_MOVES = null;



        private Piece SelectedPiece = null;

        private string start_pos_str = "";

        public Dictionary<SharedDataType.EColor, SafariAgent> Agent = new Dictionary<SharedDataType.EColor, SafariAgent>();




        public SharedDataType.EColor eCurrentTurn = SharedDataType.EColor.White;


        public void InitializeAgent()
        {
            Agent[SharedDataType.EColor.Black] = CreateAgent("BlackAgent", SharedDataType.EColor.Black);
            Agent[SharedDataType.EColor.White] = CreateAgent("WhiteAgent", SharedDataType.EColor.White);

        }

        public SafariAgent CreateAgent(string objectName, SharedDataType.EColor colorType)
        {
            GameObject agentObj = new GameObject(objectName);
            agentObj.transform.parent = transform;

            SafariAgent agent = agentObj.AddComponent<SafariAgent>();
            agent.InitializeAgent(this, objectName, colorType);

            

            return agent;
        }

        public void Initialize(Environment animalGame)
        {
            env = animalGame;

            ALLOWED_MOVES = new Dictionary<int, (int dirY, int dirX)[]>()
            {
                [Environment.L1] = new (int dirY, int dirX)[] { UL, UU, UR, ML, MR, DL, DD, DR },
                [Environment.L2] = new (int dirY, int dirX)[] { UL, UU, UR, ML, MR, DL, DD, DR },
                [Environment.E1] = new (int dirY, int dirX)[] { UL, UR, DL, DR },
                [Environment.E2] = new (int dirY, int dirX)[] { UL, UR, DL, DR },
                [Environment.G1] = new (int dirY, int dirX)[] { UU, ML, MR, DD },
                [Environment.G2] = new (int dirY, int dirX)[] { UU, ML, MR, DD },
                [Environment.P1] = new (int dirY, int dirX)[] { UU },
                [Environment.P2] = new (int dirY, int dirX)[] { DD },
                [Environment.C1] = new (int dirY, int dirX)[] { UL, UU, UR, ML, MR, DD },
                [Environment.C2] = new (int dirY, int dirX)[] { DL, DD, DR, ML, MR, UU },

            };





        }

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            Agent[eCurrentTurn].RequestDecision();

            //해당 피스를 새로 만들어주고 크기는 원래대로 
            //마우스 포지션에 붙게함
            if (SelectedPiece != null)
            {
                if (CheckTurn(SelectedPiece.eColor) == false)
                {
                    //non select 
                    return;
                }

                SelectedPiece.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }


 
            



        }

        public void ResetGame()
        {
            eCurrentTurn = SharedDataType.EColor.White;
            GameObject envObj = env.GetEnvironmentObj();
            Destroy(envObj);
            GameObject environment = new GameObject("Environment");
            environment.transform.parent = transform;
            environment.transform.localPosition = new Vector3(0, 0, 0);

            Environment newEnv = new Environment();
            Initialize(newEnv);

            env.Initialize(this, environment.transform);

            


        }



        //해당 액션이 적법한지 확인하는 메소드
        public bool CheckAction(double action)
        {
            Dictionary<double,double> allAction = GetAvailableAllActions();

            if ( allAction.ContainsKey(action) == false)
            {
                return false;
            }

            return true;
        }

        //action 들어오면 그 행동을 한다
        public void SetActionMove(double action)
        {
            //액션 디코딩 후 

            if( GetAvailableAllActions().ContainsKey(action) == false )
            {
                return;
            }


            int[,] boardState = env.GetCurrentBoardState();

            (string start, string dest) pos = Decoder.action_to_stringTuple(action, boardState);

            EGameState actionResult = SetActionMove(pos.start, pos.dest);


            SetReward(actionResult);

            if (actionResult == EGameState.Win)
            {
                return;
            }

            ChangeTurn();

        }

        //
        public enum EActionType
        {
            Stock,
            Piece,
            None,
        }

        public enum EGameState
        {
            Continue,
            CaptureChick,
            CaptureChicken,
            CaptureGiraph,
            CaptureElephant,
            OnBoardChick,
            OnBoardGiraph,
            OnBoardElephant,
            Promotion,
            ThreatLion,
            StupidAction,
            Win,
        }

        public EGameState SetActionMove(string start, string end)
        {
            EActionType actionType = EActionType.Piece;
            EGameState result = EGameState.Continue;

            //새로운 기물 놓는지 check
            if (start.Length < 2 )
            {
                actionType = EActionType.Stock;
            }

            //(int X, int Y) target_pos = GetPositionFromString(end);
            (int Y, int X) target_pos = GameManager.StringPosToIntPos[end];
            
            //      x:0,    x:1 ,    x2:
            //y:0  [0,0]   [0,1]   [0,2]
            //y:1  [1,0]   [1,1]   [1,2]
            //y:2  [2,0]   [2,1]   [2,2]
            //y:3  [3,0]   [3,1]   [3,2]


            //  1 ,2 ,3
            //a 1a 2a 3a
            //b 1b 2b 3b
            //c 1c 2c 3c
            //d 1d 2d 3d
            //[ y , x ]



            //      x:0,    x:1 ,    x2:
            //y:0  [0,0]   [0,1]   [0,2] 
            //y:1  [1,0]   [1,1]   [1,2]
            //y:2  [2,0]   [2,1]   [2,2]
            //y:3  [3,0]   [3,1]   [3,2]

            //  1 ,2 ,3
            //a 1a 2a 3a
            //b 1b 2b 3b
            //c 1c 2c 3c
            //d 1d 2d 3d 

            //[ y , x ] 


            BoardSlot targetSlotScript = env.BoardSlots[target_pos.Y, target_pos.X];   


            switch(actionType)
            {
                case EActionType.Piece:
                    {
                        // target피스가 뭔진 모르는데 일단 가져와
                        Piece targetPiece = targetSlotScript.GetPiece();
                        //기존 위치에 있던 기물의 piece의 부모를 변경 
                        //(int X, int Y) start_pos = GetPositionFromString(start);
                        (int Y, int X) start_pos = GameManager.StringPosToIntPos[start];
                        BoardSlot startSlotScript = env.BoardSlots[start_pos.Y, start_pos.X];

                        Piece startPiece = startSlotScript.GetPiece();
                        startSlotScript.SetPiece(null);

                        
                        if (targetPiece != null ) //가져온 타겟피스에 기물이 있는 경우
                        {
                            //잡는 기물이 있는 경우
                            (int color, int pieceID) stockID = BoardStock.GetStockID(targetPiece.pieceID);

                            int currentTurn = (int)eCurrentTurn;
                            //stock 하나 늘리고
                            if (env.BoardStocks[currentTurn, stockID.pieceID].Count < 9)
                            {
                                env.BoardStocks[currentTurn, stockID.pieceID].Count += 1;
                                env.BoardStocks[currentTurn, stockID.pieceID].SetCount(env.BoardStocks[currentTurn, stockID.pieceID].Count);
                            }

                            //게임 종료 
                            if (targetPiece.pieceID == Environment.L1 || targetPiece.pieceID == Environment.L2)
                            {
                                result = EGameState.Win;
                                return result;
                            }

                            if( targetPiece.pieceID == Environment.P1 || targetPiece.pieceID == Environment.P2)
                            {
                                result = EGameState.CaptureChick;
                            }
                            else if (targetPiece.pieceID == Environment.C1 || targetPiece.pieceID == Environment.C2)
                            {
                                result = EGameState.CaptureChicken;
                            }
                            else if (targetPiece.pieceID == Environment.G1 || targetPiece.pieceID == Environment.G2)
                            {
                                result = EGameState.CaptureGiraph;
                            }
                            else if (targetPiece.pieceID == Environment.E1 || targetPiece.pieceID == Environment.E2)
                            {
                                result = EGameState.CaptureElephant;
                            }

                            //잡은 피스 삭제
                            Destroy(targetPiece.gameObject);
                            
                        }

                        //병아리가 끝까지 도달한 경우
                        //흰색   병아리이 y == 0일 때 
                        //검은색  병아리가 y== 3일때 
                        if( target_pos.Y == 0 && startPiece.pieceID == Environment.P1)
                        {
                            startPiece.Promotion();
                            result = EGameState.Promotion;
                        }
                        if (target_pos.Y == 3 && startPiece.pieceID == Environment.P2)
                        {
                            startPiece.Promotion();
                            result = EGameState.Promotion;
                        }

                        if (target_pos.Y == 3 && startPiece.pieceID == Environment.L2)
                        {
                            //끝까지 갔음
                            result = EGameState.Win;
                            return result;
                        }

                        if (target_pos.Y == 0 && startPiece.pieceID == Environment.L1)
                        {
                            //끝까지 갔음
                            result = EGameState.Win;
                            return result;
                        }


                        //변경 
                        targetSlotScript.SetPiece(startPiece);
                        startPiece.transform.parent = targetSlotScript.transform;
                        startPiece.transform.localPosition = Vector3.zero;

                        break;
                    }


                case EActionType.Stock:
                    {
                        //SourcePiece
                        (int color, int pieceID) stockID = GetStockFromString(start);
                        Piece sourcePiece = env.BoardStocks[stockID.color, stockID.pieceID].GetPiece();

                        GameObject copy = Instantiate(sourcePiece.gameObject);
                        Piece copyPiece = copy.GetComponent<Piece>();


                        targetSlotScript.SetPiece(copyPiece);
                        copyPiece.transform.localScale = new Vector3(1, 1, 0);
                        copyPiece.transform.parent = targetSlotScript.transform;
                        copyPiece.transform.localPosition = Vector3.zero;


                        //스톡 카운터 감소 
                        env.BoardStocks[stockID.color, stockID.pieceID].Count -= 1;
                        env.BoardStocks[stockID.color, stockID.pieceID].SetCount(env.BoardStocks[stockID.color, stockID.pieceID].Count);

                        if(copyPiece.pieceID == Environment.P1 || copyPiece.pieceID == Environment.P2)
                        {
                            result = EGameState.OnBoardChick;
                        }
                        else if (copyPiece.pieceID == Environment.G1 || copyPiece.pieceID == Environment.G2)
                        {
                            result = EGameState.OnBoardGiraph;
                        }
                        else if (copyPiece.pieceID == Environment.E1 || copyPiece.pieceID == Environment.E2)
                        {
                            result = EGameState.OnBoardElephant;
                        }


                        break;
                    }
            }

            // 내기물이 사자인가? 
            if (targetSlotScript.GetPiece().pieceID == Environment.L1 || targetSlotScript.GetPiece().pieceID == Environment.L2)
            {
                //사자인데, 상대방 기물의 사정거리에 내 사자 있는 경우 
                foreach (BoardSlot slot in env.BoardSlots)
                {
                    if (slot.GetPiece() == null)
                    {
                        continue;
                    }

                    if (slot.GetPiece().eColor != eCurrentTurn)
                    {
                        //해당 슬롯의 기물은 적군이다.
                        //적군의 사정거리에 현재 나의 포지션이 있는가? 
                        //피스의 모든 경로를 리턴하는 함수 GetPieceMovePosition

                        Dictionary<(int Y, int X), (int Y, int X)> allowed_position = GetPieceMovePositionDict(slot.X, slot.Y, slot.GetPiece().pieceID);

                        //적군기물의 이동범위에 현재 내 사자위치가 있다면 스튜핏 무브를 리턴한다
                        if (allowed_position.ContainsKey((targetSlotScript.Y, targetSlotScript.X)) == true)
                        {
                            return EGameState.StupidAction;
                        }

                    }
                }
            }

            //나의 기물로 상대방의 사자에게 장군을 불렀는가?
            if (CheckRangeOpponentLion(targetSlotScript) == true)
            {
                result = EGameState.ThreatLion;
            }


            //내가 어떤 액션을 다했는데, 여전히 장군상태인 경우, 스튜핏액션을 한거다.
            foreach( BoardSlot thisSlot in env.BoardSlots )
            {
                //비어있으면 
                if (thisSlot.GetPiece() == null )
                {
                    continue;
                }

                //적군 기물이면  
                if (thisSlot.GetPiece().eColor != eCurrentTurn)
                {
                    continue;
                }

                if (thisSlot.GetPiece().pieceID == Environment.L1 || thisSlot.GetPiece().pieceID == Environment.L2 )
                {
                    BoardSlot myLioninSlot = thisSlot;

                    //적군 기물들한테, thisSlot에 올 수 있는 친구?
                    foreach ( BoardSlot enemySlot in env.BoardSlots)
                    {
                        if( enemySlot.GetPiece() == null)
                        {
                            continue;
                        }

                        if (enemySlot.GetPiece().eColor == eCurrentTurn)
                        {
                            continue;
                        }

                        var allowedPositionDict = GetPieceMovePositionDict(enemySlot.X, enemySlot.Y, enemySlot.GetPiece().pieceID);

                        if (allowedPositionDict.ContainsKey( (myLioninSlot.Y, myLioninSlot.X) ) == true)
                        {
                            return EGameState.StupidAction;
                        }

                    }


                }

            }

            return result;


        }

        public void SetReward(EGameState actionResult)
        {
            SharedDataType.EColor eOpponent = SharedDataType.EColor.Count;
            if (eCurrentTurn == SharedDataType.EColor.White)
            {
                eOpponent = SharedDataType.EColor.Black;
            }
            else if (eCurrentTurn == SharedDataType.EColor.Black)
            {
                eOpponent = SharedDataType.EColor.White;
            }


            switch(actionResult)
            {
                case EGameState.Promotion:
                    {
                        UnityEngine.Debug.Log("진화");

                        Agent[eCurrentTurn].AddReward(+0.025f);
                        break;
                    }
                case EGameState.OnBoardGiraph:
                    {
                        UnityEngine.Debug.Log("기린 놓기");

                        Agent[eCurrentTurn].AddReward(+0.03f);
                        break;
                    }
                case EGameState.OnBoardElephant:
                    {

                        UnityEngine.Debug.Log("코끼리 놓기");

                        Agent[eCurrentTurn].AddReward(+0.02f);
                        break;
                    }
                case EGameState.OnBoardChick:
                    {
                        UnityEngine.Debug.Log("병아리 놓기");

                        Agent[eCurrentTurn].AddReward(+0.01f);
                        break;
                    }
                case EGameState.CaptureChick:
                    {
                        UnityEngine.Debug.Log("병아리 잡기");
                        Agent[eCurrentTurn].AddReward(+0.01f);
                        Agent[eOpponent].AddReward(-0.01f);
                        break;
                    }
                case EGameState.CaptureChicken:
                    {
                        UnityEngine.Debug.Log("닭 잡기");

                        Agent[eCurrentTurn].AddReward(+0.035f);
                        Agent[eOpponent].AddReward(-0.035f);
                        break;
                    }
                case EGameState.CaptureElephant:
                    {
                        UnityEngine.Debug.Log("코끼리 잡기");

                        Agent[eCurrentTurn].AddReward(+0.02f);
                        Agent[eOpponent].AddReward(-0.02f);
                        break;
                    }
                case EGameState.CaptureGiraph:
                    {
                        UnityEngine.Debug.Log("기린 잡기");

                        Agent[eCurrentTurn].AddReward(+0.03f);
                        Agent[eOpponent].AddReward(-0.03f);
                        break;
                    }
                case EGameState.Win:
                    {
                        UnityEngine.Debug.Log("State : Win");

                        Agent[eCurrentTurn].SetReward(1.0f);
                        Agent[eOpponent].SetReward(-1.0f);

                        Agent[eCurrentTurn].EndEpisode();
                        Agent[eOpponent].EndEpisode();

                        ResetGame();

                        break;
                    }
                case EGameState.ThreatLion:
                    {
                        UnityEngine.Debug.Log("사자 위협");

                        Agent[eCurrentTurn].AddReward(0.1f);
                        Agent[eOpponent].AddReward(-0.1f);
                        break;
                    }
                case EGameState.Continue:
                    {
                        //일반
                        UnityEngine.Debug.Log("일반 움직임");
                        Agent[eCurrentTurn].AddReward(-0.01f);
                        break;
                    }
                case EGameState.StupidAction:
                    {
                        UnityEngine.Debug.Log("자충수");

                        Agent[eCurrentTurn].SetReward(-1.0f);
                        Agent[eOpponent].SetReward(0.0f);
                        
                        Agent[eCurrentTurn].EndEpisode();
                        Agent[eOpponent].EndEpisode();

                        break;
                    }
                default:
                    {
                        UnityEngine.Debug.Log("actionResult Warning");
                        break;
                    }
            }



        }

        public bool CheckTurn(SharedDataType.EColor color)
        {
            if( eCurrentTurn == color )
            {
                return true;
            }

            return false;
        }

        //특정 기물을 잡고 있을 때 처리하는 메소드
        public void SelectSlot(BoardSlot slot)
        {
            //기물을 잡고 있는 상태라고 인지하기
            //start_pos_str = GetStringFromPosition(slot.X, slot.Y);
            start_pos_str = GameManager.intPosToStringPos[(slot.Y, slot.X)];


            SelectedPiece = Instantiate(slot.GetPiece());
            SelectedPiece.transform.parent = transform;
            SelectedPiece.gameObject.SetActive(true);

            if (CheckTurn(SelectedPiece.eColor) == false)
            {
                //non select 
                return;
            }

            SelectedPiece.Selected();

            VisualizeMovePath(slot);


        }

        public void SelectStock(BoardStock selectedStock)
        {
            int pieceID = selectedStock.PieceID;

            GameObject createPieceObj = PieceManager.instance.CreatePiece(pieceID);
            SelectedPiece = createPieceObj.GetComponent<Piece>();
            SelectedPiece.gameObject.name = "SelectedStock";
            SelectedPiece.transform.parent = transform;
            SelectedPiece.Selected();

            switch (SelectedPiece.pieceID)
            {
                case Environment.E1:
                case Environment.E2:
                    {
                        start_pos_str = "E";
                        break;
                    }
                case Environment.G1:
                case Environment.G2:
                    {
                        start_pos_str = "G";
                        break;
                    }
                case Environment.P1:
                case Environment.P2:
                    {
                        start_pos_str = "P";
                        break;
                    }

            }

            VisualizeMovePath(SelectedPiece);


        }



        public (int, int) GetStockFromString(string stock_str)
        {
            int pieceID = 0;

            switch(stock_str)
            {
                case "P":
                case "C":
                    {
                        pieceID = 2;
                        break;
                    }
                case "E":
                    {
                        pieceID = 0;
                        break;
                    }
                case "G":
                    {
                        pieceID = 1;
                        break;
                    }
            }


            (int, int) stockID = ((int)eCurrentTurn, pieceID);


            return stockID;
        }


        public bool MoveTry(int target_x, int target_y)
        {
            if( start_pos_str == "")
            {
                return false;
            }


            //string target_pos_str = GetStringFromPosition(target_x, target_y);
            string target_pos_str = GameManager.intPosToStringPos[(target_y, target_x)];


            //현재 액션을 숫자 변환을 한다 
            double action = Decoder.encode_to_action_index(start_pos_str, target_pos_str, (double)eCurrentTurn);

            

            //해당 액션이 적법한지 체크
            if (CheckAction(action) == false)
            {
                //허용 되는 좌표가 아닌 경우 
                //셀렉트 된 피스 원래상태로 원복


                //셀렉트 피스 초기화
                UnSelectedPiece();

                return false;
            }

            //stock or piece
            //UnityEngine.Debug.Log("start : " + start_pos_str);

            //target slot
            //UnityEngine.Debug.Log("dest : " + target_pos_str);

            //UnityEngine.Debug.LogFormat("encode : {0}", action.ToString());

            //해당 액션 수행 
            //게임 종료

            //if(SetActionMove(start_pos_str, target_pos_str) == EGameState.Win)
            
                //ResetGame();
                //return false;
            
            var results = SetActionMove(start_pos_str, target_pos_str);

            if (results == EGameState.Win) 
            {
                SetReward(results);
                ResetGame();
                return false;
            }

            SetReward(results);




            //셀렉트 피스 초기화
            UnSelectedPiece();

            //턴 변경
            ChangeTurn();

            return true;

        }

        public void ChangeTurn()
        {
            if (eCurrentTurn == SharedDataType.EColor.White )
            {
                eCurrentTurn = SharedDataType.EColor.Black;
            }
            else
            {
                eCurrentTurn = SharedDataType.EColor.White;
            }

        }
        //public double GetToPlay(SharedDataType.EColor color)
        //{
        //    double to_play = -1;
        //    if (color == SharedDataType.EColor.White)
        //    {
        //        to_play = 1;
        //    }
        //    else
        //    {
        //        to_play = 0;
        //    }

        //    return to_play;
        //}


        public Dictionary<double, double> GetAvailableAllActions()
        {
            Dictionary<double,double> actions = new Dictionary<double,double>();

            List<BoardSlot> emptySlot = new List<BoardSlot>();

            //보드에 있는 피스 처리
            for (int y = 0; y < Environment.Y; ++y)
            {
                for (int x = 0; x < Environment.X; ++x)
                {
                    BoardSlot slot = env.BoardSlots[y, x].GetComponent<BoardSlot>();



                    Piece piece = slot.GetPiece();

                    //슬롯에 기물이 없는 경우 패스
                    if(piece == null)
                    {
                        emptySlot.Add(slot);
                        continue;
                    }

                    //기물의 색이 현재 턴과 다른 경우 패스
                    if( piece.eColor != eCurrentTurn )
                    {
                        continue;
                    }

                    //현재 기물 이동 할 수 있는 모든 루트를 인코딩해서 actions에 add
                    List<(int Y, int X)> allowed_position = GetPieceMovePosition(x, y, piece.pieceID);


                    //현재 위치에서 해당 기물의 이동 경로만 밝게 표현
                    for (int i = 0; i < allowed_position.Count; ++i)
                    {
                        //갈 수 있는지
                        if (CheckMove(piece.eColor, allowed_position[i].X, allowed_position[i].Y) == false)
                        {
                            //갈 수 없다
                            continue;
                        }
                        //갈 수 있다 

                        //start , end
                        //string start_pos = GetStringFromPosition(x, y);
                        string start_pos = GameManager.intPosToStringPos[(y, x)];

                        //string end_pos = GetStringFromPosition(allowed_position[i].X, allowed_position[i].Y);
                        string end_pos = GameManager.intPosToStringPos[(allowed_position[i].Y, allowed_position[i].X)];


                        double action = Decoder.encode_to_action_index(start_pos, end_pos, (double)eCurrentTurn);
                        actions.Add(action, action);
                    }

                }
            }


            int currentColor = (int)eCurrentTurn;


            for (int i = 0; i < 3; ++i)
            {
                string start_pos = "";
                if (env.BoardStocks[currentColor, i].Count > 0)
                {
                    switch (i)
                    {
                        //코끼리
                        case 0:
                            {
                                start_pos = "E";
                                break;
                            }
                        //기린
                        case 1:
                            {
                                start_pos = "G";
                                break;
                            }
                        case 2:
                            {
                                start_pos = "P";
                                break;
                            }
                    }
                }

                foreach (BoardSlot slot in emptySlot)
                {
                    //string end_pos = GetStringFromPosition(slot.X, slot.Y);
                    string end_pos = GameManager.intPosToStringPos[(slot.Y, slot.X)];

                    if (start_pos == "")
                    {
                        continue;
                    }

                    double action = Decoder.encode_to_action_index(start_pos, end_pos, (double)eCurrentTurn);
                    actions.Add(action, action);
                }
            }
            return actions;
        }

        public bool MoveTryStock()
        {

            return false;
        }


        //모든 부분은 어둡게
        public void VisualizeAllNonPermission()
        {
            for (int y = 0; y < Environment.Y; ++y)
            {
                for (int x = 0; x < Environment.X; ++x)
                {
                    env.BoardSlots[y, x].SetPermission(true);
                }
            }
        }

        //특정 기물의 움직일 수 있는 모든 좌표 리턴 함수
        public List<(int Y, int X)> GetPieceMovePosition(int start_x, int start_y, int pieceID)
        {
            (int dirY, int dirX)[] allowed_moves = ALLOWED_MOVES[pieceID];


            //현재 위치에서 이동 할 수 있는 좌표들 얻어오기 
            List<(int Y, int X)> allowed_position = new List<(int Y, int X)>();

            for (int i = 0; i < allowed_moves.Length; ++i)
            {
                int moved_x_pos = start_x + allowed_moves[i].dirX;

                //x 좌표 유효
                if (moved_x_pos < 0 || moved_x_pos >= Environment.X)
                    continue;

                int moved_y_pos = start_y + allowed_moves[i].dirY;

                //y좌표 유효
                if (moved_y_pos < 0 || moved_y_pos >= Environment.Y)
                    continue;

                allowed_position.Add((moved_y_pos, moved_x_pos));
            }

            return allowed_position;
        }


        public Dictionary<(int Y, int X) , (int Y, int X)> GetPieceMovePositionDict(int start_x, int start_y, int pieceID)
        {
            (int dirY, int dirX)[] allowed_moves = ALLOWED_MOVES[pieceID];


            //현재 위치에서 이동 할 수 있는 좌표들 얻어오기 
            Dictionary<(int Y, int X), (int Y, int X)> allowed_position = new Dictionary<(int Y, int X), (int Y, int X)>();

            for (int i = 0; i < allowed_moves.Length; ++i)
            {
                int moved_x_pos = start_x + allowed_moves[i].dirX;

                //x 좌표 유효
                if (moved_x_pos < 0 || moved_x_pos >= Environment.X)
                    continue;

                int moved_y_pos = start_y + allowed_moves[i].dirY;

                //y좌표 유효
                if (moved_y_pos < 0 || moved_y_pos >= Environment.Y)
                    continue;

                allowed_position[(moved_y_pos, moved_x_pos)] = (moved_y_pos, moved_x_pos);
            }

            return allowed_position;
        }


        public void VisualizeMovePath(Piece selectedStockPiece)
        {
            VisualizeAllNonPermission();



            for (int y = 0; y < Environment.Y; ++y)
            {
                for (int x = 0; x < Environment.X; ++x)
                {
                    BoardSlot slot = env.BoardSlots[y, x].GetComponent<BoardSlot>();

                    if (slot.HasPiece() == false)
                    {
                        env.BoardSlots[y, x].SetPermission(false);
                    }

                }
            }



        }

        public bool CheckMove(SharedDataType.EColor pieceColor, int targetPos_x, int targetPos_y)
        {
            BoardSlot targetSlotScript = env.BoardSlots[targetPos_y, targetPos_x];

            if (targetSlotScript.HasPiece() == false)
            {
                return true;
            }

            Piece target_piece = targetSlotScript.GetPiece();

            SharedDataType.EColor targetColor = target_piece.eColor;

            if(pieceColor == targetColor)
            {
                //아군
                return false;
            }
            else
            {
                return true;
            }
        }


        //해당 슬롯의 피스의 사정거리내에 상대방 라이언이 있는지 
        public bool CheckRangeOpponentLion(BoardSlot slot)
        {
            int pieceID = slot.GetPiece().pieceID;
            SharedDataType.EColor color = slot.GetPiece().eColor;
            int board_x = slot.X;
            int board_y = slot.Y;

            //현재 위치에서 이동 할 수 있는 좌표들 얻어오기 
            List<(int Y, int X)> allowed_position = GetPieceMovePosition(board_x, board_y, pieceID);


            //현재 위치에서 해당 기물의 이동 경로만 밝게 표현
            for (int i = 0; i < allowed_position.Count; ++i)
            {
                //갈 수 있는지
                if (CheckMove(color, allowed_position[i].X, allowed_position[i].Y) == false)
                {
                    continue;
                }

                //피스가 있는가?
                BoardSlot targetSlotScript = env.BoardSlots[allowed_position[i].Y, allowed_position[i].X];
                if( targetSlotScript.HasPiece() == false )
                {
                    continue;
                }


                if ( targetSlotScript.GetPiece().pieceID == Environment.L1 || targetSlotScript.GetPiece().pieceID == Environment.L2 )
                {
                    return true;
                }


            }


            return false;
        }

        //특정 기물을 잡았을 때 이동 가능 경로 보여주기
        public void VisualizeMovePath(BoardSlot slot)
        {
            int pieceID = slot.GetPiece().pieceID;
            SharedDataType.EColor color = slot.GetPiece().eColor;
            int board_x = slot.X;
            int board_y = slot.Y;

            //나머지부분은 어둡게
            VisualizeAllNonPermission();


            //해당 기물이 이동 할 수 있는 디렉션 얻어오기 

            //현재 위치에서 이동 할 수 있는 좌표들 얻어오기 
            List<(int Y, int X)> allowed_position = GetPieceMovePosition(board_x, board_y, pieceID);


            //현재 위치에서 해당 기물의 이동 경로만 밝게 표현
            for (int i = 0; i < allowed_position.Count; ++i)
            {
                //갈 수 있는지
                if (CheckMove(color, allowed_position[i].X, allowed_position[i].Y) == false)
                {
                    env.BoardSlots[allowed_position[i].Y, allowed_position[i].X].SetPermission(true);
                    continue;
                }

                env.BoardSlots[allowed_position[i].Y, allowed_position[i].X].SetPermission(false);

            }


            //현재 잡은 기물 위치는 밝게 표현
            env.BoardSlots[board_y, board_x].SetPermission(false);


        }


        public void UnSelectedPiece()
        {
            start_pos_str = "";
            
            if (SelectedPiece != null)
            {
                
                SelectedPiece.CancleSelected();
                SelectedPiece.transform.localPosition = Vector3.zero;

            }
        }

        public void DestorySelectedPiece()
        {
            if(SelectedPiece == null)
            {
                return;
            }

            //start_pos_str = "";
            Destroy(SelectedPiece.gameObject);
            SelectedPiece = null;
            
        }

        public bool InactiveVisualizeMovePath()
        {
            bool result = false;

            for (int y = 0; y < Environment.Y; ++y)
            {
                for (int x = 0; x < Environment.X; ++x)
                {
                    env.BoardSlots[y, x].SetPermission(false);
                }
            }

            return result;
        }


    }

}