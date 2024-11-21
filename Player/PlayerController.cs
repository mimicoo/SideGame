using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody; //Rigidbody2D型の変数
    float axisH = 0.0f; //入力
    public float speed = 3.0f; //移動速度

    public float jump = 9.0f; //ジャンプ力
    public LayerMask groundLayer; //特定のレイヤー(Ground)情報を格納する変数 ※LayerMask型
    bool goJump = false; //ジャンプ開始したかを見るフラグ

    //アニメーション対応
    Animator animator; //Animatorコンポーネントの情報を格納したい変数

    //Animeのクリップ名を代入する変数達
    public string stopAnime = "PlayerStop"; 
    public string moveAnime = "PlayerMove"; 
    public string jumpAnime = "PlayerJump"; 
    public string goalAnime = "PlayerGoal"; 
    public string deadAnime = "PlayerOver";

    //アニメが切り替わったかどうかの検査用
    string nowAnime = "";
    string oldAnime = "";

    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2Dを取ってくる
        rbody = this.GetComponent<Rigidbody2D>();

        //Animatorを取ってくる
        animator = GetComponent<Animator>();
        nowAnime = stopAnime; //初期設定として「更新すべきアニメ」は"PlayerStop"にしておく
        oldAnime = stopAnime; //初期設定として「更新前のアニメ」も"PlayerStop"にしておく
    }

    // Update is called once per frame
    void Update()
    {
        //水平方向の入力をチェックする
        axisH = Input.GetAxisRaw("Horizontal");

        if (axisH > 0.0f)
        {
            //右移動
            Debug.Log("右移動");
            transform.localScale = new Vector2(1, 1); //絵を右向き
        }
        else if (axisH < 0.0f)
        {
            //左移動
            Debug.Log("左移動");
            transform.localScale = new Vector2(-1, 1); //絵を左向き
        }

        //スペースキーがおされたかどうか
        if (Input.GetButtonDown("Jump"))
        {
            Jump(); //ジャンプメソッドの発動
        }
    }

    void FixedUpdate()
    {
        //地上判定
        //円のセンサーを設置して、指定した特定レイヤーに引っかかればtrue
        bool onGround = Physics2D.CircleCast(
            transform.position, //発射位置→プレイヤーの位置（足元）
            0.2f, //円の半径
            Vector2.down, //発射方向 <new Vector2(0,-1)>=Vector2.down
            0.0f, //発射距離
            groundLayer //対象レイヤー
            );


        //地面にいる時はvelocityがすべてに反応
        //空中にいる時はvelocityは左右だけに反応
        if (onGround || axisH != 0)
        {
            //速度を更新する
            rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);
        }

        //地面にいる＆スペースキーが押された
        if (onGround && goJump)
        {
            //後のAddForceメソッドの第一引数にいれる方向データをあらかじめ作っておく
            Vector2 jumpPw = new Vector2(0, jump);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse); //jumpPwの方向に瞬間的な力を加えて押し出す
            goJump = false; //ジャンプフラグを元に戻しておく
        }

        //アニメーション更新
        if(onGround) //地面の上なら
        {
            if(axisH == 0)
            {
                nowAnime = stopAnime; //「PlayerStop」クリップに更新すべき
            }
            else
            {
                nowAnime = moveAnime; //「PlayerMove」クリップに更新すべき
            }
        }
        else //空中なら
        {
            nowAnime = jumpAnime; //「PlayerJump」クリップに更新すべき
        }

        //「更新すべきアニメ」情報が「1フレーム前までのアニメ」情報と異なるのであれば
        if (nowAnime != oldAnime) 
        {
            oldAnime = nowAnime; //次にそなえて情報更新
            //Animatorコンポーネントの機能を使ってnowAnimeに入っているクリップ名のアニメーションに切り替え
            animator.Play(nowAnime); 
        }
    }

    //自作メソッド ジャンプフラグを立てるメソッド
    public void Jump()
    {
        goJump = true; //ジャンプ開始されたフラグがtrue
    }

    void OnDrawGizmos()
    {
        // 円を描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }

    //当たり判定のある何かとぶつかったら起こるイベント
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Goal") //当たった相手(collision)のTagがGoalだったら
        {
            Goal(); //ゴールメソッドの発動
        }
        else if(collision.gameObject.tag == "Dead")//当たった相手(collision)のTagがDeadだったら
        {
            GameOver();//ゲームオーバーメソッドの発動
        }
    }

    public void Goal()
    {
        //Animatorコンポーネントの機能を使って「PlayerGoal」クリップに切り替え
        animator.Play(goalAnime);
    }

    public void GameOver()
    {
        //Animatorコンポーネントの機能を使って「PlayerOver」クリップに切り替え
        animator.Play(deadAnime);
    }
}

