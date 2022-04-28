using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Resources.Scripts
{

    //자신이 어떤 기물인지 확인
    /*
    // first player
    const int L1 = 1;  // Lion
    const int E1 = 2;  // Elephant
    const int G1 = 3;  // Giraph
    const int P1 = 4;  // Chick  (Piyo Piyo! or Pawn)
    const int C1 = 5;  // Chicken

    //second player
    const int L2 = 6;
    const int E2 = 7;
    const int G2 = 8;
    const int P2 = 9;
    const int C2 = 10;
    */
    //public int PieceID = -1;

    //보드판 기준, 자신 좌표를 알아야함 3x4
    /*
     *  0 1 2 
     *  3 4 5
     *  6 7 8 
     *  9 10 11
     */
    //public int X = -1;
    //public int Y = -1;



    // 기물 최상의 클래스
    // 어떻게 움직여야하는지, 자신의 보드판 좌표, 자신의 아이콘 ID
    public class Piece : MonoBehaviour
    {


        public int pieceID = -1;

        public SharedDataType.EColor eColor = SharedDataType.EColor.Count;
  

        public SpriteRenderer PieceImageRenderer;
        public SpriteRenderer BackColorRenderer;

        public Color ImageColor = Color.white;

        public GameObject backColorObj = null;
        public GameObject pieceImageObj = null;



        public void Initialize(int pieceID)
        {
            this.pieceID = pieceID;

            GameObject pieceObj = transform.gameObject;

            backColorObj = transform.GetChild(0).gameObject;
            pieceImageObj = transform.GetChild(1).gameObject;

            BackColorRenderer = backColorObj.GetComponent<SpriteRenderer>();
            PieceImageRenderer = pieceImageObj.GetComponent<SpriteRenderer>();


            string default_name = pieceImageObj.name;
            //player2 piece
            if (pieceID > 5)
            {
                eColor = SharedDataType.EColor.Black;
                pieceImageObj.name = "Black_" + default_name;
                pieceObj.transform.Rotate(new Vector3(0, 0, 180));
                BackColorRenderer.color = new Color(0, 0, 0, 0.8f);
            }
            else
            {
                eColor = SharedDataType.EColor.White;
                pieceImageObj.name = "White_" + default_name;
                pieceObj.transform.Rotate(new Vector3(0, 0, 0));
                BackColorRenderer.color = new Color(1f, 1f, 1f, 0.8f);
                
            }



        }

        public void SetColor(SharedDataType.EColor color)
        {

        }

        public void Promotion()
        {
            pieceImageObj.name = "chicken";
            PieceImageRenderer.sprite = UnityEngine.Resources.Load<Sprite>("images/n-piece4");

            if(eColor == SharedDataType.EColor.White)
            {
                pieceID = Environment.C1;
            }
            else
            {
                pieceID = Environment.C2;
            }
                

        }

        public void Start()
        {

        }

        //백컬러 오더 높이기
        //아이콘 이미지 오더 높이기
        public void Selected()
        {
            BackColorRenderer.sortingOrder += 2;
            PieceImageRenderer.sortingOrder += 2;
        }

        //백컬러 오더 원래대로
        //아이콘 이미지 오더 원래대로
        public void CancleSelected()
        {
            BackColorRenderer.sortingOrder -= 2;
            PieceImageRenderer.sortingOrder -= 2;
        }

        public void GetAllowedMove()
        {

        }
    }
}

