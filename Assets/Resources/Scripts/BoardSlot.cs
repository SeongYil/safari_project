using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Scripts
{

    public class BoardSlot : MonoBehaviour
    {

        private bool isDragging = false;

        private AnimalRuleManager ruleManager = null;

        public int X = -1;
        public int Y = -1;

        private Piece hasPiece = null;

        public GameObject PermissionObj = null;

        


        public void Initialize(AnimalRuleManager ruleManager, GameObject permissionObj,  int x, int y)
        {
            this.ruleManager = ruleManager;
            //this.pieceTran

            //this.pieceTransform = transform.parent;
            //this.ruleManager = ruleManager;
            //this.PieceID = pieceID;
            this.X = x;
            this.Y = y;

            //slot
            transform.gameObject.layer = 9;

            PermissionObj = permissionObj;

            //piece = new Piece();


        }

        public Piece DetachPiece()
        {
            Piece detachPiece = hasPiece;
            hasPiece = null;
            return detachPiece;
        }

        public void DestoryHasPiece()
        {
            if(hasPiece == null)
            {
                return;
            }

            Destroy(hasPiece.gameObject);
            hasPiece = null;
        }

        public void SetPiece(Piece piece)
        {
            if(hasPiece != null)
            {
                DestoryHasPiece();
            }

            hasPiece = piece;
        }


        public void SetPermission(bool active)
        {
            PermissionObj.SetActive(active);
            
        }
        public Piece GetPiece()
        {
            return hasPiece; 
        }

        void Start()
        {





        }

        //학습용 주석
        // Update is called once per frame
        void Update()
        {

            //if (ruleManager == null)
            //    return;

            ////잡았다가 놓음
            if (isDragging == false)
            {

                if (hasPiece == null)
                {
                    return;
                }

                //if(hasPiece.transform.position.Equals(Vector3.zero) == true )
                //{
                //    return;
                //}

                hasPiece.transform.localPosition = Vector3.zero;
            }



        }



        void OnMouseOver()
        {


            // 위에 올라온 상태에서 왼쪽 버튼 클릭시 잡는다
            // 게임 룰 매니저에게 잡은 기물위치, 잡힌 기물 이벤트 전송
            // 게임 룰매니저는 받아서 갈 수 있는 위치 보여줌 
            // 게임 룰 매니저는 잡았을 때 처리 메소드 구현
            //

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                //매니저야 여기서 마우스 다운함! 
                //여기 기물 있는지 알려줘 
                //있으면 selectPiece에 찾아서 넣음
                
                if (hasPiece == null)
                {
                    return;
                }



                if ( ruleManager.CheckTurn(hasPiece.eColor) == false)
                {
                    return;
                }

                if (hasPiece != null)
                {
                    hasPiece.gameObject.SetActive(false);
                }

                ruleManager.SelectSlot(this);
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("dest : " + ruleManager.GetStringPosition(X, Y));
                //매니저야 여기서 마우스 업했엉
                //
                ruleManager.MoveTry(X, Y);
                //ruleManager.isEnvironmentInit = false;
                
            }



        }

        private void OnMouseUp()
        {
            if (hasPiece == null)
                return;

            if (hasPiece != null)
            {
                hasPiece.gameObject.SetActive(true);
            }


            isDragging = false;
            ruleManager.DestorySelectedPiece();
            ruleManager.InactiveVisualizeMovePath();




        }



    }
}
