using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField]
    private float xLimit = 2.5f;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Animator anim;

    private int _direction = 1;

    public int Direction { get { return _direction; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        _direction = (horizontalInput != 0) ? (int)Mathf.Sign(horizontalInput) : 0;
        transform.localScale = new Vector3(_direction != 0 ? _direction : transform.localScale.x, 0.5f, 1);

        if (_direction != 0)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime * _direction;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xLimit, xLimit), transform.position.y, transform.position.z);
        }

        if (_direction != 0)
        {
            anim.SetBool("Start", true);

        }
        else
        {
            anim.SetBool("Start", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            SoundManager.instance.PlayCoin();
            MenuManager.instance.IncreaseCoin();
            GameObject coinEffect = ObjectPooling.instance.GetPooledObject("CoinEffect");
            coinEffect.transform.position = transform.position;
            coinEffect.SetActive(true);

            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Enemy"))
        {
            MenuManager.instance.GameOver();
            gameObject.SetActive(false);
        }
    }
}
